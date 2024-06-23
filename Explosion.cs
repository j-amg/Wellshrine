using Godot;
using System;

public partial class Explosion : Area3D
{
	private float explosionDur = 2;
	private MeshInstance3D mesh;
	public override void _Ready()
	{
		mesh = GetNode<MeshInstance3D>("MeshInstance3D");
		Explode();
	}

    private async void Explode()
    {
        Tween tween = GetTree().CreateTween();
		tween.TweenProperty(mesh.MaterialOverride, "albedo_color", new Color(0,0,0,1), explosionDur);
		//await ToSignal(tween, "finished");
		//QueueFree();
    }
}
