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

	private Sprite3D sprite;
	public Vector3 velocity = Vector3.Zero;
	public float damage = 0;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		explosion.explosionFinished += OnExplosionFinished;
		AreaEntered += OnAreaEntered;
		sprite = GetNode<Sprite3D>("projectile");
		Setup();
		
	}

	private async void Setup()
	{
		await ToSignal(GetTree().CreateTimer(.1), "timeout");
		sprite.Visible = true;
		//SetCollisionMaskValue(2, true);
	}

    private void OnAreaEntered(Area3D area)
    {
    	if (area is Projectile proj)
		{
			proj.Destroy();
			Hit(proj);
		}
    }

    private void OnExplosionFinished() => CallDeferred("queue_free");

	public void Destroy() => CallDeferred("queue_free");

	public void Hit(Node3D target)
	{
		sprite.Visible = false;
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
			CallDeferred("queue_free");
		}
	}

    private void OnBodyEntered(Node3D body) => Hit(body);

    public override void _PhysicsProcess(double delta)
	{
		if (velocity.Length() == 0) return;
		velocity += (Vector3.Down * gravity) * (float)delta;
		LookAt(Transform.Origin + velocity.Normalized(), Vector3.Up);
		var transform = Transform;
		transform.Origin = Transform.Origin + velocity * (float)delta;
		Transform = transform;
	}
}
