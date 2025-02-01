using Godot;
using System;

public partial interface IDamageable
{
	float Health { get; set; }
	void Damage(float amount);
}
