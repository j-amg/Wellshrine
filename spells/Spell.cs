using System;
using Godot;


[GlobalClass]
public partial class Spell : Resource
{ 
	[Export] public float castTime;
	[Export] private AudioStream sound;
	[Export] public PackedScene spellScene;
	public Node3D spell;
	public void Cast(Player player)
	{
		
		if (spellScene == null) return;
		SpellScene spell = spellScene.Instantiate<SpellScene>();
		GD.Print("cast");
		Global.Singleton.AddToScene(spell, player.head.GlobalTransform);


		//SpellScene.InitSpellScene(spellScene, player.head.GlobalTransform, player);


		
	}

	internal void Equip(Player player)
	{
		
	}

	internal void Unequip()
	{
		//spell.CallDeferred("queue_free");
	}
}
