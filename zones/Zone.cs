using System.Linq;
using Godot;
using Godot.Collections;

public partial class Zone : Node3D
{
	[Signal]
	public delegate void ZoneEnteredEventHandler(Zone zone);
	[Signal]
	public delegate void ZoneObjectiveCompleteEventHandler(Zone zone);
	[Export]
	public string objective;
	[Export]
	public Door door;

	[Export]
	public EnemySpawner spawner;
	public bool objectiveComplete = false;

	public override void _Ready()
	{
		CallDeferred("emit_signal", SignalName.ZoneEntered, this);
		SpawnDelay();
	
	}

	public async void SpawnDelay()
	{
		await ToSignal(GetTree().CreateTimer(1), "timeout");
		spawner?.Spawn(50, Global.Singleton.enemyArray);
	}

	public virtual void UpdateObjective() => CompleteObjective();
	public void CompleteObjective()
	{
		objectiveComplete = true;
		objective = "Enter the next zone";
		EmitSignal(SignalName.ZoneObjectiveComplete, this);
		//door.Open();
	}

	public virtual void CloseZone()
	{

	}
}
