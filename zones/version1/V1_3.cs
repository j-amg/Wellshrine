using Godot;
using System;

public partial class V1_3 : Zone
{
    private int currentStep;
    public override void _Ready()
    {
        base._Ready();
        checkpoints[0].BodyEntered += OnCP1BodyEntered;
        checkpoints[1].BodyEntered += OnCP2BodyEntered;
        checkpoints[2].BodyEntered += OnCP3BodyEntered;
        currentStep = 0;
    }

    private void OnCP3BodyEntered(Node3D body)
    {
        Global.Singleton.ClosePopUp("", false);
    }

    private void OnCP1BodyEntered(Node3D body)
    {
        if (body is Player && currentStep == 0) Sequence1();
    }

    private void OnCP2BodyEntered(Node3D body)
    {
        if (body is Player && currentStep == 1) Sequence2();
    }

    private void Sequence2()
    {
        currentStep = 2;
        Global.Singleton.ClosePopUp("", true);
        Global.Singleton.SendPopUp("Combine Dash and Glide to cover great distances", "");
    }

    private void Sequence1()
    {
        currentStep = 1;
        Global.Singleton.SendPopUp("Hold [Shift] and Press [Spacebar] to Dash", "");
    }

}
