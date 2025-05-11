![logo](https://github.com/user-attachments/assets/ae776cd4-4de1-4c6a-ab98-e70b0c181956)

FmodSharp is a C# integration for FMOD in Godot

Add this to your .csproj
```
<ItemGroup>
    <Content Include="addons\fmodsharp\**\*.dll">
      <Link>%(Filename)%(Extension)</Link>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
      <TargetPath>%(Filename)%(Extension)</TargetPath>
    </Content>
  </ItemGroup>
```

In FMOD, you must select the build path to a desired path inside Godot. I recommend that you use the Build folder inside of `addons/fmodsharp/Build`

![image](https://github.com/user-attachments/assets/7ccab10a-8ad7-4a17-8bc2-6c828df045be)

![image](https://github.com/user-attachments/assets/1337d7cf-517d-48b6-9227-83c005ec7a22)

Once you build your banks in FMOD, in Godot, use the FMODSharp panel window at the bottom to select the folder and fetch the banks

![image](https://github.com/user-attachments/assets/2d3e2517-f4c4-4851-89a3-3386357c0198)



Lately, in code:
1. Call FmodServer.Initialize()
2. Call FmodServer.Update() in your _Process(double delta)
3. Enjoy!

# FmodServer Class Documentation

## Overview
The `FmodServer` class provides methods to initialize, update, and control FMOD events and audio systems in a Godot game. It manages the loading of audio banks, playing events, adjusting bus volumes, and handling background music (BGM). The class is used to interface with FMOD's audio engine to handle dynamic sound and music within the game.

## Methods

### `Initialize()`
Initializes the FMOD system and loads the necessary audio banks. Should be called once at the start of the game.

### `Update()`
Updates the FMOD system. This method should be called in the `_Process` function of the game loop to ensure FMOD events are updated each frame.

### `Play(Guid guid)`
Plays an event using the provided unique identifier.

### `PlayBgm(Guid guid, STOP_MODE stopMode = STOP_MODE.ALLOWFADEOUT)`
Plays background music (BGM) using a unique event identifier. If a BGM is already playing, it will be stopped and replaced.

### `PlayBgm(string path, STOP_MODE stopMode = STOP_MODE.ALLOWFADEOUT)`
Plays background music (BGM) using the event path. If a BGM is already playing, it will be stopped and replaced.

### `Play(string path)`
Plays an event using the provided path.

### `SetBusVolume(string path, float volume)`
Sets the volume of a specific FMOD bus using the event path.

### `SetBusVolume(Guid guid, float volume)`
Sets the volume of a specific FMOD bus using the event's unique identifier.

### `MuteBus(string path, bool mute)`
Mutes or unmutes a specific FMOD bus using the event path.

### `MuteBus(Guid guid, bool mute)`
Mutes or unmutes a specific FMOD bus using the event's unique identifier.

### `SetParameter(EventInstance instance, string paramName, float value)`
Sets a parameter for an event instance.

### `CreateInstance(string path)`
Creates a new instance of an event using the event path.

### `CreateInstance(Guid guid)`
Creates a new instance of an event using the event's unique identifier.

### `GetEvent(string path)`
Retrieves an event description by its path.

### `GetAllEvents()`
Retrieves all event descriptions loaded in the system.

### `SetCallback(EventInstance instance, EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE type)`
Sets a callback for an event instance.

### `Dispose()`
Releases all FMOD resources and stops any playing events.

## Properties

### `IsInitialized`
A boolean property indicating whether the FMOD system has been successfully
