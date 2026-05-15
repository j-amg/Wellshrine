using Godot;
using System;

public partial class ChargeAttackState : AttackState
{

    public enum AttackKeybinds
    {
        LeftMouse,
        spell_slot_1,
        spell_slot_2,
        spell_slot_3,
        spell_slot_4
    }

    ulong startChargeTime;
    float chargeTime;
    float castTime;

    Spell spell;
    public override void Enter()
    {
        spell = owningEntity.spellData.spells[spellIndex];
        if (spell == null) return;
        chargeTime = spell.chargeTime / ((owningEntity.attributeData.attributes[AttributeType.CastSpeed].Value / 100.0f) + 1);
        castTime = spell.castTime / ((owningEntity.attributeData.attributes[AttributeType.CastSpeed].Value / 100.0f) + 1);
        startChargeTime = Time.GetTicksMsec();
    }
    public override void Update(double delta)
    {
        if (spell == null)
        {
            EmitSignal(SignalName.attacktransition, "idle", 0);
            GD.Print("spell is null");
            return;
        }

        float currentChargeAmmount = (Time.GetTicksMsec() - startChargeTime) / (chargeTime * 10); // *10 to translate between to ms to 0 - 100 range

        SignalManager.Singleton.EmitSignal(SignalManager.SignalName.attackChargeUpdated, currentChargeAmmount);

        if (!Input.IsActionPressed(Enum.GetValues<AttackKeybinds>()[spellIndex].ToString()))
        {
            if (Time.GetTicksMsec() >= startChargeTime + chargeTime * 1000)
            {
                Attack(1.0f);
            }
            else
            {
                if (spell.triggerType == Spell.SpellTriggerType.QuickRelease)
                {
                    Attack(Mathf.Max(0.25f, currentChargeAmmount / 100));
                }
                EmitSignal(SignalName.attacktransition, "idle", 0);
            }
            EmitSignal(SignalName.attacktransition, "idle", 0);
        }
    }

    public async void Attack(float chargeAmmount)
    {
        await ToSignal(GetTree().CreateTimer(castTime), "timeout");
        // owningEntity.AttackAnim();
        spell.Cast(owningEntity, chargeAmmount);
        EmitSignal(SignalName.attacktransition, "idle", 0);
    }

    public override void Exit()
    {
        SignalManager.Singleton.EmitSignal(SignalManager.SignalName.attackChargeUpdated, 0);
    }
}
