using System;
using Audio.FmodSharp;
using FMOD.Studio;
using Godot;

public partial class Example : Node2D
{
    const float PIXELS_PER_METER = 100.0f;
    
    private EVENT_CALLBACK _callback;
    
    public override void _Ready()
    {
       FmodServer.Initialize();
       _callback = MyEventCallback;
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
            var instance = FmodServer.Play(new Guid("{2242f7d2-3a92-446e-80fc-9b44ce74c285}"), randomPos);
            instance.setCallback(_callback);
            FmodServer.PrintPerformanceData();
         
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
    
    FMOD.RESULT MyEventCallback(EVENT_CALLBACK_TYPE type, IntPtr ptr, IntPtr _)
    {
        GD.Print("Event callback: " + type);
        GD.Print("Pointer: " + ptr);
        return FMOD.RESULT.OK;
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

