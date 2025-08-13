using Godot;
using System;
using System.Text.RegularExpressions;

public partial class Inv : Control
{

    [Export] public InvContainer invContainer;
    [Export] public InvSlot grabbedSlot;

    SlotData grabbedSlotData;

    public override void _Ready()
    {
        InventoryData inventoryData = Global.Singleton.player.inventoryData;
        invContainer.SetInventoryData(inventoryData);
        inventoryData.InventoryInteracted += OnInventoryInteracted;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (grabbedSlot.Visible) grabbedSlot.GlobalPosition = GetGlobalMousePosition() + new Vector2(5,5);
    }

    private void OnInventoryInteracted(InventoryData inventoryData, int index, int buttonIndex)
    {
        //GD.Print(inventoryData.ToString(), index);

        //grabbedSlotData ??= inventoryData.GrabSlotData(index);

        if (buttonIndex == 1) // left click
        {
            if (grabbedSlotData != null)
            {
                grabbedSlotData = inventoryData.DropSlotData(index, grabbedSlotData);
            }
            else
            {
                grabbedSlotData = inventoryData.GrabSlotData(index);
            }
        }
        else if (buttonIndex == 2) // right click
        {
            if (grabbedSlotData != null)
            {
                grabbedSlotData = inventoryData.DropSingleSlotData(index, grabbedSlotData);
            }
            else
            {
                // use item
            }
        }


        GD.Print(grabbedSlotData);
        UpdateGrabbedSlot();
    }

    private void UpdateGrabbedSlot()
    {
        if (grabbedSlotData != null)
        {
            grabbedSlot.Show();
            grabbedSlot.SetSlotData(grabbedSlotData);
        }
        else grabbedSlot.Hide();
    }
}
