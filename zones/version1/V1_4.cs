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
        Global.Singleton.EnterDialogue(new string[] {"You made it, good job.",
        "The road ahead is dangerous, equip yourself with one of the spells from the shrines here.",
         "Once you leave this room you won't be able to change spells,", 
         "So it's a good idea to try them each out to see which one feels right before you leave."}, "???", false);
    }
}
