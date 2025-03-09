using Godot;
using System;

public partial class ShooterIdle : State
{
    // [Export]
    // Enemy enemy;
    [Export]
    NavigationAgent3D nav;
    private Vector3 velocity;
    private Enemy enemy;


    public override void Enter()
    {
        base.Enter();
        enemy = GetOwner<Enemy>();
    }

    public override void Update(double delta)
    {
        enemy.sprite.Play("spawn");
        velocity = Vector3.Zero;
        if (!enemy.IsOnFloor()) velocity.Y -= player.gravity * (float)delta;
        enemy.Velocity = velocity;
        if (enemy.awake && (player.GlobalPosition - enemy.GlobalPosition).Length() <= enemy.detectionRange && enemy.inview) EmitSignal(SignalName.transition, "chase");
    }
}
