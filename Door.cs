using Godot;
using System;

public partial class Door : Area3D
{
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (Global.Singleton.CurrentZone.objectiveComplete) Global.Singleton.GotoZone("res://levels/Level.tscn");
    }
}
