using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ItemSpellData : ItemData
{
	[Export] public int level = 1;
	[Export] public Spell spell;


	public void Equip(Player player, SpellInventoryData inventoryData)
	{
		GD.Print("Equip Spell");
		player.equippedSpells.SetValue(spell, inventoryData.spellSlotIndex);
	}

	public void Unequip(Player player, SpellInventoryData inventoryData)
	{
		player.equippedSpells.SetValue(null, inventoryData.spellSlotIndex);
	}
}
