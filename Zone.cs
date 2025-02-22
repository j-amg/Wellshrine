using System.Linq;
using Godot;
using Godot.Collections;

public partial class Zone : Node3D
{
    [Export]
    public string objective;
    [Export]
    public Door door;
    public bool objectiveComplete = false;
    public static Zone Initialise(PackedScene scene)
	{
		Zone zone = scene.Instantiate<Zone>();
		return zone;
	}

    public override void _Ready()
    {
        //Populate(Global.Singleton.currentLevel);
        Global.Singleton.Objective = objective;
        //UpdateObjective();

        foreach(EnemySpawner spawner in GetTree().GetNodesInGroup("spawners").Cast<EnemySpawner>())
        {
            //GD.Print("test");
            string TypeToSpawn = (string)Global.Singleton.EnemyTypes.PickRandom();
            spawner.LoadEnemy(TypeToSpawn);
        } 
    }

    public virtual void UpdateObjective()
    {
        GD.Print("Try update zone");
        objectiveComplete = true;
        door.Open();
    }
}
