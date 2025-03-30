using Godot;
using System;

public partial class V2_2 : ShrineZone
{
    private int currentStep;
    public override void _Ready()
    {
        base._Ready();
        CallDeferred("Sequence1");
        Global.Singleton.PopUpClosed += OnPopUpClosed;
        Global.Singleton.EquippedWeapon += OnEquippedWeapon;
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

    private async void Sequence1()
    {
        GD.Print("sequence1");
        currentStep = 1;
        await ToSignal(GetTree().CreateTimer(.5f), "timeout");
        Global.Singleton.EnterDialogue(new string[] {"Welcome stranger.",
        "The path ahead is dangerous, equip yourself with one of the spells here before you continue.", "If you need assistance speak with me, I will make myself available where fitting.", "Now go."}, "ShrineKeeper", false);
    }
}
