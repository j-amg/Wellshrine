using System.Linq;
using Godot;
using Godot.Collections;

public partial class Zone : Node3D
{
    [Signal]
	public delegate void ZoneEnteredEventHandler(Zone zone);
    [Signal]
    public delegate void ZoneOjectiveCompleteEventHandler(Zone zone);
    [Export]
    public string objective;
    [Export]
    public Door door;
    public bool objectiveComplete = false;
    public override void _Ready()
    {
        CallDeferred("emit_signal", SignalName.ZoneEntered, this);
    }

    public virtual void UpdateObjective() => CompleteObjective();
    public void CompleteObjective()
    {
        objective = "Enter the next zone";
        EmitSignal(SignalName.ZoneOjectiveComplete, this);
        door.Open();
    }
}
