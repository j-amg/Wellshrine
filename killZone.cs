using Godot;
using System;

public partial class killZone : Zone
{
    public override void UpdateObjective()
    {
        if (GetTree().GetNodesInGroup("enemies").Count > 0) return;
        objectiveComplete = true;
        door.Open();
    }

}
