using System.Collections.Generic;
using Godot;
using Audio.FmodSharp;

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
        
        //wasd movement
        if (Input.IsActionPressed("ui_up"))
        {
            Position += new Vector2(0, -1) * 200f * (float)delta;
        }
        if (Input.IsActionPressed("ui_down"))
        {
            Position += new Vector2(0, 1) * 200f * (float)delta;
        }
        if (Input.IsActionPressed("ui_left"))
        {
            Position += new Vector2(-1, 0) * 200f * (float)delta;
        }
        if (Input.IsActionPressed("ui_right"))
        {
            Position += new Vector2(1, 0) * 200f * (float)delta;
        }
    }
}
