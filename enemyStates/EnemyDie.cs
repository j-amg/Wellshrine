using Godot;
using System;

public partial class EnemyDie : State
{
    public override void Enter()
    {
        base.Enter();
        owner.sprite.Play("die");
        owner.SetCollisionLayerValue(3, false);
        DeathDelay();
        owner.Velocity = Vector3.Zero;
        owner.labelSprite.Visible = false;
        SetPhysicsProcess(false);
    }

    public async void DeathDelay()
    {
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(owner.sprite, "modulate", new Color(0,0,0,0), 1);
        await ToSignal(GetTree().CreateTimer(1), "timeout");
        owner.Die();
    }
}
