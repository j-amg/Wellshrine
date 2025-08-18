using Godot;
using System;
using System.Text.RegularExpressions;

public partial class Inv : Control
{

    [Export] public InvContainer invContainer;
    [Export] public InvSlot grabbedSlot;
    [Export] public InvContainer externalInvContainer;
    [Export] public PanelContainer externalInv;
    [Export] public Tooltip tooltip;
    public Chest currentExternalInventoryOwner;
    public InventoryData inventoryData;

    SlotData grabbedSlotData;

    public override void _Ready()
    {
        inventoryData = Global.Singleton.player.inventoryData;
        invContainer.SetInventoryData(inventoryData);
        inventoryData.InventoryInteracted += OnInventoryInteracted;
        inventoryData.InventorySlotHovered += OnInventorySlotHovered;
        inventoryData.InventorySlotExited += OnInventorySlotExited;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (grabbedSlot.Visible) grabbedSlot.GlobalPosition = GetGlobalMousePosition() + new Vector2(5, 5);
    }

    private void OnInventorySlotHovered(InventoryData inventoryData, int index)
    {
        SlotData slot = inventoryData.slotDatas[index];
        if (slot != null)
        {
            tooltip.ShowItem(slot.itemData);
        }
    }

    private void OnInventorySlotExited()
    {
        tooltip.Remove();
    }

    private void OnInventoryInteracted(InventoryData inventoryData, int index, int buttonIndex)
    {
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

    internal void SetExternalInventory(Chest externalInventoryOwner)
    {
        currentExternalInventoryOwner = externalInventoryOwner;
        InventoryData inventoryData = currentExternalInventoryOwner.inventoryData;

        externalInvContainer.SetInventoryData(inventoryData);
        inventoryData.InventoryInteracted += OnInventoryInteracted;
        inventoryData.InventorySlotHovered += OnInventorySlotHovered;
        inventoryData.InventorySlotExited += OnInventorySlotExited;
        externalInv.Show();

    }

    internal void ClearExternalInventory()
    {
        if (currentExternalInventoryOwner != null)
        {
            InventoryData inventoryData = currentExternalInventoryOwner.inventoryData;
            currentExternalInventoryOwner = null;
            externalInvContainer.ClearInventoryData(inventoryData);
            inventoryData.InventoryInteracted -= OnInventoryInteracted;
            externalInv.Hide();
        }
    }

    public override void _ExitTree()
    {
        inventoryData.InventoryInteracted -= OnInventoryInteracted;
        inventoryData.InventorySlotHovered -= OnInventorySlotHovered;
        inventoryData.InventorySlotExited -= OnInventorySlotExited;
        invContainer.DisconnectSlots(inventoryData);
        invContainer.ClearInventoryData(inventoryData);
    }
}
