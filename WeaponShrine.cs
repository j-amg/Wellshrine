using Godot;
using System;
using System.Linq;

public partial class WeaponShrine : Shrine
{
    [Export]
    public string weapon;
    [Export]
    public string displayName;

    public override void _Ready()
    {
        base._Ready();
        name.Text = displayName;
        PopUpText = Global.Singleton.weapons[weapon].description;
    }
    public override void OnInteract()
    {
        base.OnInteract();
        Global.Singleton.EquipWeapon(weapon);
        foreach (Shrine shrine in GetTree().GetNodesInGroup("shrines").Cast<Shrine>()) shrine.Activate();
        Deactivate();
    }
}
