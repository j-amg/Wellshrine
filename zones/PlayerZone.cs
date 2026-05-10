using Godot;
using System;

public partial class PlayerZone : Zone
{
	[Export] public Chest doorChest;
	[Export] public string CurrentDoorDestinationPath;
	public override void _Ready()
	{
		base._Ready();
	}
}
