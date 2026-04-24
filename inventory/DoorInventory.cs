using Godot;
using System;

public partial class DoorInventory : PanelContainer
{
    [Export] public Button openLevelButton;
    public override void _Ready() => openLevelButton.Pressed += OpenLevelButtonPressed;
    private void OpenLevelButtonPressed()
    {
        InventoryData inventoryData = Global.Singleton.playerZone.doorChest.inventoryData;

        bool foundKeyInSlot = false;
        bool isValidCombination = true;
        ItemKeyData key = null;

        foreach (SlotData slotData in inventoryData.slotDatas)
        {
            if (slotData == null) continue;
            if (slotData.itemData is ItemKeyData k)
            {
                if (foundKeyInSlot)
                {
                    isValidCombination = false;
                    GD.Print("multiple keys found");
                    break; 
                }
                else
                {
                    key = k;
                    foundKeyInSlot = true;
                }

            }
        }

        if (isValidCombination)
        {
            GD.Print("valid combination");
            foreach (SlotData slotData2 in inventoryData.slotDatas)
            {
                if (slotData2 != null)
                {
                    inventoryData.ConsumeSlotData(0);
                }
            }
            if (key == null)
            {
                GD.Print("invalid key");
                return;
            }
            GD.Print("Open path: " + key.zonePath);
            Global.Singleton.ToggleInv();
            Global.Singleton.playerZone.door.Open();
            Global.Singleton.playerZone.door.SetDestination(key.zonePath);
        }
    }
}
