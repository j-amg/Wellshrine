using System;
using Godot;
using Godot.Collections;


[GlobalClass]
public partial class Spell : Resource
{

	public enum SpellTriggerType
	{
		Held,
		HeldQuickRelease,
		Instant
	}

	[Export] public Array<DamageData> damageDatas = [];
	[Export] public SpellTriggerType triggerType;
	[Export] public float chargeTime; // duration to charge spell
	[Export] public float castTime; // delay before spell is actually cast
	[Export] public float manaCost;
	[Export] private AudioStream sound;
	[Export] public PackedScene spellScene;
	//public Node3D spell;
	public void Cast(Player player, float spellScale = 1.0f)
	{

		if (spellScene == null) return;
		SpellScene spell = spellScene.Instantiate<SpellScene>();

		// convert damageData to damageInsts
		Array<DamageInst> damageInsts = [];
		foreach (DamageData damageData in damageDatas)
		{
			damageInsts.Add(new DamageInst(damageData, player));
		}

		DamagePackage damagePackage = new(damageInsts, false, this, player);

		GD.Print("cast");
		if (spell.spellType is SpellType.Projectile) // handle multiple projectiles
		{
			int proj = spell.projectileCount + (int)Global.Singleton.player.attributeData.Attributes[AttributeType.ProjCount].Value;
			for (int i = 0; i < proj; i++)
			{
				SpellScene spellInst = spellScene.Instantiate<SpellScene>();
				spellInst.hurtBox.damagePackage = damagePackage;
				spellInst.muzzleVelocity *= spellScale;
				spellInst.muzzleVelocity *= player.attributeData.Attributes[AttributeType.ProjSpeed].Value / 100;
				float rotationOffset = (i * spell.projectileSpread) - (proj - 1) * spell.projectileSpread / 2;
				Transform3D t = player.head.GlobalTransform.RotatedLocal(Vector3.Up, Mathf.DegToRad(rotationOffset));
				Global.Singleton.AddToScene(spellInst, t);
			}
		}
		else
		{
			Global.Singleton.AddToScene(spell, player.head.GlobalTransform);
		}
	}
}
