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
        Global.Singleton.SendPopUp("[Mouse] to look around", "look");
    }

    private async void Sequence2()
    {
        GD.Print("sequence3");
        await ToSignal(GetTree().CreateTimer(.5f), "timeout");
        currentStep = 2;
        Global.Singleton.SendPopUp("WASD to move", "walk");
    }

    private async void Sequence3()
    {
        await ToSignal(GetTree().CreateTimer(.5f), "timeout");
        currentStep = 4;
        CompleteObjective();
    }

    private void OnPopUpClosed(string action)
    {
        GD.Print(action);
        if (currentStep == 1) Sequence2();
        if (currentStep == 2) Sequence3();
    }

    public override void CloseZone()
    {
        Global.Singleton.PopUpClosed -= OnPopUpClosed;
    }
}
