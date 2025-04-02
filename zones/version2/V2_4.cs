using Godot;
using System;

public partial class V2_4 : KillZone
{
    private int currentStep = 0;

    public override void _Ready()
    {
        base._Ready();
        ZoneObjectiveComplete += OnObjectiveComplete;
    }

    private void OnObjectiveComplete(Zone zone)
    {
        if (currentStep == 0) Sequence1();

    }

    private async void Sequence1()
    {
        currentStep = 1;
        await ToSignal(GetTree().CreateTimer(.5f), "timeout");
        Global.Singleton.SendPopUp("Press [Spacebar] while holding [Shift] to Dash", "dash");
    }

    public override void CloseZone()
    {
        ZoneObjectiveComplete -= OnObjectiveComplete;
    }


}
