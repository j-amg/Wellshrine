using Godot;
using System;

public partial class V1_2 : Zone
{
    private int currentStep;
    public override void _Ready()
    {
        base._Ready();
        checkpoints[0].BodyEntered += OnCP1BodyEntered;
        checkpoints[1].BodyEntered += OnCP2BodyEntered;
        CallDeferred("Sequence1");
    }

    private void OnCP1BodyEntered(Node3D body)
    {
        if (body is Player && currentStep == 1) Sequence2();
    }

    private void OnCP2BodyEntered(Node3D body)
    {
        if (body is Player && currentStep == 2) Sequence3();
    }

    private async void Sequence1()
    {
        GD.Print("sequence1");
        //await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        currentStep = 1;
        //GD.Print("step 1");
        await ToSignal(GetTree().CreateTimer(1f), "timeout");
        Global.Singleton.hud.FadeScreen(new Color(0, 0, 0, 0), .5f);
        await ToSignal(GetTree().CreateTimer(.5f), "timeout");
        Global.Singleton.EnterDialogue(new string[] {"Ah...",
        "The years have been harsh on this place,", "I'm sure you'll find a way through."}, "???", false);
    }

    private async void Sequence2()
    {
        GD.Print("sequence2");
        currentStep = 2;
        await ToSignal(GetTree().CreateTimer(1f), "timeout");
        //GD.Print("step 2");
        Global.Singleton.SendPopUp("Press [␣] to Jump", "jump");
    }

        private async void Sequence3()
    {
        GD.Print("sequence3");
        currentStep = 3;
        await ToSignal(GetTree().CreateTimer(1f), "timeout");
        //GD.Print("step 2");
        Global.Singleton.SendPopUp("Hold [Right Mouse Button] while midair to Glide", "glide");
    }

}
