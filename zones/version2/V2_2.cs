using Godot;
using System;

public partial class V2_2 : Zone
{
    private int currentStep;
    public override void _Ready()
    {
        base._Ready();
        CallDeferred("Sequence1");
    }

    private async void Sequence1()
    {
        GD.Print("sequence1");
        currentStep = 1;
        await ToSignal(GetTree().CreateTimer(.5f), "timeout");
        Global.Singleton.EnterDialogue(new string[] {"Welcome stranger",
        "The path ahead is dangerous, equip yourself with one of the spells here before you continue.", "If you need assistance speak with me, I will make myself available where fitting.", "Now go."}, "ShrineKeeper", false);
    }
}
