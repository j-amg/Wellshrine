using Godot;
using System;

public partial class EnemySpawner : Node3D
{
    private PackedScene LoadedEnemy;
    private int SpawnSpeed = 1;
    public override void _Ready()
    {
        AddToGroup("spawners");
        Spawn();
    }
    public void LoadEnemy(string type) => LoadedEnemy = GD.Load<PackedScene>("res://enemies/" + type + ".tscn");

    public async void Spawn()
    {
        await ToSignal(GetTree().CreateTimer(SpawnSpeed), "timeout");
        if (LoadedEnemy == null) return;
        Enemy enemy = Enemy.InitEnemy(LoadedEnemy, Global.Singleton.currentLevel, GlobalTransform);
        var main = GetTree().CurrentScene;
		main.CallDeferred("add_child", enemy);
    }
}

