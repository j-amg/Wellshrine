using Godot;

public partial class Zone : Node3D
{
    [Signal]
	public delegate void objectiveCompletionEventHandler();
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

    public virtual void UpdateObjective()
    {
        GD.Print("Try update zone");
        objectiveComplete = true;
        door.Open();
        //EmitSignal(SignalName.objectiveCompletion);
    }

    public void Populate(int level)
    {
        //GD.Print("try spawn");
        foreach(EnemySpawner spawner in GetTree().GetNodesInGroup("spawners"))
        {
            //GD.Print("test");
            spawner.Spawn(level);
        } 
    }
}
