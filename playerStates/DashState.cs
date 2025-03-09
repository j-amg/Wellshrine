using Godot;
using System;

public partial class DashState : State
{
    [Export]
    float speed = 5;
    [Export]
    float acceleration = 1f;
    [Export]
    float deceleration = 0.01f;
	[Export]
	float dashVelocity = 15f;

    public override void Enter()
    {
        base.Enter();
        if (Global.Singleton.awaitedAction == "dash") Global.Singleton.ClosePopUp();
    }
    public override void Update(double delta)
	{
        player.UpdateInput(speed, acceleration, deceleration);
        player.velocity = -new Vector3(player.head.GlobalBasis.Z.X, Math.Min(player.head.GlobalBasis.Z.Y, -0.25f), player.head.GlobalBasis.Z.Z) * dashVelocity;
        player.UpdateVelocity();
		EmitSignal(SignalName.transition, "fall");
	} 
}
