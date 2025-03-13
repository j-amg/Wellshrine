using Godot;
using System;

public partial class EnemySpawner : Node3D
{
    [Signal]
    public delegate void EnemySpawnedEventHandler(Enemy enemy);
    private Enemy LoadedEnemy;
    private int SpawnSpeed = 1;
    public override void _Ready()
    {
        AddToGroup("spawners");
        Spawn();
    }
    public void LoadEnemy(string type)
    {
        PackedScene enemy = GD.Load<PackedScene>("res://enemies/" + type + ".tscn");
        LoadedEnemy = Enemy.InitEnemy(enemy, Global.Singleton.currentLevel, GlobalTransform);
    }

    public async void Spawn()
    {
        await ToSignal(GetTree().CreateTimer(SpawnSpeed), "timeout");
        if (LoadedEnemy == null) return;
        
        var main = GetTree().CurrentScene;
		main.CallDeferred("add_child", LoadedEnemy);
        EmitSignal(SignalName.EnemySpawned, LoadedEnemy);
    }
}

