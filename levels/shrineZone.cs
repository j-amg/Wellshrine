using Godot;
using System;

public partial class shrineZone : Zone
{
    public override void UpdateObjective()
    {
        objectiveComplete = true;
        Global.Singleton.Objective = "Enter the next zone";
        Global.Singleton.UpdateHUD();
        door.Open();
    }
}
