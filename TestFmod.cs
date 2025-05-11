using System;
using Audio.FmodSharp;
using Godot;
using Environment = System.Environment;

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
            FmodServer.PlayBgm(new Guid("{b522a844-9eab-4069-b7cf-e2e0e6386e60}"));
        }
        
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            FmodServer.PlayBgm(new Guid("{426d2e95-a1af-4065-8d20-d5add94e48bd}"));
        }

    }
}
