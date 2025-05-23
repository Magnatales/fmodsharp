using Godot;
using System;
using Audio.FmodSharp;

[Icon("uid://cq5arhf80ch8e")]
[GlobalClass]
public partial class FmodEventEmitter : Node2D
{
    [Export] private string _eventPathOrGuid;
    [Export] private bool _playOnReady = true;
    [Export] private bool _destroyOnPlay = false;

    public override void _Ready()
    {
        if (_playOnReady)
        {
            Play();
        }
    }

    public void Play()
    {
        if (string.IsNullOrEmpty(_eventPathOrGuid))
        {
            GD.PrintErr("Event path or GUID is not set.");
            return;
        }

        if (Guid.TryParse(_eventPathOrGuid, out var guid))
        {
            FmodServer.Play(guid, Position);
        }
        else
        {
            FmodServer.Play(_eventPathOrGuid, Position);
        }
        if (_destroyOnPlay)
        {
            QueueFree();
        }
    }
}
