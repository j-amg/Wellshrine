using Godot;
using System;
public partial class IdleState : State
{
    [Export]
    float speed = 5;
    [Export]
    float acceleration = 0.1f;
    [Export]
    float deceleration = 0.25f;
	public override void Update(double delta)
	{
        player.UpdateGravity(delta);
        player.UpdateInput(speed, acceleration, deceleration);
        player.UpdateVelocity();
		if (player.Velocity.Length() > 0.0 && player.IsOnFloor()) EmitSignal(SignalName.transition, "walkState");
        if(!player.IsOnFloor()) EmitSignal(SignalName.transition, "fallState");
        if (Input.IsActionJustPressed("Space"))
        {
            GD.Print("should be jumping");
            EmitSignal(SignalName.transition, "jumpState");
        } 
	} 

}
