using Godot;
using System;
using System.Linq;
using Godot.Collections;

public partial class shrineZone : Zone
{

    public Array<string> buffTypes = new() {
        "Increased Damage",
        "Increased Critical Damage",
        "Increased Move Speed",
        "Increased Stun Duration",
        "Reduced Recharge",
        "Increased Max Health"};

    public override void _Ready()
    {
        base._Ready();
        SetShrineBuffs();
    }

    private void SetShrineBuffs()
    {
        Array<string> clonedArray = buffTypes;
        foreach (Shrine shrine in GetTree().GetNodesInGroup("shrines").Cast<Shrine>())
        {
            if (shrine is BuffShrine s)
            {
                string selectedBuff = clonedArray.PickRandom();
                clonedArray.Remove(selectedBuff);
                s.SetBuff(selectedBuff);
            }
        }
    }
    public override void UpdateObjective()
    {
        objectiveComplete = true;
        Global.Singleton.Objective = "Enter the next zone";
        Global.Singleton.UpdateHUD();
        door.Open();
    }
}
