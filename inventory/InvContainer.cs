using Godot;
using Godot.Collections;
using System;

public partial class InvContainer : GridContainer
{

    public PackedScene slotScene;
    private int slotSize = 32;

    public override void _Ready()
    {
        slotScene = GD.Load<PackedScene>("res://inventory/inv_slot.tscn");
    }

    public void InitSlots(InventoryData inventoryData)
    {
        //dynamic sizing

        // int colNum = (int)MathF.Floor(Size.X / slotSize);
        // int rowNum = (int)MathF.Floor(Size.Y / slotSize);
        // int numSlots = colNum * rowNum;
        // Columns = colNum;

        foreach (Node n in GetChildren()) { n.QueueFree(); }

        foreach (SlotData slotdata in inventoryData.slotDatas)
        {
            InvSlot slot = slotScene.Instantiate<InvSlot>();
            AddChild(slot);

            if (slotdata != null) slot.SetSlotData(slotdata);

            slot.SlotInput += inventoryData.OnSlotClicked;

            // s.MouseExited += OnMouseExitedSlot;
            // s.MouseEntered += () => OnMouseEnteredSlot(s);
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


    // private void OnMouseExitedSlot()
    // {
    //     GD.Print("exited");
    // }

    // private void OnMouseEnteredSlot(InvSlot slot)
    // {
    //     GD.Print("Mouse entered " + slot.Name);
    // }
}
