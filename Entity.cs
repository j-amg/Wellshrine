using Godot;
using System;

public partial class Entity : CharacterBody3D
{
    [Signal] public delegate void DamageTakenEventHandler(Damage d);
    [Signal] public delegate void HealthChangedEventHandler(Entity entity);
    [Signal] public delegate void DamageExecutedEventHandler(Damage d);
    [Signal] public delegate void DiedEventHandler(Entity entity);
    [Export] public StateMachine stateMachine;
    [Export] public AttributeData attributeData;
    [Export] public RayCast3D lookRay;
    [Export]
    public AnimationPlayer animationPlayer;
    public Vector3 velocity;
    public bool dead = false;
    public float Health { get; protected set; }

    public override void _Ready()
    {
        attributeData.SetDefaultValues();
    }

    public override void _PhysicsProcess(double delta)
    {
        return;
    }

    public virtual void TakeDamage(Damage d)
    {
        if (dead) return;
        d.Hit();
        Health = Mathf.Clamp(Health - d.amount, 0, attributeData.Attributes[AttributeType.Health].Value);
        UpdateHealth();
    }

    public void SetHealth(float value)
    {
        if (dead) return;
        Health = Mathf.Clamp(value, 0, attributeData.Attributes[AttributeType.Health].Value);
        UpdateHealth();
    }

    public void UpdateHealth()
    {
        EmitSignal(SignalName.HealthChanged, this);
        if (Health <= 0) Die();
    }

    public virtual void Die()
    {
        dead = true;
        EmitSignal(SignalName.Died);
        return;
    }
}
