using Godot;
using System;
using System.Net;

public partial class State : Node
{
	[Signal]
	public delegate void transitionEventHandler(StringName stateName);
    //public Player player = Global.Singleton.player;
	public Player player;

    public override async void _Ready()
    {
        await ToSignal(Owner, "ready");
		player = Owner as Player;
    }
    public virtual void Enter()
	{
		GD.Print("Entered " + Name);
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
