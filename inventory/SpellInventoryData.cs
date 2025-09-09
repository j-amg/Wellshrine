using Godot;
using System;

public partial class SpellInventoryData : InventoryData
{
	[Export] public int spellSlotIndex;
	public override SlotData GrabSlotData(int index)
	{
		SlotData slotData = slotDatas[index];

		if (slotData.itemData is ItemSpellData spell) spell.Unequip(Global.Singleton.player, this);

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

	public override SlotData DropSlotData(int index, SlotData grabbedSlotData)
	{

		if (allowedType != 0 && grabbedSlotData.itemData.Type != allowedType) return grabbedSlotData;

		SlotData currentSlotData = slotDatas[index];

		if (grabbedSlotData.itemData is ItemSpellData spell) spell.Equip(Global.Singleton.player, this);
		

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
}
