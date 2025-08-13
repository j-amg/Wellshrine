using System;
using Godot;

[GlobalClass]
public partial class ItemData : Resource
{
    [Export] public string name = "";
    [Export] public string description = "";
    [Export] public int level = 1;
    [Export] public bool stackable = false;
    [Export] public Texture2D texture;
    // [Export] public Godot.Collections.Array<Affix> prefixes = [];
    // [Export] public Godot.Collections.Array<Affix> suffixes = [];
}