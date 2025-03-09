using Godot;
using System;

public partial class Hud : Control
{
    private ProgressBar healthBar;
	private Label zoneNumberLabel;
    private Label objectiveLabel;
    private Player parent;
    public override void _Ready()
    {
        parent = GetOwner<Player>();
		parent.damageTaken += OnDamageTaken;

    }

    private void OnDamageTaken() => healthBar.Value = Global.Singleton.currentPlayerHealth;

    public  void FlashCrossHair()
    {
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(GetNode<Control>("crossHair"), "modulate", new Color(0,0,0,0), .2).From(new Color(1,0,0,.5f));
    }

    public void SetValues()
    {
        healthBar = GetNode<ProgressBar>("health");
		zoneNumberLabel = GetNode<Label>("VBoxContainer/zone");
        objectiveLabel = GetNode<Label>("VBoxContainer/objective");
		zoneNumberLabel.Text = "Zone: " + Global.Singleton.currentLevel.ToString();
        objectiveLabel.Text = "Objective: " + Global.Singleton.Objective.ToString();
		healthBar.MaxValue = Global.Singleton.playerHealth;
		healthBar.Value = Global.Singleton.currentPlayerHealth;
    }
}
