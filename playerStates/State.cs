using Godot;
using System;
using System.Net;

public partial class State : Node
{
	[Signal]
	public delegate void transitionEventHandler(StringName stateName);
    public dynamic owner;
    public override void _Ready() => owner = Owner;
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
