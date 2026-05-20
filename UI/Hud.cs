using Godot;
using System;

public partial class Hud : Control
{
    [Export]
    public ProgressBar healthBar;
    [Export]
    public ProgressBar manaBar;
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
    [Export]
    public TextureProgressBar attackChargeIndicator;

    public override void _Ready()
    {
        //Global.Singleton.player.DamageTaken += OnDamageTaken;
        //SignalManager.Singleton.PlayerHealthChanged += OnPlayerHealthChanged;
        SignalManager.Singleton.AttackChargeUpdated += OnAttackChargeUpdated;
    }

    public void SpawnDamageNumber(DamageInst damage, Entity entity)
    {
        DamageNumber dn = Global.Singleton.damageNumberScene.Instantiate<DamageNumber>();
        dn.Initialise(damage, entity);
        AddChild(dn);
    }

    internal void OnPlayerDamageExecuted(Entity entity, DamagePackage damage, Entity target)
    {
        GD.Print("damage executed");
        FlashCrossHair();
        foreach (DamageInst d in damage.damageInstances)
        {
            if (target.visibleOnScreen) SpawnDamageNumber(d, target);
        }
    }

    internal void OnPlayerDamageTaken(Entity entity, DamagePackage d, Entity source)
    {
        Tween tween = CreateTween();
        tween.TweenProperty(healthLabel, "modulate", new Color(1, 1, 1), .25f).From(new Color(1, 0, 0));
        Flash(new Color(1, 0, 0));
        UpdateHealth(entity);
    }
    internal void OnPlayerHealthChanged(Entity player) => UpdateHealth(player);
    internal void OnPlayerManaChanged(Entity player) => UpdateMana(player);

    internal void OnAttackChargeUpdated(float value) => attackChargeIndicator.Value = value;

    //private void OnZoneEntered(Zone zone) => UpdateZoneInformation(zone);

    public void UpdateZoneInformation(Zone zone)
    {
        zoneLabel.Text = "Zone: " + Global.Singleton.currentLevel.ToString();
        objectiveLabel.Text = zone.objective == null ? "" : "Objective: " + zone.objective.ToString();
    }

    private void OnZoneObjectiveComplete(Zone zone)
    {
        objectiveLabel.Text = "Objective: " + zone.objective.ToString();
    }

    public void UpdateHealth(Entity player)
    {
        healthBar.MaxValue = player.attributeData.attributes[AttributeType.MaximumHealth].Value;
        healthBar.Value = player.Health;
        healthLabel.Text = "HP: " + Mathf.Round(player.Health) + "/" + player.attributeData.attributes[AttributeType.MaximumHealth].Value;
    }

    public void UpdateMana(Entity player)
    {
        manaBar.MaxValue = player.attributeData.attributes[AttributeType.MaximumMana].Value;
        manaBar.Value = player.Health;
        //healthLabel.Text = "HP: " + Mathf.Round(player.Health) + "/" + player.attributeData.attributes[AttributeType.MaximumHealth].Value;
    }
    public void FlashCrossHair()
    {
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(crossHair, "modulate", new Color(0, 0, 0, 0), .5).From(new Color(1, 1, 1, 1f));
    }

    public void Flash(Color col)
    {
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(hitFlash, "modulate", new Color(0, 0, 0, 0), .25).From(col);
    }

    public override void _ExitTree()
    {
        SignalManager.Singleton.PlayerHealthChanged -= OnPlayerHealthChanged;
        //SignalManager.Singleton.ZoneEntered -= OnZoneEntered;
        SignalManager.Singleton.ZoneObjectiveComplete -= OnZoneObjectiveComplete;
    }

    public void SetScreen(Color col) => screen.Modulate = col;

    public void FadeScreen(Color col, float dur)
    {
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(screen, "modulate", col, dur);
    }
}
