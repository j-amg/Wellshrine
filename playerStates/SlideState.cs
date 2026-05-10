using Godot;
using System;

public partial class SlideState : State
{
    private float finalSlideDelta;
    public override void Enter()
    {
        owningEntity.UpdateInput(owningEntity.hvel.Length(), .75f, 0);

        // move in input direction, otherwise move in velocity direction
        Vector3 slideDir = owningEntity.inputDir.Length() != 0 ? new Vector3(owningEntity.direction.X, 0, owningEntity.direction.Z) : new Vector3(owningEntity.velocity.Normalized().X, 0, owningEntity.velocity.Normalized().Z);

        // if the current speed is greater than the slidespeed then use the current speed instead
        float finalSlideSpeed = owningEntity.hvel.Length() > owningEntity.slideSpeed ? owningEntity.hvel.Length() : owningEntity.slideSpeed;

        owningEntity.velocity = owningEntity.velocity.MoveToward(slideDir * finalSlideSpeed, owningEntity.slideDelta);
        owningEntity.UpdateVelocity();
    }
    public override void Update(double delta)
	{
        if (!owningEntity.IsOnFloor()) EmitSignal(SignalName.transition, "fall"); else EmitSignal(SignalName.transition, "crouch");
	}
}
