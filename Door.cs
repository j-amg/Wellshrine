using Godot;
using System;
using Godot.Collections;

public partial class Door : Area3D
{
    public Array<string> killZones = new() {"killZone1", "killZone4"};
    public string shrinePath = "res://levels/shrineZone.tscn";
    private PackedScene zoneToLoad;
    private bool open = false;
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        PreloadZone();
    }
    private void PreloadZone()
    {
        if (Global.Singleton.currentLevel % 3 == 0) { zoneToLoad = GD.Load<PackedScene>(shrinePath);
        } else zoneToLoad = GD.Load<PackedScene>("res://levels/" + killZones.PickRandom() + ".tscn");
    }

    public void Open()
    {
        //GD.Print("Door open");     
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
