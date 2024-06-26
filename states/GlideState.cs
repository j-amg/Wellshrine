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
		player.leftHand.Play("aim");
		player.rightHand.Play("aim");
		s = speed;
		if (player.hvel.Length() > speed) s = player.hvel.Length();
        base.Enter();
    }

    public override void Exit()
    {
		player.leftHand.Play("idle");
		player.rightHand.Play("idle");
        base.Exit();
    }
    public override void Update(double delta)
	{

        player.UpdateInput(s, acceleration, deceleration);
		if (player.Velocity.Y > 0) player.velocity.Y = Mathf.MoveToward(player.velocity.Y, 0, 0.1f);
		player.velocity.Y = Mathf.MoveToward(player.velocity.Y, -3, 0.01f);
        player.UpdateVelocity();
		if (Input.IsActionJustReleased("RightMouse")) EmitSignal(SignalName.transition, "fallState");
		if (player.IsOnFloor())
		{
			EmitSignal(SignalName.landed);
			EmitSignal(SignalName.transition, "idleState");
		}
	} 
}
