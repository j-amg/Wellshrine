using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class InventoryData : Resource
{
    
    [Export] public Array<SlotData> slotDatas = [];

    public SlotData GrabSlotData(int index)
    {
        SlotData slotData = slotDatas[index];

        return slotData == null ? null : slotData;
    }

    public void OnSlotClicked(int index)
    {
        GD.Print("Inventory clicked at: " + index);
    }
}