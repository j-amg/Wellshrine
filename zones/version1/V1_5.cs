using Godot;
using System;

public partial class V1_5 : KillZone
{
    private int currentStep = 0;
    public override void _Ready()
    {
        base._Ready();
        //CallDeferred("Sequence1");
        checkpoints[0].BodyEntered += OnBodyEntered;
        Global.Singleton.DialogueFinished += OnDialogueFinished;
        ZoneOjectiveComplete += OnObjectiveComplete;
    }

    private void OnDialogueFinished()
    {
        if (currentStep == 1) Sequence2();
    }

    private void OnObjectiveComplete(Zone zone)
    {
        if (currentStep == 2) Sequence3();
    }

    private void OnBodyEntered(Node3D body)
    {
        if (body is Player && currentStep == 0)
        {
            if (objectiveComplete) Sequence1Alt(); else Sequence1();
        }
    }

    private async void Sequence3()
    {
        currentStep = 3;
        await ToSignal(GetTree().CreateTimer(.5f), "timeout");
        Global.Singleton.EnterDialogue(new string[] {"The rooms ahead are uncharted,",
        "try to stay alive won't you."}, "???", false);
    }

    private async void Sequence1()
    {
        currentStep = 1;
        await ToSignal(GetTree().CreateTimer(.5f), "timeout");
        Global.Singleton.EnterDialogue(new string[] {"Those poor souls are trapped there...",
        "well... may as well use this as an opportunity to test out that spell you picked up."}, "???", false);
    }

    private async void Sequence1Alt()
    {
        currentStep = 2;
        await ToSignal(GetTree().CreateTimer(.5f), "timeout");
        Global.Singleton.EnterDialogue(new string[] {"Bloodthirsty...", "noted.",
        "The rooms ahead are uncharted,", "try to stay alive won't you."}, "???", false);
    }

    private void Sequence2()
    {
        currentStep = 2;
        Global.Singleton.SendPopUp("[Left Mouse Button] to Attack", "attack");
    }

}
