using Godot;
using System;

public partial class V1_1 : Zone
{
    private int currentStep;
    public override void _Ready()
    {
        base._Ready();
        Global.Singleton.PopUpClosed += OnPopUpClosed;
        Global.Singleton.DialogueFinished += OnDialogueFinished;
        CallDeferred("Sequence1");
    }

    private async void Sequence1()
    {
        GD.Print("sequence1");
        //await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        currentStep = 1;
        //GD.Print("step 1");
        Global.Singleton.hud.objectiveLabel.Visible = false;
        Global.Singleton.player.PauseInput();
        Global.Singleton.hud.SetScreen(new Color(0, 0, 0, 1));
        await ToSignal(GetTree().CreateTimer(1f), "timeout");
        Global.Singleton.hud.FadeScreen(new Color(0, 0, 0, 0), .5f);
        await ToSignal(GetTree().CreateTimer(.5f), "timeout");
        Global.Singleton.EnterDialogue(new string[] {"Welcome",
        "Take a second to look around and stretch your legs"}, "???", false);
    }

    private async void Sequence2()
    {
        GD.Print("sequence2");
        currentStep = 2;
        await ToSignal(GetTree().CreateTimer(1f), "timeout");
        //GD.Print("step 2");
        Global.Singleton.SendPopUp("Use Mouse to look around", "look");
    }

    private async void Sequence3()
    {
        GD.Print("sequence3");
        currentStep = 3;
        await ToSignal(GetTree().CreateTimer(1f), "timeout");
        //GD.Print("step 3");
        Global.Singleton.SendPopUp("Use WASD to move", "walk");
    }

    private async void Sequence4()
    {
        GD.Print("sequence4");
        currentStep = 4;
        await ToSignal(GetTree().CreateTimer(1f), "timeout");
        Global.Singleton.EnterDialogue(new string[] {
        "When you feel ready, head through the door"}, "???", false);
    }

    private async void Sequence5()
    {
        currentStep = 4;
        await ToSignal(GetTree().CreateTimer(1f), "timeout");
        CompleteObjective();
        Global.Singleton.hud.objectiveLabel.Visible = true;
    }

    private void OnDialogueFinished()
    {
        GD.Print("dialogue finished");
        if (currentStep == 1) Sequence2();
        if (currentStep == 4) Sequence5();
    }

    private void OnPopUpClosed(string action)
    {
        //GD.Print("action fulfilled");
        GD.Print(action);
        if (action == "look" && currentStep == 2) Sequence3();
        if (action == "walk" && currentStep == 3) Sequence4();
    }
}
