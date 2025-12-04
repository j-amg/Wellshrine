using Godot;
using System;

public partial class SlideState : State
{
    private float finalSlideDelta;
    public override void Enter()
    {
        owner.UpdateInput(owner.hvel.Length(), .75f, 0);

        // move in input direction, otherwise move in velocity direction
        Vector3 slideDir = owner.inputDir.Length() != 0 ? new Vector3(owner.direction.X, 0, owner.direction.Z) : new Vector3(owner.velocity.Normalized().X, 0, owner.velocity.Normalized().Z);

        // if the current speed is greater than the slidespeed then use the current speed instead
        float finalSlideSpeed = owner.hvel.Length() > owner.slideSpeed ? owner.hvel.Length() : owner.slideSpeed;

        owner.velocity = owner.velocity.MoveToward(slideDir * finalSlideSpeed, owner.slideDelta);
        owner.UpdateVelocity();
    }
    public override void Update(double delta)
	{
        if (!owner.IsOnFloor()) EmitSignal(SignalName.transition, "fall"); else EmitSignal(SignalName.transition, "crouch");
	}
}
