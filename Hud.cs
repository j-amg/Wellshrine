using Godot;
using System;

public partial class Hud : Control
{
    private ProgressBar healthBar;
	private Label zoneNumberLabel;
	private Label enemyNumberLabel;
    private Label objectiveLabel;
    private Player parent;
    public override void _Ready()
    {
        parent = GetOwner<Player>();
		parent.damageTaken += OnDamageTaken;

    }

    private void OnDamageTaken() => healthBar.Value = Global.Singleton.currentPlayerHealth;

    public void SetValues()
    {
        GD.Print("try set values");
        healthBar = GetNode<ProgressBar>("health");
		zoneNumberLabel = GetNode<Label>("VBoxContainer/zone");
		enemyNumberLabel = GetNode<Label>("VBoxContainer/enemies");
        objectiveLabel = GetNode<Label>("VBoxContainer/objective");
        enemyNumberLabel.Text = "Enemies remaining: " + GetTree().GetNodesInGroup("enemies").Count.ToString();
		zoneNumberLabel.Text = "Zone: " + Global.Singleton.currentLevel.ToString();
        objectiveLabel.Text = "Objective: " + Global.Singleton.Objective.ToString();
		healthBar.MaxValue = Global.Singleton.playerHealth;
		healthBar.Value = Global.Singleton.currentPlayerHealth;
    }
}
