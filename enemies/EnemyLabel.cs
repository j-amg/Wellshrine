using System;
using System.Diagnostics;
using Godot;

public partial class EnemyLabel : VBoxContainer
{
	[Export] private ProgressBar healthBar;
	[Export] private ProgressBar healthBackground;
	[Export] private Label nameLabel;
	private Entity parent;

	public override void _Ready()
	{
		parent = GetOwner<Entity>();
		parent.HealthChanged += OnHealthChanged;
	}

	public void SetValues()
	{
		nameLabel.Text = parent.name.ToString();
		healthBar.MaxValue = parent.Health;
		healthBar.Value = parent.Health;
		healthBackground.MaxValue = parent.Health;
		healthBackground.Value = parent.Health;
	}
	private async void OnHealthChanged(Entity _entity)
	{
		healthBar.Value = parent.Health;
		await ToSignal(GetTree().CreateTimer(.25), "timeout");
		healthBackground.Value = parent.Health;
	}
}
