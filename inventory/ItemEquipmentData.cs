using System;
using Godot;

[GlobalClass]
public partial class ItemEquipmentData : ItemData
{
    //[Export] public string type = "";
    //[Export] public int level = 1;
    [Export] public Godot.Collections.Array<AttributeModifier> attributeModifiers = [];


    public void Equip(Player player)
    {
        foreach (AttributeModifier mod in attributeModifiers)
        {
            
        }
    }
}