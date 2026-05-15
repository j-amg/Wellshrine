using Godot;
using System;

public partial class Dummy : Enemy
{

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
	}
	public override void Die()
	{
		SetHealth(attributeData.attributes[AttributeType.MaximumHealth].Value);
	}
}
