using Godot;
using System;

public partial class EnemySpawner : Node3D
{
    [Export]
    public string enemyName = "chaser";
    private PackedScene loadedEnemy;
    public override void _Ready()
    {
        AddToGroup("spawners");
        loadedEnemy = ResourceLoader.Load<PackedScene>("res://enemies/" + enemyName + ".tscn");
    }

    public void Spawn(int level)
    {
        Enemy enemy = Enemy.InitEnemy(loadedEnemy, level, GlobalTransform);
        var main = GetTree().CurrentScene;
		main.CallDeferred("add_child", enemy);
    }
}
