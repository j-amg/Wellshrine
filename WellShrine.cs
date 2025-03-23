using Godot;
using System;

public partial class WellShrine : Shrine, IInteractable
{
    public override void _Ready()
    {
        base._Ready();
        PopUpText = "Restores player Health.";
    }
    public override void OnInteract()
    {
        Global.Singleton.hud.Flash(new Color(0,1,0));
        //Global.Singleton.PlaySound2D(pickUp);
        Global.Singleton.SetPlayerHealth(Global.Singleton.playerHealth);
        Deactivate();
    }

}
