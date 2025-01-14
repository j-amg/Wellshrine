using Godot;
using System;
using System.Diagnostics;

public partial class Explosion : Area3D
{
	[Signal]
	public delegate void explosionFinishedEventHandler();
	private float explosionDur = .5f;
	private float explosionRadius = 1f;
	[Export]
	private int damage;
	public override void _Ready()
	{
		Scale = new Vector3(explosionRadius, explosionRadius, explosionRadius);
	}

    public void Explode()
    {
		ExplodeAsync();
		foreach(Node3D node in GetOverlappingBodies())
		{
    		if(node is IDamageable damageable) damageable.Damage(damage);
		}	
    }

	private async void ExplodeAsync()
	{
		Tween tween = GetTree().CreateTween();
		GetNode<Sprite3D>("sprite").Visible = true;
		tween.TweenProperty(GetNode<Sprite3D>("sprite"), "modulate", new Color(0,0,0,0), explosionDur);
		await ToSignal(tween, Tween.SignalName.Finished);
		EmitSignal(SignalName.explosionFinished);
	}
}
