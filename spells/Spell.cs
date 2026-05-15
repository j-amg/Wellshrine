using System;
using Godot;
using Godot.Collections;


[GlobalClass]
public partial class Spell : Resource
{

	public enum SpellTriggerType
	{
		Held,
		QuickRelease,
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

		float lightningDamageVal = player.attributeData.attributes[AttributeType.FlatLightningDamage].Value;
		if (lightningDamageVal != 0) damageInsts.Add(new DamageInst(new DamageData(DamageType.Lightning, lightningDamageVal, lightningDamageVal), player));
		float fireDamageVal = player.attributeData.attributes[AttributeType.FlatFireDamage].Value;
		if (fireDamageVal != 0) damageInsts.Add(new DamageInst(new DamageData(DamageType.Fire, fireDamageVal, fireDamageVal), player));
		float coldDamageVal = player.attributeData.attributes[AttributeType.FlatColdDamage].Value;
		if (coldDamageVal != 0) damageInsts.Add(new DamageInst(new DamageData(DamageType.Cold, coldDamageVal, coldDamageVal), player));

		GD.Print(damageInsts.Count);

		DamagePackage damagePackage = new(damageInsts, false, this, player);

		if (spell.spellType is SpellType.Projectile) // handle multiple projectiles
		{
			int proj = spell.projectileCount + (int)Global.Singleton.player.attributeData.attributes[AttributeType.ProjectileCount].Value;
			for (int i = 0; i < proj; i++)
			{
				SpellScene spellInst = spellScene.Instantiate<SpellScene>();
				spellInst.hurtBox.damagePackage = damagePackage;
				spellInst.muzzleVelocity *= spellScale;
				spellInst.muzzleVelocity *= player.attributeData.attributes[AttributeType.ProjectileSpeed].Value / 100;
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
