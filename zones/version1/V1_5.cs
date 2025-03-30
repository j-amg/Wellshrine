using Godot;
using System;

public partial class V1_5 : KillZone
{
    private int currentStep = 0;
    public override void _Ready()
    {
        base._Ready();
        checkpoints[0].BodyEntered += OnBodyEntered;
        Global.Singleton.DialogueFinished += OnDialogueFinished;
        ZoneObjectiveComplete += OnObjectiveComplete;
        Global.Singleton.PopUpClosed += OnPopUpClosed;
    }

    private void OnPopUpClosed(string action)
    {
        if (currentStep == 2) Sequence3();
    }

    private void OnDialogueFinished()
    {
        if (currentStep == 1) Sequence2();
    }

    

    private void OnObjectiveComplete(Zone zone)
    {
        Sequence4();
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is Player && currentStep == 0)
        {
            if (objectiveComplete) Sequence1Alt(); else Sequence1();
        }
    }

    private async void Sequence4()
    {
        currentStep = 4;
        await ToSignal(GetTree().CreateTimer(.5f), "timeout");
        Global.Singleton.EnterDialogue(new string[] {"Excellently done, however the dead won't always be found in such a fortuitous situation.", "Continue"}, "???", false);
    }

    private async void Sequence1()
    {
        currentStep = 1;
        await ToSignal(GetTree().CreateTimer(.5f), "timeout");
        Global.Singleton.EnterDialogue(new string[] {"These poor souls are trapped here...",
        "well... may as well use this as an opportunity to test out that spell you picked up."}, "???", false);
    }

    private async void Sequence1Alt()
    {
        currentStep = 2;
        await ToSignal(GetTree().CreateTimer(.5f), "timeout");
        Global.Singleton.EnterDialogue(new string[] {"Bloodthirsty...", "noted.",}, "???", false);
    }

    private void Sequence2()
    {
        currentStep = 2;
        Global.Singleton.SendPopUp("[Right Mouse Button] to Aim", "aim");
    }
    private void Sequence3()
    {
        currentStep = 3;
        Global.Singleton.SendPopUp("[Left Mouse Button] to Attack", "attack");
    }

}
