using Godot;
using System;

public partial class Label : Node3D
{
	private Global globals;
	private Label3D healthLabel;
	private Label3D nameLabel;
	private Label3D levelLabel;
	private Enemy parent;

    public override void _Ready()
	{
		parent = GetParent<Enemy>();
		parent.damageTaken += OnDamageTaken;
		globals = GetNode<Global>("/root/Global");
		healthLabel = GetNode<Label3D>("health");
		nameLabel = GetNode<Label3D>("name");
		levelLabel = GetNode<Label3D>("level");

		healthLabel.Text = parent.currentHealth.ToString();
		nameLabel.Text = parent.name.ToString();
		levelLabel.Text = parent.level.ToString();

	}

    private void OnDamageTaken()
    {
        healthLabel.Text = parent.currentHealth.ToString();
    }

    public override void _Process(double delta)
	{
		Rotation = new Vector3(Rotation.X, Mathf.Atan2(globals.player.GlobalPosition.X - GlobalPosition.X, globals.player.GlobalPosition.Z - GlobalPosition.Z), Rotation.Z);
	}
}
