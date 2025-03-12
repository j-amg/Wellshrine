using Godot;
using System;

public partial class WellShrine : Shrine, IInteractable
{
    [Export]
	public AudioStream pickUp;
    void IInteractable.Interact()
    {
        Global.Singleton.hud.Flash(new Color(0,1,0));
        Global.Singleton.currentPlayerHealth = Global.Singleton.playerHealth;
        Global.Singleton.PlaySound2D(pickUp);
        Global.Singleton.UpdateHUD();
        Deactivate();
    }

}
