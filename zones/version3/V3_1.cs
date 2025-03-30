using Godot;
using System;

public partial class V3_1 : ShrineZone
{
    private int currentStep;
    public override void _Ready()
    {
        base._Ready();
        CallDeferred("Sequence1");
        Global.Singleton.PopUpClosed += OnPopUpClosed;
        Global.Singleton.EquippedWeapon += OnEquippedWeapon;
    }

    private async void Sequence1()
    {
        GD.Print("sequence1");
        currentStep = 1;
        Global.Singleton.hud.SetScreen(new Color(0, 0, 0, 1));
        await ToSignal(GetTree().CreateTimer(1f), "timeout");
        Global.Singleton.hud.FadeScreen(new Color(0, 0, 0, 0), .5f);
        await ToSignal(GetTree().CreateTimer(.5f), "timeout");
        Global.Singleton.SendPopUp("WASD to move", "walk");
    }

    private void OnEquippedWeapon()
    {
        if (currentStep == 1) Sequence2();
    }

    private void OnPopUpClosed(string action)
    {
        if (currentStep == 2) CompleteObjective();
    }

    private async void Sequence2()
    {
        await ToSignal(GetTree().CreateTimer(.5f), "timeout");
        currentStep = 2;
        Global.Singleton.SendPopUp("[Left Mouse] to Shoot", "attack");
    }
}
