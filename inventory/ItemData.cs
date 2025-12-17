using System;
using Godot;

public enum ItemType
{
    Generic,
    Key,
    Spell,
    Helmet,
    Armour,
    Ring,
    Amulet,
    Boot,
}

[GlobalClass]
public partial class ItemData : Resource
{
    [Export] public string name = "";
    [Export] public string description = "";
    [Export] public bool stackable = false;
    [Export] public Texture2D texture;
    [Export] public ItemType Type;
    [Export] public int rarity = 0; // 0 Common, 1 rare

}