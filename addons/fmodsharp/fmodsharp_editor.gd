@tool
extends EditorPlugin

var dock_view
var file_dialog
var current_instance

var bank_icon = preload("uid://cfqs4khhcwp4c")
var events_icon = preload("uid://dv67yqss4qauw")

const AUTO_FETCH_KEY = "fmodsharp.autofetch"
const DEBUG_KEY = "fmodsharp.debug"  # Fixed casing to be consistent

var loader

func _enter_tree():
	loader = preload("res://addons/fmodsharp/Scripts/FmodSharpBridge.cs").new()
	add_child(loader)

	dock_view = preload("uid://1qphoknjlixy").instantiate()
	add_control_to_bottom_panel(dock_view, "FMOD Sharp")

	file_dialog = EditorFileDialog.new()
	file_dialog.mode = Window.MODE_MAXIMIZED
	file_dialog.file_mode = EditorFileDialog.FILE_MODE_OPEN_DIR
	file_dialog.title = "Select FMOD Banks Folder"
	file_dialog.filters = ["*.bank", "*.strings.bank"]
	file_dialog.dir_selected.connect(_on_dir_selected)
	add_child(file_dialog)

	dock_view.SelectFolder.pressed.connect(_on_SelectFolder_pressed)
	dock_view.FetchBanks.pressed.connect(_FetchBanks)
	dock_view.RightPanel.visible = false
	dock_view.PlayButton.pressed.connect(_play_current_instance)
	# dock_view.stop_button.pressed.connect(_stop_current_instance)
	dock_view.tree.item_selected.connect(_on_item_selected)
	dock_view.tree.cell_selected.connect(_on_item_selected)
	dock_view.CopyGuidButton.pressed.connect(_copy_guid)
	dock_view.CopyPathButton.pressed.connect(_copy_path)

	dock_view.AutoFetchOnFocus.pressed.connect(func():
		EditorInterface.get_editor_settings().set_setting(AUTO_FETCH_KEY, dock_view.AutoFetchOnFocus.button_pressed)
	)
	dock_view.Debug.pressed.connect(func():
		var cache = ResourceLoader.load("uid://c0qeurhxncbgw")
		cache.Debug = dock_view.Debug.button_pressed
		EditorInterface.get_editor_settings().set_setting(DEBUG_KEY, cache.Debug)
		ResourceSaver.save(cache)
	)

	var auto_fetch = EditorInterface.get_editor_settings().get_setting(AUTO_FETCH_KEY)
	if auto_fetch == null:
		auto_fetch = false
		
	print(auto_fetch)
	dock_view.AutoFetchOnFocus.set_pressed_no_signal(auto_fetch)
	
	var debug_pressed = EditorInterface.get_editor_settings().get_setting(DEBUG_KEY)
	if debug_pressed == null:
		debug_pressed = false
	dock_view.Debug.button_pressed(debug_pressed)


	var cache = preload("uid://c0qeurhxncbgw")
	if cache.BankFolderPath != "":
		dock_view.FolderLabel.text = "Folder: %s" % cache.BankFolderPath
		_try_populate_bank_paths(cache)

func _notification(what):
	if what == NOTIFICATION_APPLICATION_FOCUS_IN:
		if dock_view.AutoFetchOnFocus.button_pressed:
			_FetchBanks()

func _exit_tree():
	if not dock_view:
		return

	dock_view.SelectFolder.pressed.disconnect(_on_SelectFolder_pressed)
	file_dialog.dir_selected.disconnect(_on_dir_selected)
	dock_view.FetchBanks.pressed.disconnect(_FetchBanks)
	dock_view.tree.item_selected.disconnect(_on_item_selected)
	dock_view.tree.cell_selected.disconnect(_on_item_selected)
	dock_view.CopyGuidButton.pressed.disconnect(_copy_guid)
	dock_view.CopyPathButton.pressed.disconnect(_copy_path)

	file_dialog.queue_free()
	remove_control_from_bottom_panel(dock_view)
	dock_view.queue_free()
	dock_view = null

func _copy_path():
	if current_instance:
		DisplayServer.clipboard_set(current_instance.Path)
		EditorInterface.get_editor_toaster().push_toast("Copied Path %s" % current_instance.Path, EditorToaster.SEVERITY_INFO)
	else:
		push_error("No valid instance to copy path from.")

func _copy_guid():
	if current_instance:
		DisplayServer.clipboard_set(current_instance.Guid)
		EditorInterface.get_editor_toaster().push_toast("Copied GUID %s" % current_instance.Guid, EditorToaster.SEVERITY_INFO)
	else:
		push_error("No valid instance to copy GUID from.")

func _play_current_instance():
	if current_instance and current_instance.is_valid():
		current_instance.start()
	else:
		push_error("No valid instance to play.")

func _FetchBanks():
	var cache = preload("uid://c0qeurhxncbgw")
	if not _try_populate_bank_paths(cache):
		return

	dock_view.tree.clear()
	var all_events = loader.GetWrappedEvents()
	if all_events == null or all_events.size() == 0:
		return

	var root = dock_view.tree.create_item()
	var master = dock_view.tree.create_item(root)
	master.set_text(0, cache.BankRelativePath)
	master.set_icon(0, bank_icon)

	var buses = dock_view.tree.create_item(master)
	buses.set_text(0, "Buses")
	buses.set_icon(0, bank_icon)

	var vcas = dock_view.tree.create_item(master)
	vcas.set_text(0, "VCAs")
	vcas.set_icon(0, bank_icon)

	var events = dock_view.tree.create_item(master)
	events.set_text(0, "Events")
	events.set_icon(0, events_icon)

	for e in all_events:
		var item = dock_view.tree.create_item(events)
		# safer substring: check rfind != -1 before adding 1
		var slash_pos = e.Path.rfind("/")
		var name = e.Path if slash_pos == -1 else e.Path.substr(slash_pos + 1)
		item.set_text(0, name)
		item.set_metadata(0, {"id": str(e.Guid), "path": e.Path})
		item.set_icon(0, events_icon)

	ResourceSaver.save(cache)

func _on_item_selected():

	var item = dock_view.tree.get_selected()
	if not item:
		dock_view.RightPanel.visible = false
		return

	var fmod_event = item.get_metadata(0)
	if typeof(fmod_event) != TYPE_DICTIONARY:
		dock_view.RightPanel.visible = false
		return

	dock_view.RightPanel.visible = true
	dock_view.EventPathLabel.text = fmod_event["path"]
	dock_view.EventGuidLabel.text = fmod_event["id"]
	
	current_instance = {"Guid": fmod_event["id"], "Path": fmod_event["path"]}

func _try_populate_bank_paths(cache) -> bool:
	if cache.BankFolderPath == "":
		dock_view.BankLabel.text = "No bank folder selected."
		return false

	if not DirAccess.dir_exists_absolute(ProjectSettings.globalize_path(cache.BankFolderPath)):
		dock_view.BankLabel.text = "Bank folder does not exist."
		return false

	var base_path = ProjectSettings.globalize_path(cache.BankFolderPath)
	var bank_files = get_bank_files(base_path)

	if bank_files["bank"] == null or bank_files["strings_bank"] == null:
		dock_view.BankLabel.text = "Nothing found in the selected directory."
		return false

	cache.BankPath = ProjectSettings.localize_path(bank_files["bank"])
	cache.StringsBankPath = ProjectSettings.localize_path(bank_files["strings_bank"])

	# Use helper function to compute relative path
	cache.BankRelativePath = _get_relative_path(base_path, bank_files["bank"])
	cache.StringsBankRelativePath = _get_relative_path(base_path, bank_files["strings_bank"])

	var modified_time = FileAccess.get_modified_time(bank_files["bank"])
	var date = Time.get_datetime_dict_from_unix_time(modified_time)
	dock_view.BankLabel.text = "Last Build: %02d/%02d/%d %02d:%02d" % [date.day, date.month, date.year, date.hour, date.minute]

	return true

func get_bank_files(base_path: String) -> Dictionary:
	var banks = {
		"bank": null,
		"strings_bank": null
	}

	var dir = DirAccess.open(base_path)
	if not dir:
		return banks

	var files = dir.get_files()
	for file_name in files:
		if file_name.ends_with(".strings.bank"):
			banks["strings_bank"] = base_path.path_join(file_name)
		elif file_name.ends_with(".bank"):
			if not file_name.ends_with(".strings.bank"):
				banks["bank"] = base_path.path_join(file_name)

	return banks

func _on_SelectFolder_pressed():
	file_dialog.popup_centered()

func _on_dir_selected(path):
	var cache = preload("uid://c0qeurhxncbgw")
	cache.BankFolderPath = path
	dock_view.FolderLabel.text = "Folder: %s" % cache.BankFolderPath

# Helper for relative paths
func _get_relative_path(base_path: String, target_path: String) -> String:
	# Ensure absolute paths
	var base_abs = ProjectSettings.globalize_path(base_path)
	var target_abs = ProjectSettings.globalize_path(target_path)
	if target_abs.begins_with(base_abs):
		return target_abs.substr(base_abs.length() + 1, target_abs.length())
	return target_path
