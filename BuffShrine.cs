using Godot;
using System;
using Godot.Collections;
using System.Linq;
public partial class BuffShrine : Shrine, IInteractable
{
    [Export]
	public AudioStream pickUp;
    private string buff;

    public override void _Ready()
    {
        base._Ready();
        AddToGroup("shrines");
    }
    public void SetBuff(string type)
    {
        buff = type;
        name.Text = buff;
        //effect.Text = buff;
    }

    void IInteractable.Interact()
    {
        Tween tween = GetTree().CreateTween();
		tween.TweenProperty(Global.Singleton.player.hitFlash, "modulate", new Color(0,0,0,0), .5).From(new Color(1,1,0,1));
        GD.Print("Add buff " + buff + "!");
        Global.Singleton.AddBuff(buff);
        Global.Singleton.PlaySound2D(pickUp);
        if (Global.Singleton.CurrentScene is Zone zone) zone.UpdateObjective();
        foreach (Shrine shrine in GetTree().GetNodesInGroup("shrines").Cast<Shrine>()) shrine.Deactivate();
    }
}
