using Godot;
using System;
using Godot.Collections;
using System.Linq;
public partial class BuffShrine : Shrine, IInteractable
{
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
        GD.Print("Add buff " + buff + "!");
        Global.Singleton.AddBuff(buff);
        if (Global.Singleton.CurrentScene is Zone zone) zone.UpdateObjective();
        foreach (Shrine shrine in GetTree().GetNodesInGroup("shrines").Cast<Shrine>()) shrine.Deactivate();
    }
}
