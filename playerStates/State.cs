using Godot;
using System;
using System.Net;

public partial class State : Node
{
	[Signal]
	public delegate void transitionEventHandler(StringName stateName);
    //public Player player = Global.Singleton.player;
	public Player player;
	public CharacterBody3D parentNode;

    public override async void _Ready()
    {
        await ToSignal(Owner, "ready");
		player = (Player)GetTree().GetNodesInGroup("player")[0];
		parentNode = GetOwner<CharacterBody3D>();
    }
    public virtual void Enter()
	{
		GD.Print(Owner + " Entered " + Name);
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
