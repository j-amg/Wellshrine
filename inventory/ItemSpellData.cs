using System;
using Godot;
using Godot.Collections;

public enum SpellType
{
    Projectile,
    Buff,
    Target,
	Effect
}

[GlobalClass]
public partial class ItemSpellData : ItemData
{
	[Export] public int level = 1;
	[Export] public Spell spell;


	public void Equip(Player player, SpellInventoryData inventoryData)
	{
		player.spellData.spells[inventoryData.spellSlotIndex] = spell;
		spell.Equip(player);
	}

	public void Unequip(Player player, SpellInventoryData inventoryData)
	{
		player.spellData.spells[inventoryData.spellSlotIndex] = null;
		spell.Unequip();
	}
}
