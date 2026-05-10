using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class SpellData : Resource
{
   [Export] public Array<Spell> spells = [null, null, null, null, null];
}