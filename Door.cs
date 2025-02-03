using Godot;
using System;

public partial class Door : Area3D
{
    [Export]
    public string levelPath = "res://levels/Level.tscn";
    private PackedScene zoneToLoad;
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        PreloadZone(levelPath);
    }

    private void PreloadZone(string path)
    {
        zoneToLoad = GD.Load<PackedScene>(path);
    }

    private void OnBodyEntered(Node3D body)
    {
        if (Global.Singleton.CurrentZone.objectiveComplete) Global.Singleton.GotoZone(zoneToLoad);
    }
}
