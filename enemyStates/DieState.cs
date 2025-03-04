using Godot;
using System;

public partial class DieState : State
{
    public Enemy enemy;
    private Vector3 velocity;

    public override void Enter()
    {
        base.Enter();
        enemy = GetOwner<Enemy>();
        enemy.sprite.Play("die");
        enemy.SetCollisionLayerValue(3, false);
        DeathDelay();
        velocity = Vector3.Zero;
        enemy.Velocity = velocity;
        enemy.labelSprite.Visible = false;
        SetPhysicsProcess(false);
    }

    public async void DeathDelay()
    {
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(enemy.sprite, "modulate", new Color(0,0,0,0), 1);
        await ToSignal(GetTree().CreateTimer(1), "timeout");
        CallDeferred("queue_free");
    }
}
