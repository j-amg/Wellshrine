using Godot;
using System;

public partial class WalkState : State
{
    [Export]
    float speed = 5;
    [Export]
    float acceleration = .5f;
    [Export]
    float deceleration = 0.25f;
	public override void Update(double delta)
	{
        player.UpdateInput(speed + Global.Singleton.playerMoveSpeedBuff, acceleration, deceleration);
        player.UpdateVelocity();
		if (Global.Singleton.player.Velocity.Length() == 0.0) EmitSignal(SignalName.transition, "idle");
        if (Input.IsActionPressed("Shift")) EmitSignal(SignalName.transition, "slide");
        if (Input.IsActionJustPressed("Space")) EmitSignal(SignalName.transition, "jump");
        if(!player.IsOnFloor()) EmitSignal(SignalName.transition, "fall");
	}
}
