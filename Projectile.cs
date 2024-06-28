using Godot;
using System;

public partial class Projectile : Area3D
{
	public float muzzleVelocity = 25;
	//public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	Vector3 g = Vector3.Down * 5;
	public Vector3 velocity = Vector3.Zero;

	private PackedScene explosion;

	[Signal]
	public delegate void explodedEventHandler(Vector3 position);

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		exploded += OnExploded;
		//explosion = ResourceLoader.Load<PackedScene>("res://explosion.tscn");
		
	}

    private void OnExploded(Vector3 position)
    {
        Explosion e = explosion.Instantiate() as Explosion;
		//b.Position = GlobalPosition;
		//projInstance.GetNode<ProjectileComponent>("projectile_component").direction = dir;
		var main = GetTree().CurrentScene;
		main.CallDeferred("add_child", e);
		e.Position = position;
    }

    private void OnBodyEntered(Node3D body)
    {
        //EmitSignal(SignalName.exploded, Position);
		QueueFree();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		velocity += g * (float)delta;
		LookAt(Transform.Origin + velocity.Normalized(), Vector3.Up);
		var transform = Transform;
		transform.Origin = Transform.Origin + velocity * (float)delta;
		Transform = transform;
	}




}
