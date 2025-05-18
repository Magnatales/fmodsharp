using Godot;
using Audio.FmodSharp;
using FMOD.Studio;

public class DebugSoundInstance(string path, EventInstance instance, Vector2 position, float minDistance, float maxDistance, bool is3D)
{
    public string path = path;
    public EventInstance instance = instance;
    public Vector2 position = position;
    public float minDistance = minDistance;
    public float maxDistance = maxDistance;
    public bool is3D = is3D;
}

public partial class FmodWorldDebugger : Node2D
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
            var pos = ToLocal(sound.position);
            //pos = pos.Round();
            var text = sound.path;
            var textSize = font.GetStringSize(text);
            var textOffset = textSize / 2;
            textOffset.Y += 18f;

            if (sound.is3D)
            {
                var minPixels = sound.minDistance * 100f;
                var maxPixels = sound.maxDistance * 100f;

                DrawCircle(pos, maxPixels, new Color(0.2f, 0.6f, 1f, 0.4f));
                DrawCircle(pos, maxPixels - minPixels, new Color(0.7f, 0.2f, 1f, 0.4f));
                
                Vector2 up = new Vector2(0, -22);
                Vector2 left = new Vector2(-18, 0);
                Vector2 down = new Vector2(0, 22);
                Vector2 right = new Vector2(18, 0);
                Vector2[] points = {
                    pos + up,
                    pos + right,
                    pos + down,
                    pos + left
                };
                
                DrawPolygon(points, new Color[] {
                    new Color(1f, 0.2f, 0.2f, 0.6f),
                    new Color(1f, 0.2f, 0.2f, 0.6f),
                    new Color(1f, 0.2f, 0.2f, 0.6f),
                    new Color(1f, 0.2f, 0.2f, 0.6f)
                });

            
            
                DrawLine(points[0], points[1], Colors.White, 2f, true);
                DrawLine(points[1], points[2], Colors.White, 2f, true);
                DrawLine(points[2], points[3], Colors.White, 2f, true);
                DrawLine(points[3], points[0], Colors.White, 2f, true);

                DrawStringOutline(font, pos - textOffset, text, modulate: new Color(0, 0, 0), size: 6, width: -0.5f,
                    fontSize: 19);
                DrawString(font, pos - textOffset, text, modulate: new Color(0.9f, 0.9f, 0.9f), fontSize: 19);
            }
        }

        foreach (var listener in FmodListener.Listeners)
        {
            var pos = ToLocal(listener.GlobalPosition);
            
            Vector2 up = new Vector2(0, -22);
            Vector2 left = new Vector2(-18, 0);
            Vector2 down = new Vector2(0, 22);
            Vector2 right = new Vector2(18, 0);
            Vector2[] points = {
                pos + up,
                pos + right,
                pos + down,
                pos + left
            };
            
            var text = $"listener:/{listener.Name}";
            var textSize = font.GetStringSize(text);
            var textOffset = textSize / 2;
            textOffset.Y += 18f;

            DrawPolygon(points, new Color[] {
                new Color(0.2f, 1f, 0.2f, 0.6f),
                new Color(0.2f, 1f, 0.2f, 0.6f),
                new Color(0.2f, 1f, 0.2f, 0.6f),
                new Color(0.2f, 1f, 0.2f, 0.6f)
            });
            
            
            DrawLine(points[0], points[1], Colors.White, 2f, true);
            DrawLine(points[1], points[2], Colors.White, 2f, true);
            DrawLine(points[2], points[3], Colors.White, 2f, true);
            DrawLine(points[3], points[0], Colors.White, 2f, true);
            
            DrawStringOutline(font, pos - textOffset, text,  modulate:new Color(0 , 0, 0), size:6, width: -0.5f, fontSize: 19);
            DrawString(font, pos - textOffset, text, modulate: new Color(0.9f , 0.9f, 0.9f), fontSize: 19);
        }
    }
}
