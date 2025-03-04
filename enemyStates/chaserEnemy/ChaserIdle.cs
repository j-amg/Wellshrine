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
        SpawnDelay();
    }

    public async void SpawnDelay()
    {
		await ToSignal(GetTree().CreateTimer(1), "timeout");
        enemy.awake = true;
    }

    public override void Update(double delta)
    {
        //if (enemy.sprite.Frame < 6 && enemy.sprite.Animation == "spawn" || enemy.sprite.Animation == "idle") enemy.sprite.Play("idle");
        enemy.sprite.Play("spawn");
        velocity = Vector3.Zero;
        if (!enemy.IsOnFloor()) velocity.Y -= player.gravity * (float)delta;
        enemy.Velocity = velocity;
        if (enemy.awake && (player.GlobalPosition - enemy.GlobalPosition).Length() <= enemy.detectionRange && enemy.inview) EmitSignal(SignalName.transition, "chase");
    }
}
