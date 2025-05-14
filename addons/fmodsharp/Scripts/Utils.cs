using Godot;

namespace FMOD;

public static class Utils
{
    public static ATTRIBUTES_3D To3DAttributes(this Node2D node, Vector3 velocity = default)
    {
        var position = new Vector3(node.GlobalPosition.X, node.GlobalPosition.Y, 0);
        var forward = new Vector3(0, 0, -1);
        Vector3 up = new Vector3(0, 1, 0); 

        return new ATTRIBUTES_3D
        {
            position = position.ToFmodVector(),
            velocity = velocity.ToFmodVector(),
            forward = forward.ToFmodVector(),
            up = up.ToFmodVector()
        };
    }
    
    public static ATTRIBUTES_3D To3DAttributes(this Node3D node, Vector3 velocity = default)
    {
        var position = node.GlobalTransform.Origin;
        var forward = -node.GlobalTransform.Basis.Z;
        var up = node.GlobalTransform.Basis.Y;

        return new FMOD.ATTRIBUTES_3D
        {
            position = position.ToFmodVector(),
            velocity = velocity.ToFmodVector(),
            forward = forward.Normalized().ToFmodVector(),
            up = up.Normalized().ToFmodVector()
        };
    }
    
    public static ATTRIBUTES_3D To3DAttributes(this Vector2 pos)
    {
        var attributes = new ATTRIBUTES_3D
        {
            forward = Vector3.Forward.ToFmodVector(),
            up = Vector3.Up.ToFmodVector(),
            position = pos.ToFmodVector(),
        };
        return attributes;
    }

    
    public static VECTOR ToFmodVector(this Vector3 vec)
    {
        VECTOR temp;
        temp.x = vec.X;
        temp.y = vec.Y;
        temp.z = vec.Z;

        return temp;
    }
    
    public static VECTOR ToFmodVector(this Vector2 vec)
    {
        VECTOR temp;
        temp.x = vec.X;
        temp.y = vec.Y;
        temp.z = 0;

        return temp;
    }
}