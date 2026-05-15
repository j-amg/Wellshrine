using Godot;
using System;
using Godot.Collections;

[GlobalClass]

public partial class Entity : CharacterBody3D
{
    [Signal] public delegate void DamageTakenEventHandler(DamagePackage d);
    [Signal] public delegate void HealthChangedEventHandler(Entity entity);
    [Signal] public delegate void DamageExecutedEventHandler(DamagePackage d);
    [Signal] public delegate void DiedEventHandler(Entity entity);
    [Export] public string name = "[PH] Entity";
    [Export] public StateMachine stateMachine;
    public AttributeData attributeData = new();
    [Export] private Array<AttributeDefault> attributeOverrides = [];
    [Export] public RayCast3D lookRay;
    [Export] public AnimationPlayer animationPlayer;

    public Array<EntityEffect> entityEffects = [];

    public Transform3D lookTransform;

    public bool initialised = false;
    public Vector3 velocity;
    public bool dead = false;
    public float Health { get; protected set; }

    public override void _Ready()
    {
        attributeData.SetDefaultValues(attributeOverrides);
        Initialise();
    }

    public virtual void Initialise()
    {
        initialised = true;
        SetHealth(attributeData.attributes[AttributeType.MaximumHealth].Value);
    }

    public override void _PhysicsProcess(double delta)
    {
        SetLookTransform();
        UpdateEffects(delta);
    }

    public void UpdateEffects(double _delta)
    {
        //GD.Print(entityEffects.Count);
        for (int i = 0; i < entityEffects.Count; i++)
        {
            //GD.Print(entityEffects.Count);
            EntityEffect entityEffect = entityEffects[i];
            
            int elapsedTimeSecs = (int)Time.GetTicksMsec() - (int)entityEffect.effectStartTime / 1000;

            if (elapsedTimeSecs % entityEffect.effectInterval == 0)
            {
                GD.Print("interval");
                entityEffect.ApplyEffect(this);
            }

            // if (elapsedTimeSecs / 1000 > entityEffect.effectDuration)
            // {
            //     entityEffects.Remove(entityEffect);
            // }
        }
    }

    public virtual void AddEffect(EntityEffect entityEffect)
    {
        entityEffects.Add(entityEffect);
        entityEffect.effectStartTime = Time.GetTicksMsec();
        
        GD.Print(entityEffects.Count);
        
        
    }

    protected virtual void SetLookTransform() => lookTransform = GlobalTransform;
    public virtual void TakeDamage(DamagePackage damagePackage)
    {
        if (dead) return;
        damagePackage.Hit();
        foreach (DamageInst damage in damagePackage.damageInstances)
        {
            Global.Singleton.currentScene.GetNode<CanvasLayer>("UI").AddChild(new DamageNumber(damage, this));
            float scaledAmount = DamageInst.ScaleToEntityDefense(damage, this).amount;
            GD.Print("damge: " + scaledAmount);
            Health = Mathf.Clamp(Health - scaledAmount, 0, attributeData.attributes[AttributeType.MaximumHealth].Value);
        }
        UpdateHealth();
    }

    public virtual void SetHealth(float value)
    {
        if (dead) return;
        Health = Mathf.Clamp(value, 0, attributeData.attributes[AttributeType.MaximumHealth].Value);
        UpdateHealth();
    }

    public virtual void UpdateHealth()
    {
        if (!initialised) return;
        GD.Print("Entity: " + Name + " Health: " + Health + " Max Health: " + attributeData.attributes[AttributeType.MaximumHealth].Value);
        EmitSignal(SignalName.HealthChanged, this);
        if (Health <= 0) Die();
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
