using Godot;
using System;
using Godot.Collections;

public partial class Shrine : StaticBody3D, IInteractable
{
    private bool highlighted;
    private string buff;
    public Array<string> buffTypes = new() {"damage", "crit", "jump", "stun", "attackspeed"};
    public bool Highlighted {get; set;}
    public override void _Ready()
    {
        SelectBuff();
    }
    public void SelectBuff()
    {
        buff = buffTypes.PickRandom();
        
    }
    void IInteractable.Interact()
    {
        GD.Print("Add buff " + buff + "!");
        Global.Singleton.AddBuff("damage");
    }
}
