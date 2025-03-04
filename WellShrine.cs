using Godot;
using System;

public partial class WellShrine : Shrine, IInteractable
{
    [Export]
	public AudioStream pickUp;
    void IInteractable.Interact()
    {
        Tween tween = GetTree().CreateTween();
		tween.TweenProperty(Global.Singleton.player.hitFlash, "modulate", new Color(0,0,0,0), .5).From(new Color(0,1,0,1));
        Global.Singleton.currentPlayerHealth = Global.Singleton.playerHealth;
        Global.Singleton.PlaySound2D(pickUp);
        Global.Singleton.UpdateHUD();
        Deactivate();
    }

}
