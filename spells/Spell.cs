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
		if (spell.spellType is SpellType.Projectile) // handle multiple projectiles
		{
			int proj = spell.projectileCount + (int)Global.Singleton.player.attributeData.playerAttributes[AttributeType.ProjCount].Value;
			for (int i = 0; i < proj; i++)
			{
				SpellScene spellInst = spellScene.Instantiate<SpellScene>();
				float rotationOffset = (i * spell.projectileSpread) - (proj - 1) * spell.projectileSpread/2; 
				Transform3D t = player.head.GlobalTransform.RotatedLocal(Vector3.Up, Mathf.DegToRad(rotationOffset));
				Global.Singleton.AddToScene(spellInst, t);
			}
		}
		else
		{
			Global.Singleton.AddToScene(spell, player.head.GlobalTransform);
		}	
	}

	internal void Equip(Player player)
	{
		
	}

	internal void Unequip()
	{
		//spell.CallDeferred("queue_free");
	}
}
