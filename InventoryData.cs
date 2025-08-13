using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class InventoryData : Resource
{

    [Signal] public delegate void InventoryInteractedEventHandler(InventoryData inventoryData, int index, int buttonIndex);
    [Signal] public delegate void InventoryUpdatedEventHandler(InventoryData inventoryData);
    
    [Export] public Array<SlotData> slotDatas = [];

    public void OnSlotClicked(int index, int buttonIndex)
    {
        EmitSignal(SignalName.InventoryInteracted, this, index, buttonIndex);
    }

    public SlotData GrabSlotData(int index)
    {
        SlotData slotData = slotDatas[index];

        if (slotData == null)
        {
            return null;
        }
        else
        {
            slotDatas[index] = null;
            EmitSignal(SignalName.InventoryUpdated, this);
            return slotData;
        }
    }

    public SlotData DropSlotData(int index, SlotData grabbedSlotData)
    {
        SlotData currentSlotData = slotDatas[index];


        SlotData returnSlotData = null;
        if (currentSlotData != null && currentSlotData.CanFullyMergeWith(grabbedSlotData))
        {
            
            currentSlotData.FullyMergeWith(grabbedSlotData);
        }
        else
        {
            slotDatas[index] = grabbedSlotData;
            returnSlotData = currentSlotData;
        }

        EmitSignal(SignalName.InventoryUpdated, this);
        return returnSlotData;
    }

    public SlotData DropSingleSlotData(int index, SlotData grabbedSlotData)
    {
        SlotData currentSlotData = slotDatas[index];

        if (currentSlotData == null)
        {
            slotDatas[index] = grabbedSlotData.CreateSingleSlotData();
        }
        else if (currentSlotData.CanMergeWith(grabbedSlotData))
        {
            currentSlotData.FullyMergeWith(grabbedSlotData.CreateSingleSlotData());
        }

        EmitSignal(SignalName.InventoryUpdated, this);
        if (grabbedSlotData.Quantity > 0) return grabbedSlotData; else return null;
    }
}