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
	public bool alive = true;
	public int level = 5;
	public string name = "Really cool enemy";
	[Export]
	public NavigationAgent3D nav;
	private Vector3 velocity;

	public override void _Ready()
	{
		SetPhysicsProcess(false);
		CallDeferred("Setup");
		currentHealth = baseHealth;
		damageTaken += OnDamageTaken;
	}

	private async void Setup()
	{
		await ToSignal(GetTree(), "physics_frame");
		SetPhysicsProcess(true);
	}
    void IDamageable.Damage(int amount)
	{
		GD.Print("damage!");
		currentHealth -= amount;
		EmitSignal(SignalName.damageTaken);
	}

	private void OnDamageTaken()
    {
        if (currentHealth <= 0) Die();
    }

	public void Die() => QueueFree();

	int IDamageable.Health{ get{ return baseHealth; } set{}}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 direction = new();
		velocity = Velocity;
		nav.TargetPosition = Global.Singleton.player.GlobalPosition;
		direction = nav.GetNextPathPosition() - GlobalPosition;
		direction = direction.Normalized();
		velocity = velocity.Lerp(direction * baseMovementSpeed, (float)(acceleration * delta));
		Velocity = velocity;
		MoveAndSlide();
	}
}
