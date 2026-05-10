using Godot;
using System;

public partial class EnemyDie : State
{
    public override void Enter()
    {
        base.Enter();
        owningEntity.sprite.Play("die");
        owningEntity.SetCollisionLayerValue(3, false);
        DeathDelay();
        owningEntity.Velocity = Vector3.Zero;
        owningEntity.labelSprite.Visible = false;
        SetPhysicsProcess(false);
    }

    public async void DeathDelay()
    {
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(owningEntity.sprite, "modulate", new Color(0, 0, 0, 0), 1);
        await ToSignal(GetTree().CreateTimer(1), "timeout");
        owningEntity.RemoveFromScene();
    }
}
