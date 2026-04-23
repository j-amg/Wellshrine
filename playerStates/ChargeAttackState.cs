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
		}
        
        if (Input.IsActionJustReleased(Enum.GetValues<AttackKeybinds>()[spellIndex].ToString()))
        {
            if (Time.GetTicksMsec() >= startChargeTime + spell.castTime * 1000)
            {
                owningEntity.AttackAnim();
                spell.Cast(owningEntity);
                EmitSignal(SignalName.attacktransition, "idle", 0);
            }
            else
            {
                EmitSignal(SignalName.attacktransition, "idle", 0);
            }
        }
	}
}
