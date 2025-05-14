using System;

namespace FMOD;

public partial struct GUID : IEquatable<GUID>
{
    public GUID(Guid guid)
    {
        byte[] bytes = guid.ToByteArray();

        Data1 = BitConverter.ToInt32(bytes, 0);
        Data2 = BitConverter.ToInt32(bytes, 4);
        Data3 = BitConverter.ToInt32(bytes, 8);
        Data4 = BitConverter.ToInt32(bytes, 12);
    }

    public static GUID Parse(string s)
    {
        return new GUID(new Guid(s));
    }

    public bool IsNull
    {
        get
        {
            return Data1 == 0
                   && Data2 == 0
                   && Data3 == 0
                   && Data4 == 0;
        }
    }

    public override bool Equals(object other)
    {
        return (other is GUID) && Equals((GUID)other);
    }

    public bool Equals(GUID other)
    {
        return Data1 == other.Data1
               && Data2 == other.Data2
               && Data3 == other.Data3
               && Data4 == other.Data4;
    }

    public static bool operator ==(GUID a, GUID b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(GUID a, GUID b)
    {
        return !a.Equals(b);
    }

    public override int GetHashCode()
    {
        return Data1 ^ Data2 ^ Data3 ^ Data4;
    }

    public static implicit operator Guid(GUID guid)
    {
        return new Guid(guid.Data1,
            (short)((guid.Data2 >> 0) & 0xFFFF),
            (short)((guid.Data2 >> 16) & 0xFFFF),
            (byte)((guid.Data3 >> 0) & 0xFF),
            (byte)((guid.Data3 >> 8) & 0xFF),
            (byte)((guid.Data3 >> 16) & 0xFF),
            (byte)((guid.Data3 >> 24) & 0xFF),
            (byte)((guid.Data4 >> 0) & 0xFF),
            (byte)((guid.Data4 >> 8) & 0xFF),
            (byte)((guid.Data4 >> 16) & 0xFF),
            (byte)((guid.Data4 >> 24) & 0xFF)
        );
    }

    public override string ToString()
    {
        return ((Guid)this).ToString("B");
    }
}