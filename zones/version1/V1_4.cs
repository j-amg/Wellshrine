using Godot;
using System;

public partial class V1_4 : ShrineZone
{
    private int currentStep;
    public override void _Ready()
    {
        base._Ready();
        CallDeferred("Sequence1");
    }

    private async void Sequence1()
    {
        currentStep = 1;
        await ToSignal(GetTree().CreateTimer(.5f), "timeout");
        Global.Singleton.EnterDialogue(new string[] {"You made it",
        "The road ahead is dangerous, choose one of the spells from the shrines here"}, "???", false);
    }

}
