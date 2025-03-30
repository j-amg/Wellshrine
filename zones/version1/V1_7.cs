using Godot;
using System;

public partial class V1_7 : ShrineZone
{
    private int currentStep = 0;
    public override void _Ready()
    {
        base._Ready();
        CallDeferred("Sequence1");
        Global.Singleton.currentZone.ZoneObjectiveComplete += OnObjectiveCompleted;
    }

    private void OnObjectiveCompleted(Zone zone)
    {
        if (currentStep == 1) Sequence2();
    }

    private async void Sequence2()
    {
        currentStep = 2;
        await ToSignal(GetTree().CreateTimer(.5f), "timeout");
        Global.Singleton.EnterDialogue(new string[] {"If you haven't already, make sure that you activate the wellshrine to the left of the room in the case that you suffered some...", "ill-fortune... in the last room.",
        "There is a wellshrine in each Amplichamber and they will be vital for your continued survival.", "In any matter, this is as far ahead as I have charted, and so is where I must leave you.", "Good Luck."}, "???", false);
    }

    private async void Sequence1()
    {
        currentStep = 1;
        await ToSignal(GetTree().CreateTimer(.5f), "timeout");
        Global.Singleton.EnterDialogue(new string[] {"You found an Amplichamber,",  "if they're still functional you will want to activate one of the shrines here.",
        "They have a range of effects, and won't always be the same.", "You won't be able to change your selection so choose carefully."}, "???", false);
    }





}
