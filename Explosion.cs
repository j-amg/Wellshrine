using Godot;

public partial class Explosion : Area3D
{
	[Signal]
	public delegate void explosionFinishedEventHandler();
	private float explosionDur = .5f;
	private AnimatedSprite3D sprite;
	Godot.Collections.Array<Node3D> bodies;

    public override void _Ready()
    {
        sprite = GetNode<AnimatedSprite3D>("sprite");
    }
    public void SetRadius(float scale)
	{
		Scale = new Vector3(scale, scale, scale);
	}

	public async void Explode(float damage, float delay)
	{
		await ToSignal(GetTree().CreateTimer(delay), "timeout");
		foreach(Node3D node in bodies)
		{
    		if(node is IDamageable damageable) damageable.Damage(damage);
		}
		Tween tween = GetTree().CreateTween();
		sprite.Visible = true;
		sprite.Play("explode");

		tween.TweenProperty(sprite, "modulate", new Color(0,0,0,0), explosionDur);
		await ToSignal(tween, Tween.SignalName.Finished);
		EmitSignal(SignalName.explosionFinished);
	}

    public override void _PhysicsProcess(double delta)
	{
		bodies = GetOverlappingBodies();
	}
}
