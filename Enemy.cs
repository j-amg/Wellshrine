using Godot;
using System;

public partial class Enemy : AnimatableBody3D, IDamageable
{
	// Called when the node enters the scene tree for the first time.

	[Signal]
	public delegate void damageTakenEventHandler();
	public int baseMovementSpeed = 100;
	private int currentMovementSpeed;
	public int baseHealth = 100;
	public int currentHealth = 100;
	public bool alive = true;
	public int level = 5;
	public string name = "Really cool enemy";
	public override void _Ready()
	{
		currentHealth = baseHealth;
	}

	void IDamageable.Damage(int amount)
	{
		GD.Print("damage!");
		currentHealth -= amount;
		EmitSignal(SignalName.damageTaken);
	}

	int IDamageable.Health{ get{ return baseHealth; } set{}}

	public override void _Process(double delta)
	{
		//GD.Print(currentHealth);
	}
}
