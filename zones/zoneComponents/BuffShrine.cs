using Godot;
using System;
using Godot.Collections;
using System.Linq;
public partial class BuffShrine : Shrine
{
    private string buff;
    public override void _Ready()
    {
        base._Ready();
        magic?.Play("idle");
    }
    public void SetBuff(string type)
    {
        buff = type;
        // name.Text = Global.Singleton.statModifiers[type].description;
        // TooltipText = Global.Singleton.statModifiers[type].longDescription;
    }

    public override void OnInteract()
    {
        base.OnInteract();
        // Global.Singleton.AddPlayerModifier(buff);
        foreach (Shrine shrine in GetTree().GetNodesInGroup("shrines").Cast<Shrine>())
        {
            if (shrine is BuffShrine) shrine.Deactivate();
        }
    }
}
