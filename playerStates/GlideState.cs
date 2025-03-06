using Godot;
using System;

public partial class GlideState : State
{
	[Export]
    float speed = 2;
	private float s;
    [Export]
    float acceleration = .1f;
    [Export]
    float deceleration = 0.005f;
	[Export]
	float input_multiplier = 0.9f;
	[Signal]
	public delegate void landedEventHandler();

    public override void Enter()
    {
		player.handSprite.Play("glide");
		player.handSprite.Play("glide");
		s = speed;
		if (player.hvel.Length() > speed) s = player.hvel.Length();
        base.Enter();
    }

    public override void Exit()
    {
		player.handSprite.Play(Global.Singleton.currentIdle);
		player.handSprite.Play(Global.Singleton.currentIdle);
        base.Exit();
    }
    public override void Update(double delta)
	{

        player.UpdateInput(s, acceleration, deceleration);
		if (player.Velocity.Y > 0) player.velocity.Y = Mathf.MoveToward(player.velocity.Y, 0, 0.1f);
		player.velocity.Y = Mathf.MoveToward(player.velocity.Y, -3, 0.01f);
        player.UpdateVelocity();
		if (Input.IsActionJustReleased("RightMouse")) EmitSignal(SignalName.transition, "fall");
		if (player.IsOnFloor())
		{
			EmitSignal(SignalName.landed);
			if (Input.IsActionPressed("Shift"))
			{
				EmitSignal(SignalName.transition, "slide");
			}
			else
			{
				EmitSignal(SignalName.transition, "idle");
			}
		}
	} 
}
