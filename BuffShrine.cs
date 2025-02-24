using Godot;
using System;
using Godot.Collections;
using System.Linq;
public partial class BuffShrine : Shrine, IInteractable
{
    private string buff;
    public Array<string> buffTypes = new() {"Damage", "Critical Chance", "Run Speed", "Stun duration", "Attack Speed"};

    public override void _Ready()
    {
        base._Ready();
        SetBuff("Damage");
        AddToGroup("shrines");
    }
    public void SetBuff(string type)
    {
        buff = type;
        name.Text = buff;
        effect.Text = "+25% damage";
    }

    void IInteractable.Interact()
    {
        GD.Print("Add buff " + buff + "!");
        Global.Singleton.AddBuff("damage");
        if (Global.Singleton.CurrentScene is Zone zone) zone.UpdateObjective();
        foreach (Shrine shrine in GetTree().GetNodesInGroup("shrines").Cast<Shrine>()) shrine.Deactivate();
    }
}
