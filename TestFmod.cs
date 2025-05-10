using System;
using Godot;

public partial class TestFmod : Node
{
    public override void _Ready()
    {
       FmodServer.Initialize();
    }
    
    
    public override void _Process(double delta)
    {
        FmodServer.Update();
        if (Input.IsActionJustPressed("ui_accept"))
        {
            FmodServer.Play(new Guid("{2242f7d2-3a92-446e-80fc-9b44ce74c285}"));
        }

    }
}
