using Godot;
using System;
using System.Linq;

public partial class WeaponShrine : Shrine, IInteractable
{
    private string buff;
    [Export]
    public string weapon;
    [Export]
    public string description;

    public override void _Ready()
    {
        base._Ready();
        name.Text = weapon;
        effect.Text = description; 
    }
    void IInteractable.Interact()
    {
        GD.Print("Add weapon " + weapon + "!");
        Global.Singleton.equippedWeapon = weapon;
        if (Global.Singleton.CurrentScene is Zone zone) zone.UpdateObjective();


        foreach (Shrine shrine in GetTree().GetNodesInGroup("shrines").Cast<Shrine>()) shrine.Deactivate();
    }
}
