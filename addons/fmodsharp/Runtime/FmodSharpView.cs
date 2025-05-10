using Godot;
using System;

#if TOOLS
[Tool]
public partial class FmodSharpView : Control
{
    [Export] public Button SelectFolder;
    [Export] public Button FetchBanks;
    [Export] public Label FolderLabel;
    [Export] public Label BankLabel;
    [Export] public Tree Tree;

    [Export] public Button CopyPathButton;
    [Export] public Button CopyGuidButton;

    [Export] public Button PlayButton;
    [Export] public Button StopButton;

    [Export] public ScrollContainer RightPanel;
    
    [Export] public Label EventPathLabel;
    [Export] public Label EventGuidLabel;
    
    [Export] public CheckBox AutoFetchOnFocus;

    public override void _EnterTree()
    {
        var texture = EditorInterface.Singleton.GetEditorTheme().GetIcon("ActionCopy", "EditorIcons");
        CopyPathButton.Icon = texture;
        CopyGuidButton.Icon = texture;
        
        var playTexture = EditorInterface.Singleton.GetEditorTheme().GetIcon("Play", "EditorIcons");
        PlayButton.Icon = playTexture;
        
        var stopTexture = EditorInterface.Singleton.GetEditorTheme().GetIcon("Stop", "EditorIcons");
        StopButton.Icon = stopTexture;
    }
}
#endif