using Godot;
using System;

public partial class Enemy : CharacterBody3D, IDamageable
{
	[Signal]
	public delegate void damageTakenEventHandler(float damageTaken);
	[Export]
	public NavigationAgent3D nav;
	[Export]
	public StateMachine sm;
	[Export]
	public AudioStream hit;
	[Export]
	public AnimatedSprite3D sprite;
	[Export]
	public Sprite3D labelSprite;
	[Export]
	public EnemyLabel label;
	[Export]
	public RayCast3D rc;
	[Export]
	public float baseMovementSpeed = 2;
	[Export]
	public string name = "[PH] Enemy";
	[Export]
	public float baseHealth;
	[Export]
	public const string enemyScenePath = "res://enemies/enemy";
	[Export]
	public float detectionRange = 200;
	[Export]
	public float attackRange = 2;
	[Export]
	public float attackDuration = 1;
	[Export]
	public float attackWindup = .5f;
	[Export]
	public float attackDamage = 5;
	public double acceleration = 2;

	public int level = 1;
	private Vector3 velocity;
	private Color defaultModulate;
	private int currentMovementSpeed;
	public float currentHealth;
	
	public bool awake = false;
	public bool dead = false;
	public bool stunned = false;
	public bool inview;
	public bool highlighted = false;
	public bool damaged = false;
	public bool aggro = false;
	

	public override void _Ready()
	{
		SetPhysicsProcess(false);
		baseHealth += level * 0.25f * baseHealth;
		attackDamage += level * 0.25f * attackDamage;
		baseMovementSpeed += level/10;
		currentHealth = baseHealth;
		damageTaken += OnDamageTaken;
		label.SetValues();
		AddToGroup("enemies");

		CallDeferred("Setup");
		Global.Singleton.UpdateHUD();
		defaultModulate = sprite.Modulate;
		FloorSnapLength = 1;
		SpawnDelay();
	}

	public async void SpawnDelay()
    {
		await ToSignal(GetTree().CreateTimer(1), "timeout");
        awake = true;
    }

	private async void Setup()
	{
		// need to do this to wait for the navigation thingie to sync
		await ToSignal(GetTree(), "physics_frame");
		SetPhysicsProcess(true);
		sprite.Visible = true;
		
	}

	private async void Stun()
	{
		if (dead) return;
		if (sm.current_state.Name != "attack" && sprite.Animation != "spawn") sprite.Play("stun");
		SetPhysicsProcess(false);
		stunned = true;
		await ToSignal(GetTree().CreateTimer(Global.Singleton.equippedWeapon.stunDuration), "timeout");
		stunned = false;
		SetPhysicsProcess(true);
	}

	public static Enemy InitEnemy(PackedScene scene, int levelParam, Transform3D transformParam)
	{
		Enemy enemy = scene.Instantiate<Enemy>();
		enemy.level = levelParam;
		enemy.GlobalTransform = transformParam;
		return enemy;
	}

    void IDamageable.Damage(float amount)
	{
		Stun();
		Tween tween = GetTree().CreateTween();
		Global.Singleton.PlaySound3D(GlobalPosition, hit);
		tween.TweenProperty(sprite, "modulate", defaultModulate, .25).From(new Color(1,0,0,1));
		currentHealth -= amount;
		EmitSignal(SignalName.damageTaken, amount);
	}

	private void OnDamageTaken(float damage)
    {
		damaged = true;
		Global.Singleton.hud.FlashCrossHair();
		//Global.Singleton.PlaySound2D(hit);
        if (currentHealth <= 0) Die();
    }

    public void Die()
    {
		labelSprite.Visible = false;
		RemoveFromGroup("enemies");
		if (Global.Singleton.CurrentScene is Zone zone) zone.UpdateObjective();
		Global.Singleton.UpdateHUD();
		dead = true;
    }

    float IDamageable.Health{ get{ return baseHealth; } set{}}

	public override void _PhysicsProcess(double delta)
	{
		sprite.FlipH = velocity.X > 0;
		labelSprite.Visible = (highlighted || damaged) && !dead;
		if (Global.Singleton.player != null) LookAt(Global.Singleton.player.GlobalPosition);
		inview = rc.GetCollider() is Player;
		ApplyFloorSnap();
		MoveAndSlide();
	}
}
