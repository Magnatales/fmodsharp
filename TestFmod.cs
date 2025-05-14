using System;
using Audio.FmodSharp;
using Godot;

public partial class TestFmod : Node2D
{
    const float PIXELS_PER_METER = 100.0f;
    
    public override void _Ready()
    {
       FmodServer.Initialize();
    }
    
    public override void _Process(double delta)
    {
        FmodServer.Update();
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            var screenSize = GetViewport().GetVisibleRect().Size;
            var randomPos = new Vector2(
                GD.Randf() * screenSize.X,
                GD.Randf() * screenSize.Y
            );
            FmodServer.Play(new Guid("{2242f7d2-3a92-446e-80fc-9b44ce74c285}"), randomPos);
        }

        if (Input.IsActionJustPressed("ui_copy"))
        {
            var screenSize = GetViewport().GetVisibleRect().Size;
            var randomPos = new Vector2(
                GD.Randf() * screenSize.X,
                GD.Randf() * screenSize.Y
            );
            FmodServer.Play(new Guid("{dd4eb15d-e1dc-4dec-afe0-bca2ca23955a}"), randomPos);
            
        }

        if (Input.IsActionJustPressed("ui_accept"))
        {
            var screenSize = GetViewport().GetVisibleRect().Size;
           
            FmodServer.Play(new Guid("{426d2e95-a1af-4065-8d20-d5add94e48bd}"), screenSize/2f);
        }
    }
  
}
