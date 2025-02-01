using Godot;

public partial class Zone : Node3D
{
    public int enemyLevel = 1;
    public bool objectiveComplete = true;

    public override void _Ready()
    {
        foreach(EnemySpawner spawner in GetTree().GetNodesInGroup("spawners")) spawner.Spawn(enemyLevel);
    }

    public static Zone Initialise(PackedScene scene, int level)
	{
		Zone zone = scene.Instantiate<Zone>();
		zone.enemyLevel = level;
		return zone;
	}
}
