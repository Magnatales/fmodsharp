using System;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using Godot;
using FMOD;
using FMOD.Studio;
using Godot.Collections;
using FileAccess = Godot.FileAccess;

namespace Audio.FmodSharp;

/// <summary>
/// The FmodServer class manages the initialization, playback, and manipulation of FMOD audio assets.
/// </summary>
public static class FmodServer
{
    private static FmodSharpCache _cache;
    private static FMOD.Studio.System _system;
    private static Bank _bank;
    private static Bank _stringsBank;
    private static EventInstance _currentBgm;
    

    /// <summary>
    /// Gets whether the FMOD system has been initialized.
    /// </summary>
    public static bool IsInitialized { get; private set; } = false;

    /// <summary>
    /// Initializes the FMOD system and loads the necessary banks.
    /// </summary>
    public static void Initialize()
    {
        try
        {
            if (!IsInitialized)
            {
                NativeLibrary.SetDllImportResolver(typeof(FmodServer).Assembly, (name, assembly, path) =>
                {
                    var resolvedPath = name switch
                    {
                        "fmod" => ProjectSettings.GlobalizePath("res://addons/fmodsharp/NativeLib/fmod.dll"),
                        "fmodstudio" =>
                            ProjectSettings.GlobalizePath("res://addons/fmodsharp/NativeLib/fmodstudio.dll"),
                        _ => null
                    };

                    if (resolvedPath != null && File.Exists(resolvedPath))
                    {
                        return NativeLibrary.Load(resolvedPath);
                    }

                    return IntPtr.Zero;
                });
            }
        }
        catch (Exception _)
        {
            // ignored
        }
        
        if(IsInitialized)
            Dispose();

        _cache = ResourceLoader.Load<FmodSharpCache>("uid://c0qeurhxncbgw");
        if (string.IsNullOrEmpty(_cache.BankPath) || string.IsNullOrEmpty(_cache.StringsBankPath))
        {
            GD.PrintErr($"{nameof(FmodServer)}: Bank path or strings path are null or empty.");
            return;
        }
        
        // The FMOD docs recommend calling any Core API before initializing the system to ensure the fmod.dll is loaded first.
        Memory.GetStats(out _, out _);
        
        var result = FMOD.Studio.System.create(out _system);
        Check(result);
        
        result = _system.initialize(512, FMOD.Studio.INITFLAGS.NORMAL, FMOD.INITFLAGS.NORMAL, IntPtr.Zero);
        Check(result);
        
        var bankPath = ProjectSettings.GlobalizePath(_cache.BankPath);
        var bankData = FileAccess.GetFileAsBytes(bankPath);
        result = _system.loadBankMemory(bankData, LOAD_BANK_FLAGS.NORMAL, out _bank);
        Check(result);
        
        var stringsPath = ProjectSettings.GlobalizePath(_cache.StringsBankPath);
        var stringsData = FileAccess.GetFileAsBytes(stringsPath);
        result = _system.loadBankMemory(stringsData, LOAD_BANK_FLAGS.NORMAL, out _stringsBank);
        Check(result);
        IsInitialized = true;
    }
    
    /// <summary>
    /// Updates the FMOD system. Should be called in the _Process function.
    /// </summary>
    public static void Update()
    {
        if (!IsInitialized) return;
        _system.update();
    }

    /// <summary>
    /// Plays an event by its unique identifier.
    /// </summary>
    /// <param name="guid">The unique identifier of the FMOD event.</param>
    public static void Play(Guid guid)
    {
        if (!CheckInitialized()) return;
        
        var result = _system.getEventByID(new GUID(guid), out var eventDescription);
        Check(result);
        
        result = eventDescription.createInstance(out var instance);
        Check(result);
        result = instance.start();
        Check(result);
        result = instance.release();
        Check(result);
    }
    
    /// <summary>
    /// Plays background music (BGM) by its unique identifier, stopping any currently playing BGM.
    /// </summary>
    /// <param name="guid">The unique identifier of the FMOD event.</param>
    /// <param name="stopMode">The stop mode for the BGM (default is allow fade out).</param>
    public static void PlayBgm(Guid guid, STOP_MODE stopMode = STOP_MODE.ALLOWFADEOUT)
    {
        if (!CheckInitialized()) return;
        
        var result = _system.getEventByID(new GUID(guid), out var eventDescription);
        Check(result);

        if (_currentBgm.isValid())
        {
            _currentBgm.stop(stopMode);
            _currentBgm.release();
        }
        
        result = eventDescription.createInstance(out _currentBgm);
        Check(result);
        result = _currentBgm.start();
        Check(result);
    }
    
    /// <summary>
    /// Plays background music (BGM) by its path, stopping any currently playing BGM.
    /// </summary>
    /// <param name="path">The path to the FMOD event.</param>
    /// <param name="stopMode">The stop mode for the BGM (default is allow fade out).</param>
    public static void PlayBgm(string path, STOP_MODE stopMode = STOP_MODE.ALLOWFADEOUT)
    {
        if (!CheckInitialized()) return;
        
        var result = _system.getEvent(path, out var eventDescription);
        Check(result);
        
        if (_currentBgm.isValid())
        {
            _currentBgm.stop(stopMode);
            _currentBgm.release();
        }
        
        result = eventDescription.createInstance(out _currentBgm);
        Check(result);
        result = _currentBgm.start();
        Check(result);
    }
    
    /// <summary>
    /// Plays an event by its path.
    /// </summary>
    /// <param name="path">The path to the FMOD event.</param>
    public static void Play(string path)
    {
        if (!CheckInitialized()) return;
        
        var result = _system.getEvent(path, out var eventDescription);
        Check(result);
        
        result = eventDescription.createInstance(out var instance);
        Check(result);
        result = instance.start();
        Check(result);
        result = instance.release();
        Check(result);
    }
    
    /// <summary>
    /// Sets the volume of a bus by its path.
    /// </summary>
    /// <param name="path">The path to the FMOD bus.</param>
    /// <param name="volume">The volume level to set (0.0 to 1.0).</param>
    public static void SetBusVolume(string path, float volume)
    {
        if (!CheckInitialized()) return;
        var result = _system.getBus(path, out var bus);
        Check(result);
        bus.setVolume(volume);
    }
    
    /// <summary>
    /// Sets the volume of a bus by its unique identifier.
    /// </summary>
    /// <param name="guid">The unique identifier of the FMOD bus.</param>
    /// <param name="volume">The volume level to set (0.0 to 1.0).</param>
    public static void SetBusVolume(Guid guid, float volume)
    {
        if (!CheckInitialized()) return;
        var result = _system.getBusByID(new GUID(guid), out var bus);
        Check(result);
        bus.setVolume(volume);
    }

    /// <summary>
    /// Mutes or unmutes a bus by its path.
    /// </summary>
    /// <param name="path">The path to the FMOD bus.</param>
    /// <param name="mute">True to mute, false to unmute.</param>
    public static void MuteBus(string path, bool mute)
    {
        if (!CheckInitialized()) return;
        var result = _system.getBus(path, out var bus);
        Check(result);
        bus.setMute(mute);
    }
    
    /// <summary>
    /// Mutes or unmutes a bus by its unique identifier.
    /// </summary>
    /// <param name="guid">The unique identifier of the FMOD bus.</param>
    /// <param name="mute">True to mute, false to unmute.</param>
    public static void MuteBus(Guid guid, bool mute)
    {
        if (!CheckInitialized()) return;
        var result = _system.getBusByID(new GUID(guid), out var bus);
        Check(result);
        bus.setMute(mute);
    }
    
    /// <summary>
    /// Sets a parameter for an event instance.
    /// </summary>
    /// <param name="instance">The event instance.</param>
    /// <param name="paramName">The name of the parameter.</param>
    /// <param name="value">The value to set for the parameter.</param>
    public static void SetParameter(EventInstance instance, string paramName, float value)
    {
        if (!CheckInitialized()) return;
        var result = instance.setParameterByName(paramName, value);
        Check(result);
    }
    
    /// <summary>
    /// Creates an instance of an event by its path.
    /// </summary>
    /// <param name="path">The path to the FMOD event.</param>
    /// <returns>A new event instance.</returns>
    public static EventInstance CreateInstance(string path)
    {
        if (!CheckInitialized())
            return default;
        
        var result = _system.getEvent(path, out var eventDescription);
        Check(result);
        
        result = eventDescription.createInstance(out var instance);
        Check(result);
        return instance;
    }
    
    /// <summary>
    /// Creates an instance of an event by its unique identifier.
    /// </summary>
    /// <param name="guid">The unique identifier of the FMOD event.</param>
    /// <returns>A new event instance.</returns>
    public static EventInstance CreateInstance(Guid guid)
    { 
        if (!CheckInitialized())
            return default;
        
        var result = _system.getEventByID(new GUID(guid), out var eventDescription);
        Check(result);
        
        result = eventDescription.createInstance(out var instance);
        Check(result);
        return instance;
    }
    
    /// <summary>
    /// Retrieves an event description by its path.
    /// </summary>
    /// <param name="path">The path to the FMOD event.</param>
    /// <returns>The event description.</returns>
    public static EventDescription GetEvent(string path)
    {
        if (!CheckInitialized()) return default;
        
        var result = _system.getEvent(path, out var eventDescription);
        Check(result);
        return eventDescription;
    }

    /// <summary>
    /// Retrieves all event descriptions loaded in the system.
    /// </summary>
    /// <returns>An array of all event descriptions.</returns>
    public static EventDescription[] GetAllEvents()
    {
        if (!CheckInitialized()) return null;
        
        _bank.getEventList(out var eventDescriptions);
        return eventDescriptions;
    }
    
    /// <summary>
    /// Sets a callback for an event instance.
    /// </summary>
    /// <param name="instance">The event instance.</param>
    /// <param name="callback">The callback to set.</param>
    /// <param name="type">The type of callback to use.</param>
    public static void SetCallback(EventInstance instance, EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE type)
    {
        var result = instance.setCallback(callback, type);
        Check(result);
    }

    /// <summary>
    /// Checks if the result from an FMOD operation is valid.
    /// </summary>
    private static void Check(RESULT result)
    {
        if (result != RESULT.OK)
        {
            GD.PrintErr($"{nameof(FmodServer)}: {result}");
        }
    }
    
    /// <summary>
    /// Checks if FMOD has been initialized.
    /// </summary>
    private static bool CheckInitialized()
    {
        if (IsInitialized) return true;
        
        GD.PrintErr($"{nameof(FmodServer)}: Not initialized. \n1. Fetch your banks through the FmodSharp panel at the bottom.\n2. Call FmodServer.Initialize() \n3. Call FmodServer.Update() in your _Process(double delta) \n4. Enjoy!");
        return false;
    }
    
    /// <summary>
    /// Disposes of the FMOD system and resources.
    /// </summary>
    public static void Dispose()
    {
        if (!IsInitialized) return;
        
        if (_currentBgm.isValid())
        {
            _currentBgm.stop(STOP_MODE.IMMEDIATE);
            _currentBgm.release();
        }

        if (_stringsBank.isValid())
        {
            _stringsBank.unload();
            _stringsBank.clearHandle();
        }

        if (_bank.isValid())
        {
            _bank.unload();
            _bank.clearHandle();
        }

        if (_system.isValid())
        {
            _system.release();
            _system.clearHandle();
        }
     
        _cache = null;
        IsInitialized = false;
    }
}