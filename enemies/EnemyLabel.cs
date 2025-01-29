using System.Diagnostics;
using Godot;

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
	}

	public void SetValues()
	{
		GD.Print("values set");
		healthBar = GetNode<ProgressBar>("health");
		nameLabel = GetNode<Label>("name");
		levelLabel = GetNode<Label>("level");
		nameLabel.Text = parent.name.ToString();
		levelLabel.Text = parent.level.ToString();
		healthBar.MaxValue = parent.baseHealth;
		healthBar.Value = parent.baseHealth;
	}
	private void OnDamageTaken() => healthBar.Value = parent.currentHealth;


    public override void _Process(double delta)
	{
		Visible = !Global.Singleton.toggled;
	}

}
