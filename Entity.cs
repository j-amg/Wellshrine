using Godot;
using System;

[GlobalClass]

public partial class Entity : CharacterBody3D
{
    [Signal] public delegate void DamageTakenEventHandler(Damage d);
    [Signal] public delegate void HealthChangedEventHandler(Entity entity);
    [Signal] public delegate void DamageExecutedEventHandler(Damage d);
    [Signal] public delegate void DiedEventHandler(Entity entity);
    [Export] public string name = "[PH] Entity";
    [Export] public StateMachine stateMachine;
    [Export] public AttributeData attributeData;
    [Export] public RayCast3D lookRay;
    [Export] public AnimationPlayer animationPlayer;

    public bool initialised = false;
    public Vector3 velocity;
    public bool dead = false;
    public float Health { get; protected set; }

    public override void _Ready()
    {
        attributeData.DefaultValuesSet += OnAttributeDataDefaultValuesSet;
        attributeData.SetDefaultValues();
    }

    private void OnAttributeDataDefaultValuesSet() => Initialise();

    public virtual void Initialise()
    {
        Health = attributeData.Attributes[AttributeType.MaxHealth].Value;
        GD.Print("Health at initialisation" + Health);
        UpdateHealth();
    }

    public virtual void TakeDamage(Damage d)
    {
        GD.Print("try take damage");
        if (dead) return;
        d.Hit();
        Health = Mathf.Clamp(Health - d.amount, 0, attributeData.Attributes[AttributeType.MaxHealth].Value);
        UpdateHealth();
    }

    public virtual void SetHealth(float value)
    {
        if (dead) return;
        Health = Mathf.Clamp(value, 0, attributeData.Attributes[AttributeType.MaxHealth].Value);
        UpdateHealth();
    }

    public virtual void UpdateHealth()
    {

        GD.Print("Entity: " + Name + " Health: " + Health + " Max Health: " + attributeData.Attributes[AttributeType.MaxHealth].Value);
        EmitSignal(SignalName.HealthChanged, this);
        if (Health <= 0) Die();
    }

    public virtual void Die()
    {
        //dead = true;
        EmitSignal(SignalName.Died);
    }
}
