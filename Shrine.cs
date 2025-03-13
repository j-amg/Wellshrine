using Godot;
using System;

using System.Linq;

public partial class Shrine : StaticBody3D, IInteractable, IHoverable
{
    [Export]
    public Color magicModulate;
    private AnimatedSprite3D magic;
    public Sprite3D label;
    public Label name;
    public bool Highlighted {get; set;}
    public bool Active {get; set;}
    public Color ReticleModulate { get; set; }

    public override void _Ready()
    {
        Active = true;
        label = GetNode<Sprite3D>("Sprite3D");
        name = GetNode<Label>("SubViewport/Control/Name");
        magic = GetNode<AnimatedSprite3D>("magic");
        ReticleModulate = new Color(0,0,1);
        magic.Modulate = magicModulate;
        magic?.Play("idle");
    }

    public void Deactivate()
    {
        Active = false;
        label.Visible = false;
        magic.Visible = false;
    }

    public void Activate()
    {
        Active = true;
        label.Visible = true;
    }
}
