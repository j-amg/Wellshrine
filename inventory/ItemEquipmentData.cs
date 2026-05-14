using System;
using System.Net.NetworkInformation;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ItemEquipmentData : ItemData
{
	[Export] public int level = 1;

	[Export] public ItemAffix _implicit;
	[Export] public ItemAffix prefix;
	[Export] public ItemAffix suffix;
	public void Equip(Player player)
	{
		foreach (ItemAffix affix in new ItemAffix[] { prefix, suffix })
		{
			if (affix != null)
			{
				affix.attributeModifier.Source = this;
				player.attributeData.attributes[affix.TargetType].AddModifier(affix.attributeModifier);
			}

		}
	}

	public void Unequip(Player player)
	{
		foreach (ItemAffix affix in new ItemAffix[] { prefix, suffix })
		{
			if (affix != null)
			{
				player.attributeData.attributes[affix.TargetType].RemoveAllModifiersFromSource(this);
			}
		}
	}
}
