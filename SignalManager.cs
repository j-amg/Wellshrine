using Godot;
using System;

public partial class SignalManager : Node
{
	public static SignalManager Singleton => ((SceneTree)Engine.GetMainLoop()).Root.GetNode<SignalManager>("/root/SignalManager");

	[Signal]
	public delegate void attackChargeUpdatedEventHandler(float value);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
