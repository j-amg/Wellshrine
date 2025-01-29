using Godot;
using System;

public partial class Enemy : CharacterBody3D, IDamageable
{
	[Export]
	public float baseMovementSpeed = 2;
	[Export]
	public string name = "[PH] Enemy";
	public double acceleration = 10;
	private int currentMovementSpeed;
	[Export]
	public float baseHealth;
	[Export]
	public const string enemyScenePath = "res://enemies/enemy";
	[Export]
	public float detectionRange = 200;
	[Export]
	public float detectionFalloffRange = 800;
	public float currentHealth;
	public int level = 2;
	[Export]
	public NavigationAgent3D nav;
	[Export]
	public float attackRange = 2;
	[Export]
	public float attackDuration = 1;
	[Export]
	public float attackWindup = .5f;
	[Export]
	public float attackDamage = 5;
	[Signal]
	public delegate void damageTakenEventHandler();

	private Vector3 velocity;
	public bool highlighted = false;
	public bool damaged = false;
	public bool aggro = false;
	

	public override void _Ready()
	{
		SetPhysicsProcess(false);
		baseHealth += level * 0.25f * baseHealth;
		attackDamage += level * 0.25f * attackDamage;
		currentHealth = baseHealth;
		damageTaken += OnDamageTaken;

		GetNode<SubViewport>("SubViewport").GetNode<EnemyLabel>("label").SetValues();
		CallDeferred("Setup");
	}

	private async void Setup()
	{
		// need to do this to wait for the navigation thingie to sync
		await ToSignal(GetTree(), "physics_frame");
		SetPhysicsProcess(true);
	}

	public static Enemy InitEnemy(PackedScene scene, int levelParam, Transform3D transformParam)
	{
		Enemy enemy = scene.Instantiate<Enemy>();
		enemy.level = levelParam;
		enemy.GlobalTransform = transformParam;
		return enemy;
	}

    void IDamageable.Damage(int amount)
	{
		currentHealth -= amount;
		EmitSignal(SignalName.damageTaken);
	}

	private void OnDamageTaken()
    {
		damaged = true;
        if (currentHealth <= 0) Die();
    }

	public void Die() => QueueFree();

	float IDamageable.Health{ get{ return baseHealth; } set{}}

	public override void _PhysicsProcess(double delta)
	{
		//if (Global.Singleton.toggled) return;
		GetNode<Sprite3D>("sprite").FlipH = velocity.X > 0;
		GetNode<Sprite3D>("labelSprite").Visible = highlighted || damaged;
		MoveAndSlide();
	}
}
