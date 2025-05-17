using Godot;

public partial class FmodEvent : GodotObject
{
    public string Path;
    public string Guid;

    public FmodEvent()
    {
        
    }

    public FmodEvent(string path, string guid)
    {
        Path = path;
        Guid = guid;
    }
}