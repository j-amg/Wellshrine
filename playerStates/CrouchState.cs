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
    float crouchSpeed = 0.1f;
    [Export]
    float bodyCrouchHeight = -0.2f;
    float bodyStandHeight = 0;

    public override void Enter()
    {
		player.crouchCollision.Disabled = false;
		player.standCollision.Disabled = true;
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(player.body, "position", new Vector3(player.body.Position.X, bodyCrouchHeight, player.body.Position.Z), crouchSpeed).SetTrans(Tween.TransitionType.Sine);
        base.Enter();
    }

    public override void Exit()
    {
		player.crouchCollision.Disabled = true;
		player.standCollision.Disabled = false;
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(player.body, "position", new Vector3(player.body.Position.X, bodyStandHeight, player.body.Position.Z), crouchSpeed).SetTrans(Tween.TransitionType.Sine);
        base.Exit();
    }
    public override void Update(double delta)
	{
        player.UpdateInput(speed, acceleration, deceleration);
        player.UpdateVelocity();
		if (player.velocity == Vector3.Zero && Input.IsActionJustReleased("Shift")) EmitSignal(SignalName.transition, "idle");
		if (player.velocity != Vector3.Zero && Input.IsActionJustReleased("Shift")) EmitSignal(SignalName.transition, "walk");
		if (Input.IsActionJustPressed("Space")) EmitSignal(SignalName.transition, "dash");
        if (!player.IsOnFloor()) EmitSignal(SignalName.transition, "fall");
	}
}
