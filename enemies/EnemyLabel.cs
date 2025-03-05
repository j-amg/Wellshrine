using System;
using System.Diagnostics;
using Godot;

public partial class EnemyLabel : VBoxContainer
{
	private ProgressBar healthBar;
	private ProgressBar healthBackground;
	private Label nameLabel;
	private Label levelLabel;
	private Label damageLabel;
	private Enemy parent;

	public override void _Ready()
	{
		parent = GetOwner<Enemy>();
		parent.damageTaken += OnDamageTaken;

	}

	public void SetValues()
	{
		healthBar = GetNode<ProgressBar>("Control/health");
		healthBackground = GetNode<ProgressBar>("Control/healthBackground");
		nameLabel = GetNode<Label>("name");
		levelLabel = GetNode<Label>("level");
		damageLabel = GetNode<Label>("damage");
		nameLabel.Text = parent.name.ToString();
		levelLabel.Text = parent.level.ToString();
		healthBar.MaxValue = parent.baseHealth;
		healthBar.Value = parent.baseHealth;
		healthBackground.MaxValue = parent.baseHealth;
		healthBackground.Value = parent.baseHealth;
	}
    private async void OnDamageTaken(float damage)
    {
		damageLabel.Text = Math.Round(damage).ToString();
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(damageLabel, "modulate", new Color (0,0,0,0), 2).From(new Color(1,1,1,1));

        healthBar.Value = parent.currentHealth;
		await ToSignal(GetTree().CreateTimer(.25), "timeout");
		healthBackground.Value = parent.currentHealth;
    }
}
