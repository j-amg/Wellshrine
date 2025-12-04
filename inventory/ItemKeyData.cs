using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ItemKeyData : ItemData
{
	[Export] public int level = 1;
	[Export] public string zonePath;
}
