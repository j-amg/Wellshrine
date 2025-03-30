using Godot;
using System;

public partial class V2_3 : KillZone
{
    private int currentStep = 0;

    public override void _Ready()
    {
        base._Ready();
        CallDeferred("Sequence1");
        Global.Singleton.currentZone.ZoneObjectiveComplete += OnZoneObjectiveComplete;
        Global.Singleton.PopUpClosed += OnPopUpClosed;
    }

    private void OnPopUpClosed(string action)
    {
        if (currentStep == 2) Sequence3();
    }

    private void OnZoneObjectiveComplete(Zone zone)
    {
        if (currentStep == 1) Sequence2();

    }

    private void Sequence2()
    {
        currentStep = 2;
        if (Global.Singleton.inPopup) Global.Singleton.ClosePopUp("", true);
        Global.Singleton.SendPopUp("[Spacebar] to jump", "jump");
    }

    private void Sequence3()
    {
        currentStep = 3;
        Global.Singleton.SendPopUp("Hold [Right Mouse] while mid-air to glide", "glide");

    }

    private async void Sequence1()
    {
        currentStep = 1;
        await ToSignal(GetTree().CreateTimer(.5f), "timeout");
        Global.Singleton.SendPopUp("Tap [Shift] while moving to evade enemy attacks", "slide");
    }

}
