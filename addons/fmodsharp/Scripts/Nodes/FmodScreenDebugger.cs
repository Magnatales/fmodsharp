using Godot;
using System;
using Audio.FmodSharp;

public partial class FmodScreenDebugger : Control
{
    public override void _Process(double delta)
    {
        QueueRedraw();
    }

    public override void _Draw()
    {
        var font = ThemeDB.FallbackFont;
        
        for (var index = FmodServer.DebugSoundInstances.Count - 1; index >= 0; index--)
        {
            var sound = FmodServer.DebugSoundInstances[index];
            var pos = sound.position;
            var text = sound.path;

            var margin = new Vector2(40f, 40f);
            var getHeightOfFont = font.GetHeight();
            var lineHeight = getHeightOfFont + 2f;
            var screenPos = margin + new Vector2(0f, index * lineHeight);

            var displayText = $"> FMOD: {text}";

            if (sound.is3D)
                displayText += $" pos: {pos}";

            DrawStringOutline(font, screenPos, displayText, modulate: new Color(0, 0, 0), size: 6, width: -0.5f,
                fontSize: 19);
            DrawString(font, screenPos, displayText, modulate: new Color(0.9f, 0.9f, 0.9f), fontSize: 19);
        }
    }
}
