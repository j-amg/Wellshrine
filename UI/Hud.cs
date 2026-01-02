using Godot;
using System;

public partial class Hud : Control
{
    [Export]
    public ProgressBar healthBar;
    [Export]
    public Label healthLabel;
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
    [Export]
    public TextureRect screen;
    [Export]
    public Tooltip itemTooltip;
    [Export]
    BoxContainer zoneInformation;
    public override void _Ready()
    {
        Global.Singleton.player.damageTaken += OnDamageTaken;
        Global.Singleton.HealthChanged += OnHealthChanged;
    }

    private void OnZoneObjectiveComplete(Zone zone)
    {
        objectiveLabel.Text = "Objective: " + zone.objective.ToString();
    }

    private void OnZoneEntered(Zone zone) => UpdateZoneInformation(zone);

    public void UpdateZoneInformation(Zone zone)
    {
        zoneLabel.Text = "Zone: " + Global.Singleton.currentLevel.ToString();
        objectiveLabel.Text = zone.objective == null ? "" : "Objective: " + zone.objective.ToString();
        UpdateHealth();
    }

    private void OnHealthChanged() => UpdateHealth();

    public void UpdateHealth()
    {
        healthBar.MaxValue = Global.Singleton.playerHealth;
        healthBar.Value = Global.Singleton.currentPlayerHealth;
        healthLabel.Text = "HP: " +  Mathf.Round(Global.Singleton.currentPlayerHealth) + "/" + Global.Singleton.playerHealth;
    }

    private void OnDamageTaken()
    {
        Tween tween = CreateTween();
        tween.TweenProperty(healthLabel, "modulate", new Color(1,1,1), .25f).From(new Color(1,0,0));
        Flash(new Color(1, 0, 0));
    }

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
        Global.Singleton.currentZone.ZoneObjectiveComplete -= OnZoneObjectiveComplete;
    }

    public void SetScreen(Color col) => screen.Modulate = col;

    public void FadeScreen(Color col, float dur)
    {
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(screen, "modulate", col, dur);
    }
}
