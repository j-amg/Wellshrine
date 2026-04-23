using Godot;
using System;
using System.Net;

public partial class AttackState : State
{
	[Signal]
	public delegate void attacktransitionEventHandler(StringName stateName, int spellIndex);

	public int spellIndex;

}
