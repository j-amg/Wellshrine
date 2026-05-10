using Godot;
using System;

public partial class SignalManager : Node
{
	public static SignalManager Singleton => ((SceneTree)Engine.GetMainLoop()).Root.GetNode<SignalManager>("/root/SignalManager");
	[Signal] public delegate void attackChargeUpdatedEventHandler(float value);
	[Signal] public delegate void ZoneEnteredEventHandler(Zone zone);
	[Signal] public delegate void ZoneObjectiveCompleteEventHandler(Zone zone);
	[Signal] public delegate void OpenedInventoryEventHandler();
	[Signal] public delegate void ClosedInventoryEventHandler();
}
