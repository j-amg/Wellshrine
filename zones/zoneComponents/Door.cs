using Godot;
using System;
using Godot.Collections;

public partial class Door : Area3D, IHoverable
{
    public Zone zoneToLoad;
    [Export] Node3D respawnPoint;
    private bool open = false;
    public Color ReticleModulate { get; set; }
    public bool Active { get; set; }

    public override void _Ready()
    {
        //Global.Singleton.inventory.door
        BodyEntered += OnBodyEntered;
    }
    public void Open()
    {
        if (open) return;
        open = true;
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(GetNode<StaticBody3D>("doorMesh"), "position", new Vector3(0, -2.5f, 0), 1.0f);
    }

    public void SetDestination(string path)
    {
        GD.Print("destination set to: " + path);
        zoneToLoad = Zone.GenerateTileZone();
        Global.Singleton.doorZone = zoneToLoad;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is not Player) return;
        GD.Print("body entered");
        Global.Singleton.GotoZone(zoneToLoad, respawnPoint.Transform);
    }

    public void StartHover()
    {
        GD.Print(zoneToLoad);
    }

    public void EndHover()
    {
        throw new NotImplementedException();
    }
}
