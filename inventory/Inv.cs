using Godot;
using System;
using System.Text.RegularExpressions;

public partial class Inv : Control
{

    [Signal] public delegate void DropSlotDataFromInventoryEventHandler(SlotData slotData);

    [Export] public InvContainer invContainer;
    [Export] public InvSlot grabbedSlot;
    [Export] public InvContainer chestInvContainer;
    [Export] public PanelContainer chestInv;
    [Export] public PanelContainer doorChestInv;
    [Export] public InvContainer doorChestInvContainer;
    [Export] public Tooltip tooltip;
    public Chest currentExternalInventoryOwner;
    public InventoryData inventoryData;
    public SlotData grabbedSlotData;

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
        if (Visible)
        {
            tooltip.GlobalPosition = GetGlobalMousePosition() + new Vector2(5, 5);
            grabbedSlot.GlobalPosition = GetGlobalMousePosition() + new Vector2(5, 5);
        }    
    }

    private void OnInventorySlotHovered(InventoryData inventoryData, int index)
    {
        SlotData slot = inventoryData.slotDatas[index];
        if (slot != null)
        {
            tooltip.SetItem(slot.itemData);
            tooltip.Show();
        }
    }

    private void OnInventorySlotExited(InventoryData inventoryData, int index)
    {
        tooltip.Hide();
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

        inventoryData.InventoryInteracted += OnInventoryInteracted;
        inventoryData.InventorySlotHovered += OnInventorySlotHovered;
        inventoryData.InventorySlotExited += OnInventorySlotExited;

        switch (externalInventoryOwner.chestType)
        {
            case "chest":
                chestInvContainer.SetInventoryData(inventoryData);
                chestInv.Show();
                break;
            case "doorchest":
                GD.Print("show door inv");
                doorChestInvContainer.SetInventoryData(inventoryData);
                doorChestInv.Show();
                break;
        }
    }

    internal void ClearExternalInventory()
    {
        if (currentExternalInventoryOwner != null)
        {
            InventoryData inventoryData = currentExternalInventoryOwner.inventoryData;

            switch (currentExternalInventoryOwner.chestType)
            {
                case "chest":
                    chestInvContainer.ClearInventoryData(inventoryData);
                    chestInv.Hide();
                    break;
                case "doorchest":
                    doorChestInvContainer.ClearInventoryData(inventoryData);
                    doorChestInv.Hide();
                    break;
            }

            inventoryData.InventoryInteracted -= OnInventoryInteracted;
            currentExternalInventoryOwner = null;
        }
    }

    public override void _GuiInput(InputEvent @event)
    {
        //GD.Print("Mouse entered slot");
        if (@event is InputEventMouseButton mbe && mbe.Pressed && mbe.ButtonIndex == MouseButton.Left)
        {
            if (grabbedSlotData != null)
            {
                EmitSignal(SignalName.DropSlotDataFromInventory, grabbedSlotData);
                grabbedSlotData = null;
                UpdateGrabbedSlot();
            }

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
