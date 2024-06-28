using Godot;
using System;
public partial class IdleState : State
{
    [Export]
    float speed = 5;
    [Export]
    float acceleration = .25f;
    [Export]
    float deceleration = 0.25f;
	public override void Update(double delta)
	{
        player.UpdateInput(speed, acceleration, deceleration);
        player.UpdateVelocity();
;	if (player.Velocity.Length() > 0.0 && player.IsOnFloor()) EmitSignal(SignalName.transition, "walk");
        if (!player.IsOnFloor()) EmitSignal(SignalName.transition, "fall");
        if (Input.IsActionPressed("Shift")) EmitSignal(SignalName.transition, "crouch");
        if (Input.IsActionJustPressed("Space")) EmitSignal(SignalName.transition, "jump");
	} 

}
