#if TOOLS
using Godot;
using System;
using System.IO;
using FMOD.Studio;
using FileAccess = Godot.FileAccess;

namespace Audio.FmodSharp;

[Tool]
public partial class FmodSharpEditor : EditorPlugin
{
	private FmodSharpView _dockView;
	private EditorFileDialog _fileDialog;
	private EventInstance _currentInstance;
	
	private Texture2D _bankIcon = ResourceLoader.Load<Texture2D>("uid://cfqs4khhcwp4c");
	private Texture2D _eventsIcon = ResourceLoader.Load<Texture2D>("uid://dv67yqss4qauw");
	
	private const string AutoFetchkey = "fmodsharp.autofetch";
	public override void _EnterTree()
	{
		_dockView = GD.Load<PackedScene>("uid://1qphoknjlixy").Instantiate<FmodSharpView>();
		var cache = ResourceLoader.Load<FmodSharpCache>("uid://c0qeurhxncbgw");
		AddControlToBottomPanel(_dockView, "FMOD Sharp", null);
		
		_fileDialog = new EditorFileDialog();
		_fileDialog.Mode = Window.ModeEnum.Maximized;
		_fileDialog.FileMode = EditorFileDialog.FileModeEnum.OpenDir;
		_fileDialog.Title = "Select FMOD Banks Folder";
		_fileDialog.Filters = new[] { "*.bank", "*.strings.bank" };
		_fileDialog.DirSelected += OnDirSelected;
		AddChild(_fileDialog);
		
		_dockView.SelectFolder.Pressed += OnSelectFolderPressed;
		_dockView.FetchBanks.Pressed += FetchBanks;
		_dockView.RightPanel.Visible = false;
		_dockView.PlayButton.Pressed += PlayCurrentInstance;
		_dockView.StopButton.Pressed += StopCurrentInstance;
		_dockView.Tree.ItemSelected += OnItemSelected;
		_dockView.Tree.CellSelected += OnItemSelected;
		_dockView.CopyGuidButton.Pressed += CopyGuid;
		_dockView.CopyPathButton.Pressed += CopyPath;
		
		_dockView.AutoFetchOnFocus.Pressed += () =>
		{
			if (_dockView.AutoFetchOnFocus.IsPressed())
			{
				EditorInterface.Singleton.GetEditorSettings().SetSetting(AutoFetchkey, true);
			}
			else
			{
				EditorInterface.Singleton.GetEditorSettings().SetSetting(AutoFetchkey, false);
			}
		};
		_dockView.AutoFetchOnFocus.ButtonPressed = EditorInterface.Singleton.GetEditorSettings().GetSetting(AutoFetchkey).AsBool();
		
		if (!string.IsNullOrEmpty(cache.BankFolderPath))
		{
			_dockView.FolderLabel.Text = $"Folder: {cache.BankFolderPath}";
		}
		TryPopulateBankPaths(cache);
	}
	
	public override void _Process(double delta)
	{
		FmodServer.Update();
	}

	public override void _Notification(int what)
	{
		if (what == NotificationApplicationFocusIn)
		{
			if(_dockView.AutoFetchOnFocus.IsPressed())
			{
				FetchBanks();
			}
		}
	}

	public override void _ExitTree()
	{
		_dockView.SelectFolder.Pressed -= OnSelectFolderPressed;
		_fileDialog.DirSelected -= OnDirSelected;
		_dockView.FetchBanks.Pressed -= FetchBanks;
		_dockView.Tree.ItemSelected -= OnItemSelected;
		_dockView.Tree.CellSelected -= OnItemSelected;
		_dockView.PlayButton.Pressed -= PlayCurrentInstance;
		_dockView.StopButton.Pressed -= StopCurrentInstance;
		_dockView.CopyGuidButton.Pressed -= CopyGuid;
		_dockView.CopyPathButton.Pressed -= CopyPath;
		_fileDialog.Free();
		RemoveControlFromBottomPanel(_dockView);
		_dockView.Free();
		FmodServer.Dispose();
	}

	private void CopyPath()
	{
		if (_currentInstance.isValid())
		{
			_currentInstance.getDescription(out var eventDescription);
			eventDescription.getPath(out var path);
			DisplayServer.ClipboardSet(path);
			//Show a small native godot notification
			EditorInterface.Singleton.GetEditorToaster().PushToast("Copied Path " + path, EditorToaster.Severity.Info);
		}
		else
		{
			GD.PrintErr("No valid instance to copy path from.");
		}
	}

	private void CopyGuid()
	{
		if (_currentInstance.isValid())
		{
			_currentInstance.getDescription(out var eventDescription);
			eventDescription.getPath(out var path);
			eventDescription.getID(out var id);
			var guid = id.ToString();
			DisplayServer.ClipboardSet(guid);
			EditorInterface.Singleton.GetEditorToaster().PushToast("Copied GUID " + guid, EditorToaster.Severity.Info);
		}
		else
		{
			GD.PrintErr("No valid instance to copy GUID from.");
		}
	}

	private void StopCurrentInstance()
	{
		if (_currentInstance.isValid())
		{
			_currentInstance.stop(STOP_MODE.IMMEDIATE);
		}
		else
		{
			GD.PrintErr("No valid instance to stop.");
		}
	}

	private void PlayCurrentInstance()
	{
		if (_currentInstance.isValid())
		{
			_currentInstance.start();
		}
		else
		{
			GD.PrintErr("No valid instance to play.");
		}
	}
	
	public partial class FmodEvent : GodotObject
	{
		public string Path { get; set; }
		public string Id { get; set; }
	}

	private void FetchBanks()
	{
		var cache = ResourceLoader.Load<FmodSharpCache>("uid://c0qeurhxncbgw");

		if (!TryPopulateBankPaths(cache))
		{
			return;
		}
		_dockView.Tree.Clear();
		
		var allEvents = FmodServer.GetAllEvents();
		cache.BankEvents = new();
		var root = _dockView.Tree.CreateItem();
		var stringsRoot = _dockView.Tree.CreateItem(root);
		stringsRoot.SetText(0, cache.StringsBankRelativePath);
		stringsRoot.SetIcon(0, _bankIcon);
		
		var stringBusesRoot = _dockView.Tree.CreateItem(stringsRoot);
		stringBusesRoot.SetText(0, "Buses");
		stringBusesRoot.SetIcon(0, _bankIcon);
		
		var vcas = _dockView.Tree.CreateItem(stringsRoot);
		vcas.SetText(0, "VCAs");
		vcas.SetIcon(0, _bankIcon);
		
		var events = _dockView.Tree.CreateItem(stringsRoot);
		events.SetText(0, "Events");
		events.SetIcon(0, _eventsIcon);
		
		var masterRoot = _dockView.Tree.CreateItem(root);
		masterRoot.SetText(0, cache.BankRelativePath);
		masterRoot.SetIcon(0, _bankIcon);
		
		var busesRoot = _dockView.Tree.CreateItem(masterRoot);
		busesRoot.SetText(0, "Buses");
		busesRoot.SetIcon(0, _bankIcon);
		
		var vcasRoot = _dockView.Tree.CreateItem(masterRoot);
		vcasRoot.SetText(0, "VCAs");
		vcasRoot.SetIcon(0, _bankIcon);
		
		var eventsRoot = _dockView.Tree.CreateItem(masterRoot);
		eventsRoot.SetText(0, "Events");
		eventsRoot.SetIcon(0, _eventsIcon);
		
		foreach (var e in allEvents)
		{
			e.getPath(out var path);
			e.getID(out var id);
			cache.BankEvents.Add(path, id.ToString());
			var item = _dockView.Tree.CreateItem(eventsRoot);
			var name = path.Substring(path.LastIndexOf('/') + 1);
			item.SetText(0, name);
			item.SetMetadata(0, new FmodEvent {Id = id.ToString(), Path = path});
			item.SetIcon(0, _eventsIcon);
		}
		ResourceSaver.Save(cache);
	}

	private void OnItemSelected()
	{
		_currentInstance.stop(STOP_MODE.IMMEDIATE);
		
		var selectedItem = _dockView.Tree.GetSelected();
		var fmodEvent = selectedItem.GetMetadata(0).As<FmodEvent>();
		if(fmodEvent == null)
		{
			_dockView.RightPanel.Visible = false;
			return;
		}
		_currentInstance.release();
		_dockView.RightPanel.Visible = true;
		_dockView.EventPathLabel.Text = $"{fmodEvent.Path}";
		_dockView.EventGuidLabel.Text = $"{fmodEvent.Id}";
		
		_currentInstance = FmodServer.CreateInstance(new Guid(fmodEvent.Id));
	}

	private bool TryPopulateBankPaths(FmodSharpCache cache)
	{
		if (string.IsNullOrEmpty(cache.BankFolderPath))
		{
			_dockView.BankLabel.Text = "No bank folder selected.";
			return false;
		}

		if (!DirAccess.DirExistsAbsolute(ProjectSettings.GlobalizePath(cache.BankFolderPath)))
		{
			_dockView.BankLabel.Text = "Bank folder does not exist.";
			return false;
		}
		
		var basePath = ProjectSettings.GlobalizePath(cache.BankFolderPath);
		var bankFiles = Directory.GetFiles(basePath, "*.bank", SearchOption.AllDirectories);
		var stringsFiles = Directory.GetFiles(basePath, "*.strings.bank", SearchOption.AllDirectories);
		
		if (bankFiles.Length == 0 || stringsFiles.Length == 0)
		{
			_dockView.BankLabel.Text = "Nothing found in the selected directory.";
			return false;
		}

		cache.BankPath = ProjectSettings.LocalizePath(bankFiles[0]);
		cache.StringsBankPath = ProjectSettings.LocalizePath(stringsFiles[0]);
		cache.BankRelativePath = Path.GetRelativePath(basePath, bankFiles[0]);
		cache.StringsBankRelativePath = Path.GetRelativePath(basePath, stringsFiles[0]);
		
		var unixTime = (long)FileAccess.GetModifiedTime(bankFiles[0]);
		var dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTime).ToLocalTime();

		var formatted = dateTime.ToString("dd/MM/yyyy HH:mm");
		_dockView.BankLabel.Text = $"Last Build: {formatted}";
		FmodServer.Initialize();
		return true;
	}

	private void OnSelectFolderPressed()
	{
		_fileDialog.Popup();
		_fileDialog.PopupCentered();
	}

	private void OnDirSelected(string path)
	{
		var cache = ResourceLoader.Load<FmodSharpCache>("uid://c0qeurhxncbgw");
		cache.BankFolderPath = path;
		_dockView.FolderLabel.Text = $"Folder: {cache.BankFolderPath}";
	}
}
#endif
