using Godot;
using System;

public partial class IdleAttackState : AttackState
{
    public override void Update(double delta)
	{
        if (Input.IsActionJustPressed("LeftMouse")) EmitSignal(SignalName.attacktransition, "charge", 0);
		if (Input.IsActionJustPressed("spell_slot_1")) EmitSignal(SignalName.attacktransition, "charge", 1);
		if (Input.IsActionJustPressed("spell_slot_2")) EmitSignal(SignalName.attacktransition, "charge", 2);
		if (Input.IsActionJustPressed("spell_slot_3")) EmitSignal(SignalName.attacktransition, "charge", 3);
		if (Input.IsActionJustPressed("spell_slot_4")) EmitSignal(SignalName.attacktransition, "charge", 4);
	}
}