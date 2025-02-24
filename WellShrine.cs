using Godot;
using System;

public partial class WellShrine : Shrine, IInteractable
{
    void IInteractable.Interact()
    {
        GD.Print("Heal!");
        Global.Singleton.currentPlayerHealth = Global.Singleton.playerHealth;
        Global.Singleton.UpdateHUD();
        Deactivate();
    }

}
