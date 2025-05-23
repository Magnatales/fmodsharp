using System.Collections.Generic;
using Godot;
using Audio.FmodSharp;

[Icon("uid://cq5arhf80ch8e")]
[GlobalClass]
public partial class FmodListener : Node2D
{
    public static List<FmodListener> Listeners = new();
    
    private int _listenerIndex;   

    public override void _EnterTree()
    {
        _listenerIndex = Listeners.Count;
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
