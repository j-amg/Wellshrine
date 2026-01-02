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
    }

    private void OnEnemySpawned(Enemy enemy) => enemy.enemyDied += OnEnemyDied;
    private void OnEnemyDied(Enemy enemy) => UpdateObjective();
    public override void UpdateObjective()
    {
        if (GetTree().GetNodesInGroup("enemies").Count - 1 <= 0) CompleteObjective();
    }

}
