using Godot;
using System;
using Godot.Collections;

public partial class Door : Area3D
{
    [Export]
    public PackedScene zoneOverride;
    [Export]
    public bool startOpen = false;
    public string shrinePath = "res://zones/shrineZone.tscn";
    private PackedScene zoneToLoad;
    private bool open = false;
    public Zone zone;
    private bool active;
    public override void _Ready()
    {
        zone = GetOwner<Zone>();
        BodyEntered += OnBodyEntered;
        PreloadZone();
        if (startOpen) CallDeferred("Open");
    }

    private void PreloadZone()
    {
        if (zoneOverride != null) {zoneToLoad = zoneOverride;} else{
            if (Global.Singleton.currentLevel % 3 == 0) { zoneToLoad = GD.Load<PackedScene>(shrinePath);
            } else zoneToLoad = GD.Load<PackedScene>("res://zones/" + Global.Singleton.killZones.PickRandom() + ".tscn");
        }
    }

    public void Open()
    {   
        if (open) return;
        open = true;   
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(GetNode<StaticBody3D>("doorMesh"), "position", new Vector3(0,-2.5f, 0), 1.0f);
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is not Player) return;
        if (zone.increasesLevel) Global.Singleton.currentLevel += 1;
        Global.Singleton.GotoScene(zoneToLoad);
    }
}
