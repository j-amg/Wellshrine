using System;
using Godot;

[GlobalClass]
public partial class SlotData : Resource
{
    public const int MAX_STACK_SIZE = 99;
    [Export] public ItemData itemData;
    private int quantity = 1;
    [Export(PropertyHint.Range, "1, 99, 1")]
    public int Quantity
    {
        get { return quantity; }
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

    public bool CanMergeWith(SlotData otherSlot)
    {
        return otherSlot.itemData == itemData && itemData.stackable && quantity < MAX_STACK_SIZE;
    }

    public bool CanFullyMergeWith(SlotData otherSlot)
    {
        return otherSlot.itemData == itemData && itemData.stackable && quantity + otherSlot.quantity < MAX_STACK_SIZE;
    }

    public void FullyMergeWith(SlotData otherSlot)
    {
        quantity += otherSlot.quantity;
    }

    public SlotData CreateSingleSlotData()
    {
        SlotData newSlotData = (SlotData)Duplicate();
        newSlotData.quantity = 1;
        quantity -= 1;
        return newSlotData;
    }
}
