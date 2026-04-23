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

    Spell spell;
    public override void Enter()
    {
        spell = owningEntity.spellData.spells[spellIndex];
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

        float chargeAmmount = (Time.GetTicksMsec() - startChargeTime) / (spell.castTime * 10); // *10 to translate between to ms to 0.0 - 1.0 range

        SignalManager.Singleton.EmitSignal(SignalManager.SignalName.attackChargeUpdated, chargeAmmount);
        
        if (Input.IsActionJustReleased(Enum.GetValues<AttackKeybinds>()[spellIndex].ToString()))
        {
            if (Time.GetTicksMsec() >= startChargeTime + spell.castTime * 1000)
            {
                Attack(1.0f);
            }
            else
            {
                if (spell.triggerType == Spell.SpellTriggerType.HeldQuickRelease)
                {
                    Attack(chargeAmmount);
                }
                EmitSignal(SignalName.attacktransition, "idle", 0);
            }
        }
	}

    public async void Attack(float chargeAmmount)
    {
        await ToSignal(GetTree().CreateTimer(spell.castTime), "timeout");
        owningEntity.AttackAnim();
        spell.Cast(owningEntity, chargeAmmount);
        EmitSignal(SignalName.attacktransition, "idle", 0);
    }

    public override void Exit()
    {
        SignalManager.Singleton.EmitSignal(SignalManager.SignalName.attackChargeUpdated, 0);
    }
}
