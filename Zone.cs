using Godot;

public partial class Zone : Node3D
{
    public int enemyLevel = 2;
    public bool objectiveComplete = true;
    public static Zone Initialise(PackedScene scene, int level)
	{
		Zone zone = scene.Instantiate<Zone>();
		zone.enemyLevel = level;
		return zone;
	}

    public void Populate()
    {
        GD.Print("try spawn");
        foreach(EnemySpawner spawner in GetTree().GetNodesInGroup("spawners"))
        {
            GD.Print("test");
            spawner.Spawn(enemyLevel);
        } 
    }
}
