using Godot;
using System;

public partial class DoorInventory : PanelContainer
{
    [Export] public Button openLevelButton;
    public override void _Ready() => openLevelButton.Pressed += OpenLevelButtonPressed;
    private void OpenLevelButtonPressed()
    {
        InventoryData inventoryData = Global.Singleton.playerZone.doorChest.inventoryData;
        SlotData slotData = inventoryData.slotDatas[0];
        if (slotData == null)
        {
            throw new Exception("No key in key slot");
        } 
        if (slotData.itemData is ItemKeyData key)
        {
            GD.Print("Open path: " + key.zonePath);
            Global.Singleton.playerZone?.door.SetDestination(key.zonePath);
            inventoryData.ConsumeSlotData(0);
            Global.Singleton.ToggleInv();
            Global.Singleton.playerZone.door.Open();
            
        } else throw new Exception("incorrect item type in key slot");
    }
}
