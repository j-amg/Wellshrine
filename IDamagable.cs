using Godot;
using System;

public partial interface IDamageable
{
	int Health { get; set; }
	void Damage(int amount);
}
