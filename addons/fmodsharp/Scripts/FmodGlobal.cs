using Godot;
using Audio.FmodSharp;

public partial class FmodGlobal : Node
{
    public override void _EnterTree()
    {
        FmodServer.Initialize();
        
        var cache = ResourceLoader.Load<FmodSharpCache>("uid://c0qeurhxncbgw");
        if (cache.Debug)
        {
            var scene = ResourceLoader.Load<PackedScene>("uid://bflv8elstveym");
            var debug = scene.Instantiate();
            AddChild(debug);
        }
    }

    public override void _Process(double delta)
    {
        FmodServer.Update();
    }

    public override void _ExitTree()
    {
        FmodServer.Dispose();
    }
    
    public override void _Notification(int what)
    {
        if (what == NotificationApplicationFocusIn)
        {
            FmodServer.SetMainBusPaused(false);
        }
        else if (what == NotificationApplicationFocusOut)
        {
            FmodServer.SetMainBusPaused(true);
        }
    }
}
