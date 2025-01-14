using Godot;
using System;
using System.Diagnostics;

public partial class Projectile : Area3D
{
	public float muzzleVelocity = 25;
	//public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	Vector3 g = Vector3.Down * 5;
	public Vector3 velocity = Vector3.Zero;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		GetNode<Explosion>("explosion").explosionFinished += OnExplosionFinished;
	}

    private void OnExplosionFinished() => QueueFree();

    private void OnBodyEntered(Node3D body)
    {
		GetNode<Sprite3D>("projectile").Visible = false;
		SetPhysicsProcess(false);
		if(body is IDamageable damageable) {
			GD.Print("hit enemy!");
        	damageable.Damage(5);
    	}
		GetNode<Explosion>("explosion").Explode();
    }

    public override void _PhysicsProcess(double delta)
	{
		velocity += g * (float)delta;
		LookAt(Transform.Origin + velocity.Normalized(), Vector3.Up);
		var transform = Transform;
		transform.Origin = Transform.Origin + velocity * (float)delta;
		Transform = transform;
	}
}
