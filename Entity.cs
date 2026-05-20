using Godot;
using System;
using Godot.Collections;

[GlobalClass]

public partial class Entity : CharacterBody3D
{
    public readonly static Array<AttributeDefault> attributeDefaults = [
        new AttributeDefault(AttributeType.Strength, 50, AttributeModType.Flat),
        new AttributeDefault(AttributeType.Dexterity, 50, AttributeModType.Flat),
        new AttributeDefault(AttributeType.Intelligence, 50, AttributeModType.Flat),
        new AttributeDefault(AttributeType.Armour, 0, AttributeModType.Flat),
        new AttributeDefault(AttributeType.MaximumHealth, 100, AttributeModType.Flat),
        new AttributeDefault(AttributeType.MaximumMana, 100, AttributeModType.Flat),
        new AttributeDefault(AttributeType.TotalDamage, 0, AttributeModType.PercentAdd),
        new AttributeDefault(AttributeType.PhysicalDamage, 0, AttributeModType.PercentAdd),
        new AttributeDefault(AttributeType.LightningDamage, 0, AttributeModType.PercentAdd),
        new AttributeDefault(AttributeType.FireDamage, 0, AttributeModType.PercentAdd),
        new AttributeDefault(AttributeType.ColdDamage, 0, AttributeModType.PercentAdd),
        new AttributeDefault(AttributeType.FlatLightningDamage, 0, AttributeModType.Flat),
        new AttributeDefault(AttributeType.FlatFireDamage, 0, AttributeModType.Flat),
        new AttributeDefault(AttributeType.FlatColdDamage, 0, AttributeModType.Flat),
        new AttributeDefault(AttributeType.LightningResistance, 0, AttributeModType.Flat),
        new AttributeDefault(AttributeType.FireResistance, 0, AttributeModType.Flat),
        new AttributeDefault(AttributeType.ColdResistance, 0, AttributeModType.Flat),
        new AttributeDefault(AttributeType.HealthRegen, 0, AttributeModType.Flat),
        new AttributeDefault(AttributeType.ProjectileCount, 0, AttributeModType.Flat),
        new AttributeDefault(AttributeType.CastSpeed, 0, AttributeModType.PercentAdd),
        new AttributeDefault(AttributeType.ProjectileSpeed, 0, AttributeModType.PercentAdd),
        new AttributeDefault(AttributeType.MovementSpeed, 0, AttributeModType.PercentAdd),
    ];
    [Signal] public delegate void DamageTakenEventHandler(Entity entity, DamagePackage d, Entity source);
    [Signal] public delegate void DamageExecutedEventHandler(Entity entity, DamagePackage d, Entity target);
    [Signal] public delegate void HealthChangedEventHandler(Entity entity);
    [Signal] public delegate void ManaChangedEventHandler(Entity entity);
    [Signal] public delegate void DiedEventHandler(Entity entity);
    [Export] public string name = "[PH] Entity";
    [Export] public StateMachine stateMachine;
    public AttributeData attributeData = new();
    [Export] private Array<AttributeDefault> attributeOverrides = [];
    [Export] public RayCast3D lookRay;
    [Export] public AnimationPlayer animationPlayer;
    [Export] public Area3D hitBox;
    [Export] public VisibleOnScreenNotifier3D visibleOnScreenNotifier3D;

    public bool visibleOnScreen;

    public Array<EntityEffect> entityEffects = [];

    public Transform3D lookTransform;

    public bool initialised = false;
    public Vector3 velocity;
    public bool dead = false;
    public float Health { get; protected set; }
    public float Mana { get; protected set; }

    public override void _Ready()
    {
        if (visibleOnScreenNotifier3D != null)
        {
            visibleOnScreenNotifier3D.ScreenEntered += OnScreenEntered;
            visibleOnScreenNotifier3D.ScreenExited += OnScreenExited;
        }

        attributeData.SetDefaultValues(attributeOverrides);
        Initialise();
    }

    private void OnScreenExited() => visibleOnScreen = false;

    private void OnScreenEntered() => visibleOnScreen = true;

    public virtual void Initialise()
    {
        initialised = true;
        SetHealth(attributeData.attributes[AttributeType.MaximumHealth].Value);
    }

    public override void _PhysicsProcess(double delta)
    {
        SetLookTransform();
        for (int i = entityEffects.Count - 1; i >= 0; i--) entityEffects[i].Update();
    }

    public virtual void AddEffect(EntityEffect entityEffect)
    {
        entityEffect.Initialise(this);
        entityEffects.Add(entityEffect);
    }

    protected virtual void SetLookTransform() => lookTransform = GlobalTransform;
    public virtual void IncrementHealth(float amount)
    {
        if (dead) return;
        Health = Mathf.Clamp(Health + amount, 0, attributeData.attributes[AttributeType.MaximumHealth].Value);
        UpdateHealth();
    }

    public virtual bool IncrementMana(float value)
    {
        if (dead) return false;
        if (Mana + value < 0) return false;
        Mana = Mathf.Clamp(Mana + value, 0, attributeData.attributes[AttributeType.MaximumMana].Value);
        UpdateMana();
        return true;
    }

    public virtual void SetHealth(float value)
    {
        if (dead) return;
        Health = Mathf.Clamp(value, 0, attributeData.attributes[AttributeType.MaximumHealth].Value);
        UpdateHealth();
    }

    public virtual void UpdateHealth()
    {

        GD.Print("Entity: " + Name + " Health: " + Health + " Max Health: " + attributeData.attributes[AttributeType.MaximumHealth].Value);
        EmitSignal(SignalName.HealthChanged, this);
        if (!initialised) return;
        if (Health <= 0) Die();
    }

    public virtual void UpdateMana()
    {
        GD.Print("Entity: " + Name + " Mana: " + Mana + " Max Mana: " + attributeData.attributes[AttributeType.MaximumMana].Value);
        EmitSignal(SignalName.ManaChanged, this);
        if (!initialised) return;
    }

    public virtual void Die()
    {
        dead = true;
        EmitSignal(SignalName.Died);
    }

    // public override void _ExitTree()
    // {
    //     attributeData.DefaultValuesSet -= Initialise;
    // }
}
