using Godot;
using System;
using System.Linq;
using Godot.Collections;

public partial class killZone : Zone
{
    [Export]
    public int enemyAmount;
    public override void _Ready()
    {
        base._Ready();
        PreloadSpawners();
    }
    public void PreloadSpawners()
    {
        Array<Node> spawners = GetTree().GetNodesInGroup("spawners");
        GD.Print(spawners.Count);

        EnemySpawner s;
        for(int i = 0; i < enemyAmount; i++)
        {
            s = (EnemySpawner)spawners.PickRandom();
            spawners.Remove(s);
            string TypeToSpawn = Global.Singleton.EnemyTypes.PickRandom();
            s.LoadEnemy(TypeToSpawn);
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
