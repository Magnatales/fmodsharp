using System;
using Godot;
using FMOD;
using FMOD.Studio;
using Godot.Collections;
using FileAccess = Godot.FileAccess;

public static class FmodServer
{
    private static FmodSharpCache _cache;
    private static FMOD.Studio.System _system;
    private static Bank _bank;

    public static bool IsInitialized { get; private set; } = false;

    public static void Initialize()
    {
        if (_system.isValid())
        {
            _system.release();
            _system.clearHandle();
        }

        if (_bank.isValid())
        {
            _bank.unload();
            _bank.clearHandle();
        }
        
        _cache = ResourceLoader.Load<FmodSharpCache>("uid://c0qeurhxncbgw");
        if(string.IsNullOrEmpty(_cache.BankPath)|| string.IsNullOrEmpty(_cache.StringsBankPath))
        {
            GD.PrintErr($"{nameof(FmodServer)}: Bank path or strings path are null or empty.");
            return;
        }
        
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
        result = _system.loadBankMemory(stringsData, LOAD_BANK_FLAGS.NORMAL, out var strings);
        Check(result);
        IsInitialized = true;
    }
    
    public static void Update()
    {
        if (!IsInitialized) return;
        _system.update();
    }

    public static void Play(Guid guid)
    {
        if (!CheckInitialized()) return;
        
        var tree = new Tree();
        var rootItem = tree.CreateItem();
        rootItem.SetMetadata(0, new TestFmod());
        Variant data = rootItem.GetMetadata(0);

        
        var result = _system.getEventByID(new GUID(guid), out var eventDescription);
        Check(result);
        
        result = eventDescription.createInstance(out var instance);
        Check(result);
        result = instance.start();
        Check(result);
        result = instance.release();
        Check(result);
    }
    
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

    public static EventDescription[] GetAllEvents()
    {
        if (!CheckInitialized()) return null;
        
        _bank.getEventList(out var eventDescriptions);
        return eventDescriptions;
    }

    private static void Check(RESULT result)
    {
        if (result != RESULT.OK)
        {
            GD.PrintErr($"{nameof(FmodServer)}: {result}");
        }
    }
    
    private static bool CheckInitialized()
    {
        if (IsInitialized) return true;
        
        GD.PrintErr($"{nameof(FmodServer)}: Not initialized. \n1. Fetch your banks through the FmodSharp panel at the bottom.\n2. Call FmodServer.Initialize() \n3. Call FmodServer.Update() in your _Process(double delta) \n4. Enjoy!");
        return false;
    }
    
    public static void Dispose()
    {
        if (!IsInitialized) return;
        
        if (_system.isValid())
        {
            _system.release();
            _system.clearHandle();
        }

        if (_bank.isValid())
        {
            _bank.unload();
            _bank.clearHandle();
        }
     
        _cache = null;
        IsInitialized = false;
    }
}
