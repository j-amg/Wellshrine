using System;
using Godot;

public partial class Enemy : CharacterBody3D, IDamageable, IHoverable
{
	[Signal]
	public delegate void damageTakenEventHandler(Damage damageTaken);
	[Signal]
	public delegate void enemyDiedEventHandler(Enemy enemy);
	[Export]
	public NavigationAgent3D nav;
	[Export]
	public StateMachine sm;
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
	public AudioStream walkSound;
	[Export]
	public AudioStreamPlayer3D audioPlayer;
	[Export]
	public float detectionRange = 250;
	[Export]
	public float attackRange = 2;
	[Export]
	public float attackDuration = 1;
	[Export]
	public float attackWindup = .5f;
	[Export]
	public float damage = 5;
	public double acceleration = 2;

	public int level = 1;
	private Vector3 velocity;
	private Color defaultModulate;
	private int currentMovementSpeed;
	
	public bool awake = false;
	public bool dead = false;
	public bool stunned = false;
	public bool highlighted = false;
	public bool damaged = false;
	public bool aggro = false;
	public bool inview;

    public Color ReticleModulate { get; set; }
    public float Health { get; set; }
    public bool Highlighted { get; set; }
    public bool Active { get; set; }

    public override void _Ready()
	{
		baseHealth += level * 0.25f * baseHealth;
		damage += level * 0.25f * damage;
		baseMovementSpeed += level/10;
		Health = baseHealth;
		Active = true;
		label.SetValues();
		AddToGroup("enemies");
		ReticleModulate = new Color(1,0,0);
		defaultModulate = sprite.Modulate;
		FloorSnapLength = 1;
		SpawnDelay();
	}

	public async void SpawnDelay()
    {
		await ToSignal(GetTree().CreateTimer(1), "timeout");
        awake = true;
    }

	private async void Stun()
	{
		if (dead) return;
		if (sm.current_state.Name != "attack" && sprite.Animation != "spawn") sprite.Play("stun");
		SetPhysicsProcess(false);
		stunned = true;
		await ToSignal(GetTree().CreateTimer(Global.Singleton.equippedWeapon.stunDuration + Global.Singleton.GetPlayerModifier("stun")), "timeout");
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

    public void Damage(Damage d)
	{
		Health -= d.amount;
		if (Health <= 0) Die();
		d.Hit();
		Stun();
		EmitSignal(SignalName.damageTaken, d);
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(sprite, "modulate", defaultModulate, .25).From(new Color(1,0,0,1));
		damaged = true;
	}

    public void Die()
    {
		EmitSignal(SignalName.enemyDied, this);
		labelSprite.Visible = false;
		RemoveFromGroup("enemies");
		dead = true;
    }

	public override void _PhysicsProcess(double delta)
	{
		sprite.FlipH = velocity.X > 0;
		labelSprite.Visible = (highlighted || damaged) && !dead;
		if (Global.Singleton.player != null) LookAt(Global.Singleton.player.GlobalPosition);
		inview = rc.GetCollider() is Player && !Global.Singleton.dead;
		ApplyFloorSnap();
		MoveAndSlide();
	}
}
