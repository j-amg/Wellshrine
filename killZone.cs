using Godot;
using System;
using System.Linq;

public partial class killZone : Zone
{
    public override void _Ready()
    {
        base._Ready();
        foreach(EnemySpawner spawner in GetTree().GetNodesInGroup("spawners").Cast<EnemySpawner>())
        {
            string TypeToSpawn = (string)Global.Singleton.EnemyTypes.PickRandom();
            spawner.LoadEnemy(TypeToSpawn);
        }
    }
    public override void UpdateObjective()
    {
        if (GetTree().GetNodesInGroup("enemies").Count > 0) return;
        objectiveComplete = true;
        Global.Singleton.Objective = "Enter the next zone";
        Global.Singleton.UpdateHUD();
        door.Open();
    }

}
