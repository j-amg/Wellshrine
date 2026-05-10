using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class Attribute : Resource
{
    [Signal] public delegate void AttributesUpdatedEventHandler();
    public float BaseValue { get; protected set; }
    private readonly List<AttributeModifier> attributeModifiers;
    private bool isDirty = true;
    private float _value;
    private float lastBaseValue = float.MinValue;

    // done so that the final value doesnt need to be calculated each time
    public float Value
    {
        get
        {
            if (isDirty || BaseValue != lastBaseValue)
            {
                lastBaseValue = BaseValue;
                _value = CalculateFinalValue();
                isDirty = false;
            }
            return _value;
        }
    }

    public void SetBaseValue(float value)
    {
        isDirty = true;
        BaseValue = value;
        EmitSignal(SignalName.AttributesUpdated);
    }

    public Attribute(float baseValue)
    {
        BaseValue = baseValue;
        attributeModifiers = [];
    }

    public void AddModifier(AttributeModifier mod)
    {
        //GD.Print("added modifier");

        isDirty = true;
        attributeModifiers.Add(mod);
        attributeModifiers.Sort(CompareModifierOrder);
        EmitSignal(SignalName.AttributesUpdated);
    }

    private int CompareModifierOrder(AttributeModifier a, AttributeModifier b)
    {
        if (a.Order < b.Order) { return -1; }
        else if (a.Order > b.Order) { return 1; }
        return 0;
    }

    public bool RemoveModifier(AttributeModifier mod)
    {
        GD.Print("removed modifier");
        isDirty = true;


        if (attributeModifiers.Remove(mod))
        {
            isDirty = true;
            EmitSignal(SignalName.AttributesUpdated);
            return true;
        }
        return false;
    }

    public bool RemoveAllModifiersFromSource(object source)
    {
        GD.Print("removed all modifiers from: " + source);
        bool didRemove = false;
        for (int i = attributeModifiers.Count - 1; i >= 0; i--)
        {
            if (attributeModifiers[i].Source == source)
            {
                GD.Print("did remove modifiers");
                isDirty = true;
                attributeModifiers.RemoveAt(i);
                didRemove = true;
                EmitSignal(SignalName.AttributesUpdated);
            }
        }

        return didRemove;
    }

    private float CalculateFinalValue()
    {
        float finalValue = BaseValue;
        float sumPercentAdd = 0;
        for (int i = 0; i < attributeModifiers.Count; i++)
        {
            if (attributeModifiers[i].ModType == AttributeModType.Flat)
            {
                finalValue += attributeModifiers[i].Value;
            }
            else if (attributeModifiers[i].ModType == AttributeModType.PercentAdd)
            {
                sumPercentAdd += attributeModifiers[i].Value;
                if (i + 1 >= attributeModifiers.Count || attributeModifiers[i + 1].ModType != AttributeModType.PercentAdd)
                {
                    finalValue *= 1 + sumPercentAdd / 100;
                }
            }
            else if (attributeModifiers[i].ModType == AttributeModType.PercentMult)
            {
                finalValue *= 1 + attributeModifiers[i].Value / 100;
            }
        }
        return (float)MathF.Round(finalValue, 4);
    }
}
