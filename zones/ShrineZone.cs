using Godot;
using System;
using System.Linq;
using Godot.Collections;

public partial class ShrineZone : Zone
{
    [Export] public bool objectiveOverride;
    public override void _Ready()
    {
        base._Ready();
        SetShrineBuffs();
    }

    private void SetShrineBuffs()
    {
        // Array<string> stats = (Array<string>)Global.Singleton.statModifiers.Keys;
        // foreach (Shrine shrine in GetTree().GetNodesInGroup("shrines").Cast<Shrine>())
        // {
        //     shrine.ShrineInteracted += OnShrineInteracted;
        //     if (shrine is BuffShrine s)
        //     {
        //         string stat = stats.PickRandom();
        //         stats.Remove(stat);
        //         s.SetBuff(stat);
        //     }
        // }
    }

    private void OnShrineInteracted()
    {
        GD.Print("ShrineInteracted");
        if (!objectiveOverride) CompleteObjective();
    }
}
