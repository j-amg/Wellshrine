using Godot;
using System;

public partial class Hud : Control
{
    [Export]
    public ProgressBar healthBar;
    [Export]
	  public Label zoneLabel;
    [Export]
    public Label objectiveLabel;
    [Export]
    public Label dialogueText;
    [Export]
    public Label dialogueName;
    [Export]
    public PanelContainer dialogue;
    [Export]
    public PanelContainer popup;
    [Export]
    public Label popupText;
    [Export]
    public TextureRect reticle;
    [Export]
    public Control crossHair;
    [Export]
    public Label interactLabel;
    [Export]
    public TextureRect hitFlash;
    [Export]
    public ProgressBar rechargeBar;
    public Player player;
    public override void _Ready()
    {
        player = GetOwner<Player>();
		    player.damageTaken += OnDamageTaken;
        Global.Singleton.HealthChanged += OnHealthChanged;
        if (Global.Singleton.currentZone != null) {
          Global.Singleton.currentZone.ZoneEntered += OnZoneEntered;
          Global.Singleton.currentZone.ZoneOjectiveComplete += OnZoneObjectiveComplete;
        } 
        healthBar.MaxValue = Global.Singleton.playerHealth;
		    healthBar.Value = Global.Singleton.playerHealth;
    }

    private void OnZoneObjectiveComplete(Zone zone)
    {
        objectiveLabel.Text = "Objective: " + zone.objective.ToString();
    }

    private void OnZoneEntered(Zone zone)
    {
        GD.Print("zone entered");
        zoneLabel.Text = "Zone: " + Global.Singleton.currentLevel.ToString();
        objectiveLabel.Text = "Objective: " + zone.objective.ToString();
    }

    private void OnHealthChanged()
    {
        healthBar.MaxValue = Global.Singleton.playerHealth;
        healthBar.Value = Global.Singleton.currentPlayerHealth;
        GD.Print("health");
    }

    private void OnDamageTaken() => Flash(new Color(1,0,0));

    public void FlashCrossHair()
    {
		  Tween tween = GetTree().CreateTween();
		  tween.TweenProperty(crossHair, "modulate", new Color(0,0,0,0), .2).From(new Color(1,0,0,.5f));
    }

    public void Flash(Color col)
    {
		  Tween tween = GetTree().CreateTween();
		  tween.TweenProperty(hitFlash, "modulate", new Color(0,0,0,0), .25).From(col);
    }

    public override void _ExitTree() 
    {
        Global.Singleton.HealthChanged -= OnHealthChanged;
        Global.Singleton.currentZone.ZoneEntered -= OnZoneEntered;
        Global.Singleton.currentZone.ZoneOjectiveComplete -= OnZoneObjectiveComplete;
    }
}
