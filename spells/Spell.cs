using System;
using Godot;


[GlobalClass]
public partial class Spell : Resource
{ 
    [Export] public float castTime;
    [Export] private AudioStream sound;
    [Export] public PackedScene spellScene;
    public Node3D spell;
    public virtual void Cast(Player player)
    {
        GD.Print("Cast");
    }

    internal void Equip(Player player)
    {
        spell = spellScene.Instantiate<Node3D>();
        player.head.AddChild(spell);
    }

    internal void Unequip()
    {
        spell.CallDeferred("queue_free");
    }
}
