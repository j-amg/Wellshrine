using Godot;
using System;
using System.Linq;
using Godot.Collections;

public partial class KillZone : Zone
{
    [Export]
    public int enemyAmount;
    public override void _Ready()
    {
        base._Ready();
    }

}
