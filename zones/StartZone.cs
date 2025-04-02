using Godot;
using System;

public partial class StartZone : ShrineZone
{
    private int currentStep;
    public override void _Ready()
    {
        base._Ready();
        CallDeferred("Open");
    }

    private async void Open()
    {
        Global.Singleton.hud.SetScreen(new Color(0, 0, 0, 1));
        await ToSignal(GetTree().CreateTimer(1f), "timeout");
        Global.Singleton.hud.FadeScreen(new Color(0, 0, 0, 0), .5f);
    }
}
