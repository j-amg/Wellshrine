using Godot;
using System;
using Godot.Collections;

public partial class Door : Area3D, IHoverable
{
    public PackedScene zoneToLoad;
    private bool open = false;
    public Color ReticleModulate { get; set; }
    public bool Active { get; set; }

    public override void _Ready()
    {
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
        zoneToLoad = GD.Load<PackedScene>(path);
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is not Player || zoneToLoad is null) return;
        Global.Singleton.GotoScene(zoneToLoad);
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
