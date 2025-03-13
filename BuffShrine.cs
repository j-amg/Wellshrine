using Godot;
using System;
using Godot.Collections;
using System.Linq;
public partial class BuffShrine : Shrine, IInteractable
{
    private string buff;
    public void SetBuff(string type)
    {
        buff = type;
        name.Text = Global.Singleton.statModifiers[type].description;
    }

    public override void OnInteract()
    {
        base.OnInteract();
        Global.Singleton.AddPlayerModifier(buff);
        foreach (Shrine shrine in GetTree().GetNodesInGroup("shrines").Cast<Shrine>())
        {
            if (shrine is BuffShrine) shrine.Deactivate();
        }
    }
}
