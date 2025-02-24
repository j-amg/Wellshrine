using Godot;
using System;

public partial class Door : Area3D
{
    public string killPath = "res://levels/killZone.tscn";
    public string shrinePath = "res://levels/shrineZone.tscn";

    private PackedScene zoneToLoad;
    private bool open = false;
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        PreloadZone();
    }
    private void PreloadZone() => zoneToLoad = Global.Singleton.currentLevel % 3 == 0 ? GD.Load<PackedScene>(shrinePath) : GD.Load<PackedScene>(killPath);

    public void Open()
    {
        GD.Print("Door open");     
        open = true;   
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(GetNode<StaticBody3D>("doorMesh"), "position", new Vector3(0,-2.5f, 0), 1.0f);
    }

    private void OnBodyEntered(Node3D body)
    {
        GD.Print("test");
        if (!open) return;
        if (Global.Singleton.CurrentScene is not shrineZone) Global.Singleton.currentLevel += 1;

        Global.Singleton.GotoScene(zoneToLoad);
    }
}
