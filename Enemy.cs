using Godot;
using System;

public partial class Enemy : CharacterBody3D, IDamageable
{
	// Called when the node enters the scene tree for the first time.

	[Signal]
	public delegate void damageTakenEventHandler();
	public float baseMovementSpeed = 2;
	public double acceleration = 10;
	private int currentMovementSpeed;
	public int baseHealth = 100;
	public int currentHealth = 100;
	public int level = 1;
	public string name = "Really cool enemy";
	[Export]
	public NavigationAgent3D nav;
	private Vector3 velocity;
	public bool highlighted = false;
	public bool damaged = false;

	public override void _Ready()
	{
		SetPhysicsProcess(false);
		CallDeferred("Setup");
		currentHealth = baseHealth;
		damageTaken += OnDamageTaken;
	}

	private async void Setup()
	{
		// need to do this to wait for the navigation thingie to sync
		await ToSignal(GetTree(), "physics_frame");
		SetPhysicsProcess(true);
	}
    void IDamageable.Damage(int amount)
	{
		//GD.Print("damage!");
		currentHealth -= amount;
		EmitSignal(SignalName.damageTaken);
	}

	private void OnDamageTaken()
    {
		damaged = true;
        if (currentHealth <= 0) Die();
    }

	public void Die() => QueueFree();

	int IDamageable.Health{ get{ return baseHealth; } set{}}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 direction;
		velocity = Velocity;
		nav.TargetPosition = Global.Singleton.player.GlobalPosition;
		direction = nav.GetNextPathPosition() - GlobalPosition;
		direction = direction.Normalized();
		velocity = velocity.Lerp(direction * baseMovementSpeed, (float)(acceleration * delta));
		Velocity = velocity;

		GetNode<Sprite3D>("sprite").FlipH = velocity.X > 0;
		GetNode<Sprite3D>("labelSprite").Visible = highlighted || damaged;
		
		MoveAndSlide();
	}
}
