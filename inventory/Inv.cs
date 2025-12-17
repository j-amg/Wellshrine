using Godot;
using System;
using System.Text.RegularExpressions;

public partial class Inv : Control
{

    [Signal] public delegate void DropSlotDataFromInventoryEventHandler(SlotData slotData);
    [Signal] public delegate void SlotGrabbedEventHandler(SlotData slotData);

    [Export] public PanelContainer playerInventory;
    [Export] public InvContainer invContainer;
    [Export] public InvSlot grabbedSlot;
    [Export] public PanelContainer chestInv;
    [Export] public InvContainer chestInvContainer;
    [Export] public PanelContainer doorChestInv;
    [Export] public InvContainer doorChestInvContainer;
    [Export] public Tooltip tooltip;

    [Export] public InvContainer EquipmentInvContainer1;
    [Export] public InvContainer EquipmentInvContainer2;
    [Export] public InvContainer EquipmentInvContainer3;
    [Export] public InvContainer EquipmentInvContainer4;
    [Export] public InvContainer EquipmentInvContainer5;
    [Export] public InvContainer EquipmentInvContainer6;
    [Export] public InvContainer EquipmentInvContainer7;
    [Export] public InvContainer EquipmentInvContainer8;
    [Export] public InvContainer EquipmentInvContainer9;
    [Export] public InvContainer EquipmentInvContainer10;

    [Export] public Label strLabel;
    [Export] public Label dexLabel;
    [Export] public Label intLabel;

    public Chest currentExternalInventoryOwner;
    public InventoryData[] inventoryDatas;
    public SlotData grabbedSlotData;

    public override void _PhysicsProcess(double delta)
    {
        if (Visible)
        {
            grabbedSlot.GlobalPosition = GetGlobalMousePosition() + new Vector2(5, 5);
        }
    }

    internal void OnAttributeDataUpdated(AttributeData attributeData)
    {
        SetAttributeLabels(attributeData);
    }

    public void SetAttributeLabels(AttributeData attributeData)
    {
        strLabel.Text = attributeData.playerAttributes[AttributeType.Strength].Value.ToString();
        dexLabel.Text = attributeData.playerAttributes[AttributeType.Dexterity].Value.ToString();
        intLabel.Text = attributeData.playerAttributes[AttributeType.Intelligence].Value.ToString();
    }

    public void SetPlayerInventoryData(InventoryData[] PlayerInventoryData)
    {
        inventoryDatas = PlayerInventoryData;
        invContainer.SetInventoryData(PlayerInventoryData[0]);
        EquipmentInvContainer1.SetInventoryData(PlayerInventoryData[1]);
        EquipmentInvContainer2.SetInventoryData(PlayerInventoryData[2]);
        EquipmentInvContainer3.SetInventoryData(PlayerInventoryData[3]);
        EquipmentInvContainer4.SetInventoryData(PlayerInventoryData[4]);
        EquipmentInvContainer5.SetInventoryData(PlayerInventoryData[5]);
        EquipmentInvContainer6.SetInventoryData(PlayerInventoryData[6]);
        EquipmentInvContainer7.SetInventoryData(PlayerInventoryData[7]);
        EquipmentInvContainer8.SetInventoryData(PlayerInventoryData[8]);
        EquipmentInvContainer9.SetInventoryData(PlayerInventoryData[9]);
        EquipmentInvContainer10.SetInventoryData(PlayerInventoryData[10]);

        foreach (InventoryData id in PlayerInventoryData)
        {
            id.InventoryInteracted += OnInventoryInteracted;
            id.InventorySlotHovered += OnInventorySlotHovered;
            id.InventorySlotExited += OnInventorySlotExited;
        }
    }

    public void SetExternalInventory(Chest externalInventoryOwner)
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


    private void OnInventorySlotHovered(InventoryData inventoryData, int index, InvSlot invSlot)
    {
        SlotData slot = inventoryData.slotDatas[index];
        
        if (slot != null)
        {
            invSlot.SelfModulate = new Color(0, 1, 0, 1);
            tooltip.headerContainer.Modulate = slot.itemData.rarity != 0 ? new Color(0.753f, 0.673f, 0.0f) : new Color(1, 1, 1);
            tooltip.SetItem(slot.itemData);
            tooltip.GlobalPosition = invSlot.GlobalPosition + new Vector2(invSlot.Size.X/2, invSlot.Size.Y/2) + new Vector2(-(tooltip.Size.X/2), -(tooltip.Size.Y + invSlot.Size.Y/2));
            tooltip.GlobalPosition = tooltip.FindSpawnPosition(invSlot);
            tooltip.Show();
        }

        if (grabbedSlotData != null)
        {
            //GD.Print("cant merge");
            if (!inventoryData.IsItemAllowed(grabbedSlotData.itemData) || slot != null && !grabbedSlotData.CanMergeWith(slot))
            {
                invSlot.SelfModulate = new Color(1, 0, 0, 1);
            }
            else
            {
                invSlot.SelfModulate = new Color(0, 1, 0, 1);
            } 
        }
    }

    private void OnInventorySlotExited(InventoryData inventoryData, int index, InvSlot invSlot)
    {
        if (!invSlot.highlighted)
        {
            invSlot.SelfModulate = new Color(1,1,1,1);
        }
        tooltip.Hide();
    }

    private void OnInventoryInteracted(InventoryData inventoryData, int index, int buttonIndex, InvSlot invSlot)
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
        EmitSignal(SignalName.SlotGrabbed, grabbedSlotData);
        if (grabbedSlotData != null)
        {
            grabbedSlot.Show();
            grabbedSlot.SetSlotData(grabbedSlotData);
        }
        else grabbedSlot.Hide();
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
        foreach (InventoryData id in inventoryDatas)
        {
            id.InventoryInteracted -= OnInventoryInteracted;
            id.InventorySlotHovered -= OnInventorySlotHovered;
            id.InventorySlotExited -= OnInventorySlotExited;
        }

        invContainer.ClearInventoryData(inventoryDatas[0]);
        EquipmentInvContainer1.ClearInventoryData(inventoryDatas[1]);
        EquipmentInvContainer2.ClearInventoryData(inventoryDatas[2]);
        EquipmentInvContainer3.ClearInventoryData(inventoryDatas[3]);
        EquipmentInvContainer4.ClearInventoryData(inventoryDatas[4]);
        EquipmentInvContainer5.ClearInventoryData(inventoryDatas[5]);
        EquipmentInvContainer6.ClearInventoryData(inventoryDatas[6]);
        EquipmentInvContainer7.ClearInventoryData(inventoryDatas[7]);
        EquipmentInvContainer8.ClearInventoryData(inventoryDatas[8]);
        EquipmentInvContainer9.ClearInventoryData(inventoryDatas[9]);
        EquipmentInvContainer10.ClearInventoryData(inventoryDatas[10]);

    }
}
