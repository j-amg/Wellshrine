using Godot;
using System;

public partial class V1_6 : KillZone
{
    private int currentStep = 0;
    public override void _Ready()
    {
        base._Ready();
        CallDeferred("Sequence1");

    }

    private async void Sequence1()
    {
        currentStep = 1;
        await ToSignal(GetTree().CreateTimer(.5f), "timeout");
        Global.Singleton.SendPopUp("Tap [Shift] while moving to evade enemy attacks", "slide");
    }

}
