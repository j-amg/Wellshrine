using Godot;
using System;
using System.Linq;

public partial class SpellScene : Node3D
{
	[Export] public SpellType spellType;

	[Export] public float muzzleVelocity = 0;
	[Export] public float gravity = 0;
	[Export] public int projectileCount = 1;
	[Export] public float projectileSpread = 15f;



	[Export] private Node3D sprites;
	[Export] private Area3D hurtBox;
	[Export] public float spriteLifeTime = .5f;

	[Export] public bool destroyOnEntityHit = false;
	[Export] public bool destroyOnSceneHit = false;

	[Export] public bool spawnOnEntityHit = false;
	[Export] public bool spawnOnSceneHit = false;
	[Export] public float spawnOnHitDelay = 0;
	[Export] public PackedScene spawnOnHitScene;
	[Export] public bool appliesCondition = false;

	private Vector3 velocity = Vector3.Zero;

	//[Export] public Condition[];
	public override void _Ready()
	{
		if (hurtBox != null)
		{
			hurtBox.AreaEntered += OnAreaEntered;
			hurtBox.BodyEntered += OnBodyEntered;
		}

		if (spellType is SpellType.Projectile)
		{
			velocity = -Transform.Basis.Z * muzzleVelocity;
		}

		Fade();
	}

	private void OnBodyEntered(Node3D body)
	{
		//if (body is Player) return;
		if (spawnOnSceneHit)
		{
			SpawnScene();
		}
		if (destroyOnSceneHit)
		{
			CallDeferred("queue_free");
		}
		//Hit()
	}

	private void OnAreaEntered(Area3D area)
	{
		// if (area is HurtBox)
		// {
		// 	if (spawnOnEntityHit)
		// 	{
		// 		SpawnScene();
		// 	}
		// 	if (destroyOnEntityHit)
		// 	{
		// 		//CallDeferred("queue_free");
		// 	}
		// }
	}

	public async void SpawnScene()
	{
		await ToSignal(GetTree().CreateTimer(spawnOnHitDelay), "timeout");
		Global.Singleton.AddToScene(spawnOnHitScene.Instantiate<SpellScene>(), Transform);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (spellType is SpellType.Projectile)
		{
			//if (velocity.Length() == 0) return;
			velocity += Vector3.Down * gravity * (float)delta;
			//LookAt(Transform.Origin + velocity.Normalized(), Vector3.Up);
			var transform = Transform;
			transform.Origin = Transform.Origin + velocity * (float)delta;
			Transform = transform;
		}

	}


	private async void Fade()
	{
		await ToSignal(GetTree().CreateTimer(spriteLifeTime), "timeout");
		foreach (Sprite3D s in sprites.GetChildren().Cast<Sprite3D>())
		{
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(s, "modulate", new Color(0, 0, 0, 0), 1f);
		}
		await ToSignal(GetTree().CreateTimer(1f), "timeout");
		CallDeferred("queue_free");

	}
}
