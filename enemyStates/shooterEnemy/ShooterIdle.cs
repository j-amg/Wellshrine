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
        velocity = Vector3.Zero;
        enemy.Velocity = velocity;
        if ((player.GlobalPosition - enemy.GlobalPosition).Length() <= enemy.detectionRange) EmitSignal(SignalName.transition, "chase");
    }
}
