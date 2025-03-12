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
    public Panel dialogue;
    [Export]
    public Panel popup;
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
    public Player parent;
    public override void _Ready()
    {
        parent = GetOwner<Player>();
		parent.damageTaken += OnDamageTaken;
        healthBar.MaxValue = Global.Singleton.playerHealth;
		healthBar.Value = Global.Singleton.playerHealth;
    }

    private void OnDamageTaken() => healthBar.Value = Global.Singleton.currentPlayerHealth;

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
}
