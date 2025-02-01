using Godot;
using System;

public partial class EnemySpawner : Node3D
{
    [Export]
    public PackedScene enemyType;
    public override void _Ready()
    {
        AddToGroup("spawners");
    }

    public void Spawn(int level)
    {
        Enemy enemy = Enemy.InitEnemy(enemyType, level, GlobalTransform);
        var main = GetTree().CurrentScene;
		main.CallDeferred("add_child", enemy);
    }
}
