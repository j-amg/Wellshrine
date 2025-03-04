using Godot;
using System;

public partial class ChaserIdle : State
{
    // [Export]
    // Enemy enemy;
    [Export]
    NavigationAgent3D nav;
    private Vector3 velocity;
    public Enemy enemy;


    public override void Enter()
    {
        base.Enter();
        enemy = GetOwner<Enemy>(); 
    }

    public override void Update(double delta)
    {
        if (enemy.awake) enemy.sprite.Play("idle"); else enemy.sprite.Play("spawn"); 
        if (enemy.stunned) enemy.sprite.Play("stun");
        velocity = Vector3.Zero;
        if (!enemy.IsOnFloor()) velocity.Y -= player.gravity * (float)delta;
        enemy.Velocity = velocity;
        if (enemy.dead) EmitSignal(SignalName.transition, "die");
        if ((enemy.awake && (player.GlobalPosition - enemy.GlobalPosition).Length() <= enemy.detectionRange && enemy.inview) || enemy.damaged) EmitSignal(SignalName.transition, "chase");
    }
}
