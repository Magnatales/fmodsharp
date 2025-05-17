#if TOOLS
using Godot;
using System;
using Audio.FmodSharp;

[Tool]
public partial class FmodSharpBridge : Node
{
    public Godot.Collections.Array GetWrappedEvents()
    {
        var result = new Godot.Collections.Array();

        var allEvents = FmodServer.GetAllEventsStandalone();
        if (allEvents == null)
        {
            return result;
        }

        foreach (var desc in allEvents)
        {
            desc.getPath(out var path);
            desc.getID(out var guid);

            var wrapper = new FmodEvent(path, guid.ToString());
            result.Add(wrapper);
        }

        return result;
    }
}
#endif