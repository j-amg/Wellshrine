using Godot;
using System;
using System.Collections.Generic;

public partial class AttackStateMachine : StateMachine
{

	public Dictionary<StringName, AttackState> attackStates = [];
	public override void _Ready()
	{

		SignalManager.Singleton.OpenedInventory += OnInventoryOpened;
		SignalManager.Singleton.ClosedInventory += OnInventoryClosed;
		foreach (Node child in GetChildren())
		{
			if (child is AttackState state)
			{
				attackStates.Add(state.Name, state);
				state.attacktransition += OnChildTransition;
			}
			else GD.PrintErr("State Machine contains incompatible child node");
		}
		current_state.Enter();
	}

    private void OnInventoryClosed() => SetPhysicsProcess(true);

    private void OnInventoryOpened() => SetPhysicsProcess(false);

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta) => current_state.Update(delta);

	private void OnChildTransition(StringName state, int spellIndex)
	{
		AttackState new_state = attackStates[state];
		if (new_state != null)
		{
			if (new_state != current_state)
			{
				current_state.Exit();
				new_state.spellIndex = spellIndex;
				new_state.Enter();
				current_state = new_state;
			}
		}
		else GD.PrintErr("State does not exist");
	}

	public override void _ExitTree()
	{
		SignalManager.Singleton.OpenedInventory -= OnInventoryOpened;
		SignalManager.Singleton.ClosedInventory -= OnInventoryClosed;
	}
}
