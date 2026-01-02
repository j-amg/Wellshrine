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
	public static string enemyScenePath = "res://enemies/enemy";
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
	[Export]
	public AudioStream attackSound;
	public double acceleration = 2;

	public int level = 1;
	private Vector3 velocity;
	private Color defaultModulate;
	private int currentMovementSpeed;
	
	public bool awake = false;
	public bool dead = false;
	public bool stunned = false;
	public bool damaged = false;
	public bool aggro = false;
	public bool inview;

    public Color ReticleModulate { get; set; }
    public float Health { get; set; }
    public bool Active { get; set; }
    public float HoverRange { get; set; }

    public override void _Ready()
	{
		baseHealth += level * 0.25f * baseHealth;
		damage += level * 0.25f * damage;
		baseMovementSpeed += level/10;
		Health = baseHealth;
		Active = true;
		HoverRange = 1000;
		label.SetValues();
		AddToGroup("enemies");
		ReticleModulate = new Color(1,0,0);
		defaultModulate = sprite.Modulate;
		FloorSnapLength = 1;
		awake = true;
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

	public static Enemy InitEnemy(PackedScene scene, int levelParam, Vector3 position)
	{
		Enemy enemy = scene.Instantiate<Enemy>();
		enemy.level = levelParam;
		enemy.Position = position;
		return enemy;
	}

    public void Damage(Damage d)
	{
		Health -= d.amount;
		if (Health <= 0) BeginDie();
		d.Hit();
		Stun();
		EmitSignal(SignalName.damageTaken, d);
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(sprite, "modulate", defaultModulate, .25).From(new Color(1,0,0,1));
		damaged = true;
	}

    public void BeginDie()
    {
		EmitSignal(SignalName.enemyDied, this);
		labelSprite.Visible = false;
		RemoveFromGroup("enemies");
		dead = true;
    }

	public void Die()
	{
        SlotData slotData = new() {itemData = GD.Load<ItemData>("res://inventory/items/gold.tres")};
        GroundItem GenItem = GroundItem.InitGroundItem(slotData, Position);
		GetTree().CurrentScene.CallDeferred("add_child", GenItem);
		CallDeferred("queue_free");
	}

	public override void _PhysicsProcess(double delta)
	{
		sprite.FlipH = velocity.X > 0;
		if (Global.Singleton.player != null) LookAt(Global.Singleton.player.GlobalPosition);
		inview = rc.GetCollider() is Player && !Global.Singleton.dead;
		ApplyFloorSnap();
		MoveAndSlide();
	}

    public void StartHover()
    {
        if (!dead) labelSprite.Visible = true; 
    }

    public void EndHover()
    {
        if (!damaged) labelSprite.Visible = false;
    }
}
