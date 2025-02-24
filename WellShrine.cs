using Godot;
using System;

public partial class WellShrine : Shrine, IInteractable
{
        void IInteractable.Interact()
    {
        GD.Print("Add buff " + buff + "!");
        Global.Singleton.AddBuff("damage");
        if (Global.Singleton.CurrentScene is Zone zone)
		{
			GD.Print("try update");
			zone.UpdateObjective();
		} 

        foreach (Shrine shrine in GetTree().GetNodesInGroup("shrines").Cast<Shrine>()) shrine.Deactivate();
    }

}
