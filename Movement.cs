using Godot;
using System;

public partial class Movement : Node2D
{
    public override void _Process(double delta)
    {
        Vector2 velocity = Vector2.Zero;

        if (Input.IsActionPressed("ui_up")) velocity.Y -= 1;
        if (Input.IsActionPressed("ui_down")) velocity.Y += 1;
        if (Input.IsActionPressed("ui_left")) velocity.X -= 1;
        if (Input.IsActionPressed("ui_right")) velocity.X += 1;

        Position += velocity.Normalized() * 200f * (float)delta;
    }
}
