using Godot;
using System;

public partial class V2_1 : Zone
{
    private int currentStep;
    public override void _Ready()
    {
        base._Ready();
        Global.Singleton.PopUpClosed += OnPopUpClosed;
        CallDeferred("Sequence1");
    }


    private async void Sequence1()
    {
        GD.Print("sequence1");
        currentStep = 1;
        Global.Singleton.hud.SetScreen(new Color(0, 0, 0, 1));
        await ToSignal(GetTree().CreateTimer(1f), "timeout");
        Global.Singleton.hud.FadeScreen(new Color(0, 0, 0, 0), .5f);
        await ToSignal(GetTree().CreateTimer(.5f), "timeout");
        //GD.Print("step 2");
        Global.Singleton.SendPopUp("[Mouse] to look around", "look");
    }

    private async void Sequence2()
    {
        GD.Print("sequence3");
        currentStep = 2;
        await ToSignal(GetTree().CreateTimer(1f), "timeout");
        //GD.Print("step 3");
        Global.Singleton.SendPopUp("WASD to move", "walk");
    }

    private async void Sequence3()
    {
        GD.Print("sequence3");
        currentStep = 4;
        await ToSignal(GetTree().CreateTimer(1f), "timeout");
        CompleteObjective();
    }

    private void OnPopUpClosed(string action)
    {
        GD.Print(action);
        if (action == "look" && currentStep == 1) Sequence2();
        if (action == "walk" && currentStep == 2) Sequence3();
    }
}
