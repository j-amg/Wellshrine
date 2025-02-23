using Godot;
using System;
using System.Diagnostics;

public partial class Projectile : Area3D
{
	[Export]
	public float muzzleVelocity = 25;
	[Export]
	public bool explode = false;
	[Export]
	public float explosionRadius = 0;
	[Export]
	public float explosionDelay = 0;
	[Export]
	public float gravity = 0;
	[Export]
	public Explosion explosion;
	public Vector3 velocity = Vector3.Zero;
	public float damage = 0;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		explosion.explosionFinished += OnExplosionFinished;
		AreaEntered += OnAreaEntered;
		//GetNode<Sprite3D>("Projectile").Visible = true;
		
	}

    private void OnAreaEntered(Area3D area)
    {
    	if (area is Projectile proj)
		{
			Hit(proj);
			proj.Hit(this);
		} 
    }

    private void OnExplosionFinished() => QueueFree();

	public void Hit(Node3D target)
	{
		GetNode<Sprite3D>("projectile").Visible = false;
		SetPhysicsProcess(false);
		if(target is IDamageable damageable && target != this) {
			GD.Print("hit enemy!");
        	damageable.Damage(damage);
    	}

		if (explode)
		{
			explosion.SetRadius(explosionRadius);
			explosion.Explode(damage, explosionDelay);
		} else
		{
			QueueFree();
		}
	}

    private void OnBodyEntered(Node3D body) => Hit(body);

    public override void _PhysicsProcess(double delta)
	{
		velocity += (Vector3.Down * gravity) * (float)delta;
		LookAt(Transform.Origin + velocity.Normalized(), Vector3.Up);
		var transform = Transform;
		transform.Origin = Transform.Origin + velocity * (float)delta;
		Transform = transform;
	}
}
