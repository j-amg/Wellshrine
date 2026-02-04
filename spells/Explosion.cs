using Godot;

public partial class Explosion : Area3D
{
	[Signal]
	public delegate void explosionFinishedEventHandler();
	[Export]
	private float explosionDur = .5f;
	[Export]
	private AnimatedSprite3D sprite;
    public void SetRadius(float scale) => Scale = new Vector3(scale, scale, scale);

	public async void Explode(Damage damage, float delay)
	{
		await ToSignal(GetTree().CreateTimer(delay), "timeout");
		foreach(Node3D node in GetOverlappingBodies()) if(node is IDamageable damageable) damageable.Damage(damage);
		Tween tween = GetTree().CreateTween();
		sprite.Visible = true;
		sprite.Play("explode");

		tween.TweenProperty(sprite, "modulate", new Color(0,0,0,0), explosionDur);
		await ToSignal(tween, Tween.SignalName.Finished);
		EmitSignal(SignalName.explosionFinished);
	}
}
