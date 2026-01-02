using Godot;
using System;
using Godot.Collections;
using System.Linq;

public partial class EnemySpawner : Node3D
{
    [Signal]
    public delegate void EnemySpawnedEventHandler(Enemy enemy);

    public void Spawn(int count, Array<PackedScene> enemies)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = NavigationServer3D.MapGetRandomPoint(GetWorld3D().NavigationMap, 1, false);
            Enemy loadedEnemy = Enemy.InitEnemy(enemies[GD.RandRange(0, enemies.Count - 1)], 1, spawnPos);
            GetTree().CurrentScene.CallDeferred("add_child", loadedEnemy);
            EmitSignal(SignalName.EnemySpawned, loadedEnemy); 
        }
    }
}