using Godot;
using System;
using System.Linq;

public partial class WeaponShrine : Shrine, IInteractable
{
    [Export]
    public string weapon;
    [Export]
    public string displayName;
    [Export]
    public string description;
    [Export]
	public AudioStream pickUp;

    public override void _Ready()
    {
        base._Ready();
        name.Text = displayName;
        AddToGroup("shrines");
    }
    void IInteractable.Interact()
    {
        GD.Print("Add weapon " + weapon + "!");
        Global.Singleton.EquipWeapon(weapon);
        Global.Singleton.PlaySound2D(pickUp);
        if (Global.Singleton.CurrentScene is Zone zone) zone.UpdateObjective();
        foreach (Shrine shrine in GetTree().GetNodesInGroup("shrines").Cast<Shrine>()) shrine.Activate();
        Deactivate();
    }
}
