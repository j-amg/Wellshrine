using Godot;
using System;
using System.Diagnostics;

public partial class Explosion : Area3D
{
	private float explosionDur = .5f;
	private float explosionRadius = 2.5f;
	private float explosionSpeed = 0.05f;
	private float damageDelay = 0.05f;
	private int damage = 10;
	private MeshInstance3D mesh;
	private SphereMesh shape;
	private Godot.Collections.Array<Node3D> overlappingBodies;
	public override void _Ready()
	{
		mesh = GetNode<MeshInstance3D>("MeshInstance3D");
		shape = (SphereMesh)mesh.Mesh;
		shape.Radius = explosionRadius;
		shape.Height = 2*explosionRadius;
		Explode();
	}

    private async void Explode()
    {

		//await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		//await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

		Tween tween = GetTree().CreateTween();
		mesh.MaterialOverride = (Material)mesh.MaterialOverride.Duplicate();
		tween.TweenProperty(mesh.MaterialOverride, "albedo_color", new Color(0,0,0,0), explosionDur);
		tween.Parallel().TweenProperty(shape, "radius", explosionRadius, explosionSpeed).From(0f);
		tween.Parallel().TweenProperty(shape, "height", 2* explosionRadius, explosionSpeed).From(0f);

		await ToSignal(GetTree().CreateTimer(damageDelay), "timeout");

		foreach(Node3D node in overlappingBodies)
		{
			//GD.Print(node.Name);
    		if(node is IDamageable damageable) {
				GD.Print("hit enemy!");
        	damageable.Damage(5);
    		}
		}
		await ToSignal(tween, "finished");
		QueueFree();
    }

    public override void _PhysicsProcess(double delta)
    {
        overlappingBodies = GetOverlappingBodies();
    }
}
