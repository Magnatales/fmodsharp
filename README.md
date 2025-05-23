![logo](https://github.com/user-attachments/assets/ae776cd4-4de1-4c6a-ab98-e70b0c181956)

FmodSharp is a C# integration for FMOD in Godot

Event UI based on [this integration](https://github.com/utopia-rise/fmod-gdextension)

## Known Issues

| Issue                  | Description                                           | Workaround  |
|------------------------|-------------------------------------------------------|-------------|
| Single bank only        | Only build metadata and assets to a single bank is supported.                | Give me some time.    |
| Parameters not filled | Parameters aren't filled in the editor view | Give me some time. |

---

## Installation

Add the following configuration to your `.csproj` file:

```xml
<ItemGroup>
    <Content Include="addons\fmodsharp\**\*.dll">
      <Link>%(Filename)%(Extension)</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
      <TargetPath>%(Filename)%(Extension)</TargetPath>
    </Content>
</ItemGroup>
```

In FMOD, you must select the build path to a desired path inside Godot. I recommend that you use the Build folder inside of `addons/fmodsharp/Build`

![image](https://github.com/user-attachments/assets/1337d7cf-517d-48b6-9227-83c005ec7a22)

Once you build your banks in FMOD, in Godot, use the FMODSharp panel window at the bottom to select the folder and fetch the banks

![image](https://github.com/user-attachments/assets/2d3e2517-f4c4-4851-89a3-3386357c0198)

Add FmodGlobal.cs to your globals 
![image](https://github.com/user-attachments/assets/4a6382e0-a67f-4825-b883-87db7e0c72d5)


# General Tips
Always cache your callbacks, not caching a callback may lead to a crash when doing `instance.setCallback(Method);`.
For AOT compilers, make the callbacks static if possible and use the `MonoPInvokeCallbackAttribute` attribute
```cs
public partial class Example : Node2D
{
    private EVENT_CALLBACK _cachedCallback;
    
    public override void _Ready()
    {
        _cachedCallback = MyMethod;
        var instance = FmodServer.Play(new Guid("my-guid"));
        instance.setCallback(_cachedCallback);
    }

    [MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    private static FMOD.RESULT MyMethod(EVENT_CALLBACK_TYPE type, IntPtr ptr, IntPtr _)
    {
        GD.Print("Event callback: " + type);
        GD.Print("Pointer: " + ptr);
        return FMOD.RESULT.OK;
    }
}
```

# FmodServer Class Documentation

## Overview
The `FmodServer` class provides methods to initialize, update, and control FMOD events and audio systems in a Godot game. It manages the loading of audio banks, playing events, adjusting bus volumes, and handling background music (BGM). The class is used to interface with FMOD's audio engine to handle dynamic sound and music within the game.

| Method | Description | Example |
|--------|-------------|---------|
| `Initialize()` | Initializes the FMOD system, loads the bank, and prepares everything for use. | `FmodServer.Initialize();` |
| `Update()` | Updates the FMOD system, typically called every frame. Use it in your _Process method. | `FmodServer.Update();` |
| `Dispose()` | Shuts down the FMOD system and releases all resources. Call this before exiting the game. | `FmodServer.Dispose();` |
| `Play(Guid guid)` | Plays a sound event specified by its unique GUID. | `FmodServer.Play(new Guid("your-guid-here"));` |
| `Play(string path)` | Plays a sound event by its path. | `FmodServer.Play("event:/sound_effect");` |
| `PlayBgm(Guid guid, STOP_MODE stopMode)` | Plays a background music event by its GUID, with stop mode. | `FmodServer.PlayBgm(new Guid("your-guid-here"), STOP_MODE.ALLOWFADEOUT);` |
| `PlayBgm(string path, STOP_MODE stopMode)` | Plays a background music event by path, with stop mode. | `FmodServer.PlayBgm("event:/bgm_sound", STOP_MODE.ALLOWFADEOUT);` |
| `SetBusVolume(string path, float volume)` | Sets volume of a bus by path. | `FmodServer.SetBusVolume("bus:/bgm", 0.5f);` |
| `SetBusVolume(Guid guid, float volume)` | Sets volume of a bus by GUID. | `FmodServer.SetBusVolume(new Guid("your-guid-here"), 0.5f);` |
| `MuteBus(string path, bool mute)` | Mutes/unmutes a bus by path. | `FmodServer.MuteBus("bus:/bgm", true);` |
| `MuteBus(Guid guid, bool mute)` | Mutes/unmutes a bus by GUID. | `FmodServer.MuteBus(new Guid("your-guid-here"), true);` |
| `SetParameter(instance, paramName, value)` | Sets a parameter on an event instance. | `FmodServer.SetParameter(eventInstance, "volume", 0.5f);` |
| `CreateInstance(string path)` | Creates an instance of an FMOD event by path. | `var instance = FmodServer.CreateInstance("event:/sound_effect");` |
| `CreateInstance(Guid guid)` | Creates an instance of an FMOD event by GUID. | `var instance = FmodServer.CreateInstance(new Guid("your-guid-here"));` |
| `GetEvent(string path)` | Retrieves an event description by path. | `var e = FmodServer.GetEvent("event:/sound_effect");` |
| `GetAllEvents()` | Retrieves all events from the loaded bank. | `var all = FmodServer.GetAllEvents();` |
| `SetCallback(instance, callback, type)` | Sets a callback on an event instance. | `FmodServer.SetCallback(instance, callback, EVENT_CALLBACK_TYPE.START);` |

