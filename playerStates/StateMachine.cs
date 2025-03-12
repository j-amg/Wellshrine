using Godot;
using System;
using System.Collections.Generic;

public partial class StateMachine : Node
{
	[Export]
	public State current_state;
	private Dictionary<StringName, State> states = new();
	public override void _Ready()
	{
		foreach (Node child in GetChildren())
		{
			if (child is State state)
			{
				states.Add(state.Name, state);
				state.transition += OnChildTransition;
			}
			else
			{
				GD.PrintErr("State Machine contains incompatible child node");
			}
		}
		current_state.Enter();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		current_state.Update(delta);
	}

	private void OnChildTransition(StringName state)
	{
		GD.Print(state);
		if (Owner is Player && Global.Singleton.awaitedAction == state) Global.Singleton.ClosePopUp();
		State new_state = states[state];
		if (new_state != null)
		{
			if (new_state != current_state)
			{
				current_state.Exit();
				new_state.Enter();
				current_state = new_state;
			}
		}
		else
		{
			GD.PrintErr("State does not exist");
		}
	}
}
