using Godot;
using System;
using System.Net;

public partial class State : Node
{
	[Signal]
	public delegate void transitionEventHandler(StringName stateName);

	public dynamic owningEntity;

	public override void _Ready() => owningEntity = Owner;

    public virtual void Enter()
	{
		return;
	}

	public virtual void Exit()
	{
		return;
	}

	public virtual void Update(double delta)
	{
		return;
	}
}
