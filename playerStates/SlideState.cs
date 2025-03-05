using Godot;
using System;

public partial class SlideState : State
{
    [Export]
    float speed = 20;
    [Export]
    float acceleration = 0.5f;
    [Export]
    float deceleration = 0.2f;

    public override void Enter()
    {
        speed = 25;
        player.crouchCollision.Disabled = false;
		player.standCollision.Disabled = true;
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(player.body, "position", new Vector3(player.body.Position.X, player.bodyCrouchHeight, player.body.Position.Z), player.crouchSpeed).SetTrans(Tween.TransitionType.Sine);
        //s = player.hvel.Length() + 15;
        player.handSprite.Offset = new Vector2(0, -10);
		//player.UpdateInput(player.hvel.Length() + 15, acceleration, deceleration);
		//player.velocity.X = player.direction.X * speed;
		//player.velocity.Z = player.direction.Z * speed;
        base.Enter();
    }

    public override void Exit()
    {
        player.handSprite.Offset = new Vector2(0, 0);
        player.crouchCollision.Disabled = true;
		player.standCollision.Disabled = false;
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(player.body, "position", new Vector3(player.body.Position.X, player.bodyStandHeight, player.body.Position.Z), player.crouchSpeed).SetTrans(Tween.TransitionType.Sine);
        base.Exit();
    }
    public override void Update(double delta)
	{
		//player.UpdateInput(s, acceleration, deceleration);
        //player.velocity.X = Mathf.Lerp(player.velocity.X, direction.X * speed, acceleration);
		//player.velocity.Z = Mathf.Lerp(player.velocity.Z, direction.Z * speed, acceleration);
        
		speed -= .75f;
        player.UpdateInput(speed, acceleration, deceleration);
        player.UpdateVelocity();
		if (speed <= 3) EmitSignal(SignalName.transition, "crouch");
		if (player.velocity != Vector3.Zero && Input.IsActionJustReleased("Shift")) EmitSignal(SignalName.transition, "walk");
		if (player.velocity != Vector3.Zero && Input.IsActionPressed("Alt")) EmitSignal(SignalName.transition, "sprint");
		if (Input.IsActionJustPressed("Space")) EmitSignal(SignalName.transition, "dash");
        if (!player.IsOnFloor()) EmitSignal(SignalName.transition, "fall");
	}
}
