using Godot;
using System;

public partial class EnemyLabel : VBoxContainer
{
	private ProgressBar healthBar;
	private Label nameLabel;
	private Label levelLabel;
	private Enemy parent;

	public override void _Ready()
	{
		parent = GetOwner<Enemy>();
		parent.damageTaken += OnDamageTaken;
		healthBar = GetNode<ProgressBar>("health");
		nameLabel = GetNode<Label>("name");
		levelLabel = GetNode<Label>("level");
		nameLabel.Text = parent.name.ToString();
		levelLabel.Text = parent.level.ToString();
		healthBar.Value = parent.currentHealth;
	}
	private void OnDamageTaken() => healthBar.Value = parent.currentHealth;

}
