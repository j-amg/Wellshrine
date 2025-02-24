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

    private async void Setup()
    {
        await ToSignal(door, "ready");
        UpdateObjective();
    }

    public virtual void UpdateObjective()
    {
        GD.Print("Try update zone");
        objectiveComplete = true;
        Global.Singleton.Objective = "Enter the next zone";
        Global.Singleton.UpdateHUD();
        door.Open();
    }
}
