using System;
using Godot;
public partial class Item(string type, int level) : Resource
{
    public string type = type;
    public int level = level;

    public Godot.Collections.Array<Affix> prefixes = [];
    public Godot.Collections.Array<Affix> suffixes = [];
}