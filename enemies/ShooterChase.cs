using Godot;
using System;

public partial class ShooterChase : State
{
    // [Export]
    // Enemy enemy;
    [Export]
    NavigationAgent3D nav;
    [Export]
    RayCast3D rc;
    private Vector3 velocity;
    private Enemy enemy;

    public override void Enter()
    {
        base.Enter();
        enemy = GetOwner<Enemy>();
    }
    public override void Update(double delta)
    {
        base._PhysicsProcess(delta);
        Vector3 direction;
		velocity = enemy.Velocity;
		nav.TargetPosition = player.GlobalPosition;
		direction = nav.GetNextPathPosition() - enemy.GlobalPosition;
		direction = direction.Normalized();
		velocity = velocity.Lerp(direction * enemy.baseMovementSpeed, (float)(enemy.acceleration * delta));
		enemy.Velocity = velocity;
        //rc.TargetPosition = enemy.ToLocal(player.head.GlobalPosition);
        if (Global.Singleton.dead) EmitSignal(SignalName.transition, "idle");
        if ((player.GlobalPosition - enemy.GlobalPosition).Length() <= enemy.attackRange) EmitSignal(SignalName.transition, "attack");
    }
}
