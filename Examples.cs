using System;
using Audio.FmodSharp;
using FMOD.Studio;
using Godot;

public partial class Examples : Node2D
{
    // Always cache your fmod callbacks
    private EVENT_CALLBACK _cachedCallback;
    
    public override void _Ready()
    {
       FmodServer.Initialize();
       _cachedCallback = MyMethod;
    }
    
    public override void _Process(double delta)
    {
        FmodServer.Update();
        
        // if (Input.IsActionJustPressed("ui_cancel"))
        // {
        //     var screenSize = GetViewport().GetVisibleRect().Size;
        //     var randomPos = new Vector2(
        //         GD.Randf() * screenSize.X,
        //         GD.Randf() * screenSize.Y
        //     );
        //     var instance = FmodServer.Play(new Guid("{2242f7d2-3a92-446e-80fc-9b44ce74c285}"), randomPos);
        //     instance.setCallback(_cachedCallback);
        // }
        //
        // FmodServer.PrintPerformanceData();
    }
    
    // Callback, use this attribute for AOT compilers
    [MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    private FMOD.RESULT MyMethod(EVENT_CALLBACK_TYPE type, IntPtr ptr, IntPtr _)
    {
        GD.Print("Event callback: " + type);
        GD.Print("Pointer: " + ptr);
        return FMOD.RESULT.OK;
    }
    
    // Pause/Unpause when leaving the game
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

