using Godot;
using System;

public partial class FallState : State
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
		s = speed;
		if (player.hvel.Length() > speed) s = player.hvel.Length();
        base.Enter();
    }
    public override void Update(double delta)
	{
		//speed = player.velocity.Length();
        player.UpdateInput(s, acceleration, deceleration);
		player.velocity.Y -= player.gravity * (float)delta;
        player.UpdateVelocity();
		if (Input.IsActionPressed("RightMouse"))
		{
			EmitSignal(SignalName.transition, "glide");
		}
		
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
