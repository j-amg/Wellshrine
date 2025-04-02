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
    public Checkpoint[] checkpoints;
    [Export]
    public bool increasesLevel = false;
    [Export]
    public string zoneValueOverride;
    [Export]
    public bool hideZoneLabel;
    public bool objectiveComplete = false;

    public override void _Ready()
    {
        CallDeferred("emit_signal", SignalName.ZoneEntered, this);
    }

    public virtual void UpdateObjective() => CompleteObjective();
    public void CompleteObjective()
    {
        objectiveComplete = true;
        objective = "Enter the next zone";
        EmitSignal(SignalName.ZoneObjectiveComplete, this);
        door.Open();
    }

    public virtual void CloseZone()
    {

    }
}
