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
        PreloadSpawners();
        //if (!Global.Singleton.musicPlayer.Playing) Global.Singleton.PlayMusic();
    }
    public void PreloadSpawners()
    {
        Array<Node> spawners = GetTree().GetNodesInGroup("spawners");

        EnemySpawner s;
        for(int i = 0; i < enemyAmount; i++)
        {
            s = (EnemySpawner)spawners.PickRandom();
            s.EnemySpawned += OnEnemySpawned;
            spawners.Remove(s);
            string TypeToSpawn = Global.Singleton.EnemyTypes.PickRandom();
            s.LoadEnemy(TypeToSpawn);
        }
    }
    private void OnEnemySpawned(Enemy enemy) => enemy.enemyDied += OnEnemyDied;
    private void OnEnemyDied(Enemy enemy) => UpdateObjective();
    public override void UpdateObjective()
    {
        if (GetTree().GetNodesInGroup("enemies").Count - 1 <= 0) CompleteObjective();
    }

}
