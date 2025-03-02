using Godot;
using System;

public partial class Enemy : CharacterBody3D, IDamageable
{
	[Signal]
	public delegate void damageTakenEventHandler();
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
	public int level = 1;
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
	private Vector3 velocity;
	public bool highlighted = false;
	public bool damaged = false;
	public bool aggro = false;
	private Sprite3D sprite;
	public RayCast3D rc;
	private Color defaultModulate;
	public bool inview;
	public StateMachine sm;
	

	public override void _Ready()
	{
		SetPhysicsProcess(false);
		baseHealth += level * 0.25f * baseHealth;
		attackDamage += level * 0.25f * attackDamage;
		baseMovementSpeed += level/10;
		currentHealth = baseHealth;
		damageTaken += OnDamageTaken;
		AddToGroup("enemies");
		GetNode<EnemyLabel>("SubViewport/label").SetValues();
		sprite = GetNode<Sprite3D>("sprite");
		rc = GetNode<RayCast3D>("rc");
		sm = GetNode<StateMachine>("sm");
		CallDeferred("Setup");
		Global.Singleton.UpdateHUD();
		defaultModulate = sprite.Modulate;	
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
		SetPhysicsProcess(false);
		await ToSignal(GetTree().CreateTimer(Global.Singleton.equippedWeapon.stunDuration), "timeout");
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
		tween.TweenProperty(sprite, "modulate", defaultModulate, .25).From(new Color(1,0,0,1));
		currentHealth -= amount;
		EmitSignal(SignalName.damageTaken);
	}

	private void OnDamageTaken()
    {
		damaged = true;
        if (currentHealth <= 0) Die();
    }

    public void Die()
    {
		RemoveFromGroup("enemies");
		if (Global.Singleton.CurrentScene is Zone zone)
		{
			GD.Print("try update");
			zone.UpdateObjective();
		} 
		Global.Singleton.UpdateHUD();
		if (sm.current_state is ShooterAttack s && s.castProj != null) s.castProj.Destroy();  
        CallDeferred("queue_free");
    }

    float IDamageable.Health{ get{ return baseHealth; } set{}}

	public override void _PhysicsProcess(double delta)
	{
		//if (Global.Singleton.toggled) return;
		GetNode<Sprite3D>("sprite").FlipH = velocity.X > 0;
		GetNode<Sprite3D>("labelSprite").Visible = highlighted || damaged;
		if (Global.Singleton.player != null) LookAt(Global.Singleton.player.GlobalPosition);
		inview = rc.GetCollider() is Player;
		MoveAndSlide();
	}
}
