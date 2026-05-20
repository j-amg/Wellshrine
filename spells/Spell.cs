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
	[Export] public bool appliesEffect = false;
	[Export] public EntityEffect[] entityEffects = [];
	public void Cast(Entity entity, float spellScale = 1.0f)
	{
		if (spellScene == null) return;
		bool canCast = entity.IncrementMana(-manaCost);
		if (!canCast) return;
		SpellScene spell = GetAndSetSpellSceneInstance(entity);

		if (spell.spellType is SpellType.Projectile) // handle multiple projectiles
		{
			int proj = spell.projectileCount + (int)entity.attributeData.attributes[AttributeType.ProjectileCount].Value;
			for (int i = 0; i < proj; i++)
			{
				SpellScene spellInst = GetAndSetSpellSceneInstance(entity);
				spellInst.muzzleVelocity *= spellScale;
				spellInst.muzzleVelocity *= 1 + entity.attributeData.attributes[AttributeType.ProjectileSpeed].Value / 100;
				float rotationOffset = (i * spell.projectileSpread) - (proj - 1) * spell.projectileSpread / 2;
				Transform3D t = entity.lookTransform.RotatedLocal(Vector3.Up, Mathf.DegToRad(rotationOffset));
				Global.Singleton.AddToScene(spellInst, t);
			}
		}
		else Global.Singleton.AddToScene(spell, entity.lookTransform);
	}

	private SpellScene GetAndSetSpellSceneInstance(Entity entity)
	{
		SpellScene spell = spellScene.Instantiate<SpellScene>();
		spell.sourceEntity = entity;
		spell.hurtBox.damagePackage = new(damageDatas, false, this, entity);
		spell.appliesEffects = appliesEffect;
		spell.entityEffects = entityEffects;
		return spell;
	}
}
