using Godot;
using System;

public partial class EnemySpawner : Node3D
{
    [Export]
    public PackedScene enemyType;
    [Export]
    public int enemyLevel = 0;
    public override void _Ready()
    {
        Enemy enemy = Enemy.InitEnemy(enemyType, 5, GlobalTransform);
        var main = GetTree().CurrentScene;
		main.CallDeferred("add_child", enemy);
    }
}
