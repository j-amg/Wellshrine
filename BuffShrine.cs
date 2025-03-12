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
    }

    void IInteractable.Interact()
    {
        Global.Singleton.hud.Flash(new Color(1,1,0));
        Global.Singleton.AddBuff(buff);
        Global.Singleton.PlaySound2D(pickUp);
        if (Global.Singleton.CurrentScene is Zone zone) zone.UpdateObjective();
        foreach (Shrine shrine in GetTree().GetNodesInGroup("shrines").Cast<Shrine>()) shrine.Deactivate();
    }
}
