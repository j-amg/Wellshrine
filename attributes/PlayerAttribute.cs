using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

public enum AttributeType
{
    Strength,
    Dexterity,
    Intelligence,
}

[GlobalClass]
public partial class PlayerAttribute : Resource
{
    [Export] public float BaseValue;
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

    public PlayerAttribute(float baseValue)
    {
        BaseValue = baseValue;
        attributeModifiers = [];
    }

    public void AddModifier(AttributeModifier mod)
    {
        isDirty = true;
        attributeModifiers.Add(mod);
        attributeModifiers.Sort(CompareModifierOrder);
    }

    private int CompareModifierOrder(AttributeModifier a, AttributeModifier b)
    {
        if (a.Order < b.Order) {return -1;}
        else if (a.Order > b.Order){ return 1;}
        return 0;
    }

    public bool RemoveModifier(AttributeModifier mod)
    {
        isDirty = true;


        if (attributeModifiers.Remove(mod))
        {
            isDirty = true;
            return true;
        }
        return false;
    }

    public bool RemoveAllModifiersFromSource(object source)
    {
        bool didRemove = false;
        for (int i = attributeModifiers.Count; i >= 0; i--)
        {

            if (attributeModifiers[i] == source)
            {
                isDirty = true;
                attributeModifiers.RemoveAt(i);
                didRemove = true;
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
                    finalValue *= 1 + sumPercentAdd;
                }
            }
            else if (attributeModifiers[i].ModType == AttributeModType.PercentMult)
            {
                finalValue *= 1 + attributeModifiers[i].Value;
            }
        }
        return (float)MathF.Round(finalValue, 4);
    }
}
