using Godot;
using System;

public partial class SlideState : State
{
    [Export]
    float speed = 10;
    [Export]
    float acceleration = 0.5f;
    [Export]
    float deceleration = 0.1f;
    [Export]
    float bodyCrouchHeight = -0.2f;
    float bodyStandHeight = 0;
    float s;

    public override void Enter()
    {
        s = player.hvel.Length() + 15;
		player.UpdateInput(player.hvel.Length() + 15, acceleration, deceleration);
		//player.velocity.X = player.direction.X * speed;
		//player.velocity.Z = player.direction.Z * speed;
		//player.crouchCollision.Disabled = false;
		//player.standCollision.Disabled = true;
        //Tween tween = GetTree().CreateTween();
        //tween.TweenProperty(player.body, "position", new Vector3(player.body.Position.X, bodyCrouchHeight, player.body.Position.Z), 0.5f);
        base.Enter();
    }

    public override void Exit()
    {
		//player.crouchCollision.Disabled = true;
		//player.standCollision.Disabled = false;
        //Tween tween = GetTree().CreateTween();
        //tween.TweenProperty(player.body, "position", new Vector3(player.body.Position.X, bodyStandHeight, player.body.Position.Z), 0.5f);
        base.Exit();
    }
    public override void Update(double delta)
	{
		//player.UpdateInput(s, acceleration, deceleration);
        //player.velocity.X = Mathf.Lerp(player.velocity.X, direction.X * speed, acceleration);
		//player.velocity.Z = Mathf.Lerp(player.velocity.Z, direction.Z * speed, acceleration);
        
		s -= .1f;
        player.UpdateVelocity();
		if (player.hvel.Length() <= 3) EmitSignal(SignalName.transition, "crouch");
		if (player.velocity != Vector3.Zero && Input.IsActionJustReleased("Shift")) EmitSignal(SignalName.transition, "walk");
		if (player.velocity != Vector3.Zero && Input.IsActionPressed("Alt")) EmitSignal(SignalName.transition, "sprint");
		if (Input.IsActionJustPressed("Space")) EmitSignal(SignalName.transition, "dash");
        if (!player.IsOnFloor()) EmitSignal(SignalName.transition, "fall");
	}
}
