using System;
using System.Diagnostics;
using Godot;

public partial class EnemyLabel : VBoxContainer
{
	[Export]
	private ProgressBar healthBar;
	[Export]
	private ProgressBar healthBackground;
	[Export]
	private Label nameLabel;
	[Export]
	private Label damageLabel;
	private Enemy parent;

	public override void _Ready()
	{
		parent = GetOwner<Enemy>();
		parent.DamageTaken += OnDamageTaken;
	}

	public void SetValues()
	{
		nameLabel.Text = parent.name.ToString();
		healthBar.MaxValue = parent.Health;
		healthBar.Value = parent.Health;
		healthBackground.MaxValue = parent.Health;
		healthBackground.Value = parent.Health;
	}
	private async void OnDamageTaken(Damage damage)
	{
		damageLabel.Text = Math.Round(damage.amount).ToString();
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(damageLabel, "modulate", new Color(0, 0, 0, 0), 2).From(new Color(1, 1, 1, 1));
		healthBar.Value = parent.Health;
		await ToSignal(GetTree().CreateTimer(.25), "timeout");
		healthBackground.Value = parent.Health;
	}
}
