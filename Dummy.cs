using Godot;
using System;

public partial class Dummy : Enemy
{

	public override void _PhysicsProcess(double delta)
	{
		return;
	}
	public override void Die()
	{
		SetHealth(attributeData.Attributes[AttributeType.MaxHealth].Value);
	}
}
