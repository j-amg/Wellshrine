using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ItemEquipmentData : ItemData
{
	[Export] public int level = 1;
	[Export] public Array<ItemAffix> affixes = [];


	public void Equip(Player player)
	{
		GD.Print("Equip");
		foreach (ItemAffix affix in affixes)
		{
			if (affix != null)
			{
				affix.attributeModifier.Source = this;
				player.attributeData.playerAttributes[affix.TargetType].AddModifier(affix.attributeModifier);
			}

		}
	}

	public void Unequip(Player player)
	{
		foreach (ItemAffix affix in affixes)
		{
			if (affix != null)
			{
			player.attributeData.playerAttributes[affix.TargetType].RemoveAllModifiersFromSource(this);
			}
		}
	}
}
