using Godot;
using System;

public partial class Door : Area3D
{
    [Export]
    public string levelPath = "res://levels/Level.tscn";
    private PackedScene zoneToLoad;
    private bool open = false;
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        PreloadZone(levelPath);
    }
    private void PreloadZone(string path) => zoneToLoad = GD.Load<PackedScene>(path);

    public void Open()
    {
        GD.Print("Door open");     
        open = true;   
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(GetNode<StaticBody3D>("doorMesh"), "position", new Vector3(0,-2.0f, 0), 1.0f);
    }

    private void OnBodyEntered(Node3D body)
    {
        GD.Print("test");
        if (!open) return;
        Global.Singleton.currentLevel += 1;
        Global.Singleton.GotoScene(zoneToLoad);
    }
}
