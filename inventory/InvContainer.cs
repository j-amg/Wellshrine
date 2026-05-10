using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class InvContainer : GridContainer
{

    public PackedScene slotScene;
    private int slotSize = 32;

    public void InitSlots(InventoryData inventoryData)
    {
        Global.Singleton.inventory.SlotGrabbed += slotData => OnSlotGrabbed(slotData, inventoryData);
        slotScene = GD.Load<PackedScene>("res://inventory/inv_slot.tscn");

        foreach (Node n in GetChildren()) { n.QueueFree(); }

        foreach (SlotData slotdata in inventoryData.slotDatas)
        {
            InvSlot slot = slotScene.Instantiate<InvSlot>();
            AddChild(slot);

            if (slotdata != null) slot.SetSlotData(slotdata);

            slot.SlotInput += inventoryData.OnSlotClicked;
            slot.SlotHover += inventoryData.OnSlotEntered;
            slot.SlotExit += inventoryData.OnSlotExited;
        }
    }

    private void OnSlotGrabbed(SlotData slotData, InventoryData inventoryData)
    {
        if (slotData == null)
        {
            foreach (InvSlot s in GetChildren().Cast<InvSlot>())
            {
                s.highlighted = false;
                s.SelfModulate = s.defaultModulate;
            }
        } else if (slotData.itemData.Type == inventoryData.allowedType && inventoryData.allowedType != ItemType.Generic)
        {
            foreach (InvSlot s in GetChildren().Cast<InvSlot>())
            {
                //s.highlighted = true;
                //s.SelfModulate = new Color(0, 1, 0, 1);
            }
        }
    }

    public void SetInventoryData(InventoryData inventoryData)
    {
        inventoryData.InventoryUpdated += OnInventoryUpdated;
        InitSlots(inventoryData);
    }

    private void OnInventoryUpdated(InventoryData inventoryData)
    {
        InitSlots(inventoryData);
    }

    public void ClearInventoryData(InventoryData inventoryData)
    {
        inventoryData.InventoryUpdated -= OnInventoryUpdated;

        foreach (InvSlot slot in GetChildren().Cast<InvSlot>())
        {
            slot.SlotInput -= inventoryData.OnSlotClicked;
            slot.SlotHover -= inventoryData.OnSlotEntered;
            slot.SlotExit -= inventoryData.OnSlotExited;
        }
    }
}
