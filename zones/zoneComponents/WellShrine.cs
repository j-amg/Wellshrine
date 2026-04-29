using Godot;
using System;

public partial class WellShrine : Shrine, IInteractable
{
    [Export]
    public MeshInstance3D bloodMesh;
    public override void _Ready()
    {
        base._Ready();
        TooltipText = "Restores player Health.";
    }
    public override void OnInteract()
    {
        // base.OnInteract();
        // Tween tween = GetTree().CreateTween();
        // tween.TweenProperty(bloodMesh, "position", new Vector3(0,.716f,0), .5f);
        // Global.Singleton.hud.Flash(new Color(0,1,0));
        // Global.Singleton.SetPlayerHealth(Global.Singleton.playerHealth);
        // Deactivate();
    }

}
