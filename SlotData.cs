using System;
using Godot;

[GlobalClass]
public partial class SlotData : Resource
{
    [Export] public ItemData itemData;
    private int quantity = 1;
    [Export(PropertyHint.Range, "1, 64, 1")] public int Quantity
    {
        get
        {
            return quantity;
        }
        set
        {
            if (value > 1 && !itemData.stackable)
            {
                quantity = 1;
                GD.PushError($"{itemData.name} is not stackable, setting quantity to 1");
            }
            else
            {
                quantity = value;
            }
        }
}
}
