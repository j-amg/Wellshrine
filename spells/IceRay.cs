using Godot;
using System;

public partial class IceRay : Node3D
{

	private Sprite3D sprite;
	public override void _Ready()
	{
		Fade();
	}


	private async void Fade()
	{
		Sprite3D raySprite = GetNode<Sprite3D>("ray");
		Sprite3D frontSprite = GetNode<Sprite3D>("frontRay");
		Tween frontTween = GetTree().CreateTween();
		Tween sideTween = GetTree().CreateTween();
		sideTween.TweenProperty(raySprite, "modulate", new Color(0,0,0,0), 1f);
		frontTween.TweenProperty(frontSprite, "modulate", new Color(0,0,0,0), 1f);
		await ToSignal(frontTween, Tween.SignalName.Finished);
		CallDeferred("queue_free");
	}
}
