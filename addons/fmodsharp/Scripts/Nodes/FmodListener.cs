using System.Collections.Generic;
using Godot;
using Audio.FmodSharp;

[Icon("uid://cq5arhf80ch8e")]
[GlobalClass]
public partial class FmodListener : Node2D
{
    [Export] private int _listenerIndex;
    [Export] private bool _debug;
    
    public static List<FmodListener> Listeners = new();

    public override void _EnterTree()
    {
        Listeners.Add(this);
    }
    
    public override void _ExitTree()
    {
        Listeners.Remove(this);
    }

    public override void _Process(double delta)
    {
        FmodServer.SetListenerLocation(_listenerIndex, this);
    }
}
