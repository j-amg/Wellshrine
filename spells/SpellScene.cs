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

	[Export] public bool spawnOnEntityImpact = false;
	[Export] public bool spawnOnSceneImpact = false;
	[Export] public float spawnOnImpactDelay = 0;
	[Export] public PackedScene spawnOnImpactScene;

	[Export] public bool appliesCondition = false;

	private Vector3 velocity = Vector3.Zero;

	//[Export] public Condition[];
	public override void _Ready()
	{
		if (spellType is SpellType.Projectile)
		{
			velocity = -Transform.Basis.Z * muzzleVelocity;
		}
		
		Fade();
	}

	// public void InitSpellScene(SpellScene scene, Transform3D transform, Node owner)
	// {
	// 	//SpellScene spellScene = new();
	// 	// if (spellType is SpellType.Projectile)
	// 	// {
	// 	// 	for (int i = 0; i < projectileCount; i++)
	// 	// 	{
	// 	// 		float rotationOffset = (i * projectileSpread) - (projectileCount - 1 * projectileSpread)/2; 
				
	// 	// 		{
	// 	// 			spellScene.Transform = transform.Rotated(Vector3.Up, Mathf.DegToRad(rotationOffset))
	//    	// 		};
				
	// 	// 	}
	// 	// }

	// }

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
		foreach(Sprite3D s in sprites.GetChildren().Cast<Sprite3D>())
		{
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(s, "modulate", new Color(0,0,0,0), 1f);
		}
		await ToSignal(GetTree().CreateTimer(1f), "timeout");
		CallDeferred("queue_free");

	}
}
