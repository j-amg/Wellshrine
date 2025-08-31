using Godot;
using System;

public partial class EnemySpawner : Node3D
{
    [Signal]
    public delegate void EnemySpawnedEventHandler(Enemy enemy);
    [Export]
    public int typeOverride = -1;
    private Enemy LoadedEnemy;
    private int SpawnSpeed = 1;
    public override void _Ready()
    {
        AddToGroup("spawners");
        Spawn();
    }
    public void LoadEnemy(int type)
    {
        if (typeOverride != -1)
        { LoadedEnemy = Enemy.InitEnemy(Global.Singleton.enemyArray[typeOverride], Global.Singleton.currentLevel, GlobalTransform);
        } else LoadedEnemy = Enemy.InitEnemy(Global.Singleton.enemyArray[type], Global.Singleton.currentLevel, GlobalTransform);
    }

    public async void Spawn()
    {
        await ToSignal(GetTree().CreateTimer(SpawnSpeed), "timeout");
        if (LoadedEnemy == null) return;
		GetTree().CurrentScene.CallDeferred("add_child", LoadedEnemy);
        EmitSignal(SignalName.EnemySpawned, LoadedEnemy);
    }
}

