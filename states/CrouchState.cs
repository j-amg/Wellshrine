using Godot;
using System;

public partial class CrouchState : State
{
    [Export]
    float speed = 3;
    [Export]
    float acceleration = .25f;
    [Export]
    float deceleration = 0.25f;
	[Export]
	float crouchOffset = -0.2f;

    public override void Enter()
    {
		player.crouchCollision.Disabled = false;
		player.standCollision.Disabled = true;
		player.body.Position = player.body.Position.Lerp(new Vector3(player.body.Position.X, player.body.Position.Y + crouchOffset, player.body.Position.Z), 1f);
        base.Enter();
    }

    public override void Exit()
    {
		player.crouchCollision.Disabled = true;
		player.standCollision.Disabled = false;
		player.body.Position = player.body.Position.Lerp(new Vector3(player.body.Position.X, player.body.Position.Y - crouchOffset, player.body.Position.Z), 1f);
        base.Exit();
    }
    public override void Update(double delta)
	{

        player.UpdateInput(speed, acceleration, deceleration);
        player.UpdateVelocity();
		if (player.velocity == Vector3.Zero && Input.IsActionJustReleased("Shift")) EmitSignal(SignalName.transition, "idleState");
		if (player.velocity != Vector3.Zero && Input.IsActionJustReleased("Shift")) EmitSignal(SignalName.transition, "walkState");
		if (Input.IsActionJustPressed("Space")) EmitSignal(SignalName.transition, "dashState");
        if(!player.IsOnFloor()) EmitSignal(SignalName.transition, "fallState");
	}
}
