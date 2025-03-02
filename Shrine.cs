using Godot;
using System;

using System.Linq;

public partial class Shrine : StaticBody3D, IInteractable
{
    public Sprite3D label;
    public Label name;
    public Label effect;
    public bool Highlighted {get; set;}
    public bool Active {get; set;}
    public override void _Ready()
    {
        Active = true;
        label = GetNode<Sprite3D>("Sprite3D");
        name = GetNode<Label>("SubViewport/Control/Name");
        effect = GetNode<Label>("SubViewport/Control/Effect");
    }

    public void Deactivate()
    {
        Active = false;
        label.Visible = false;
    }

    public void Activate()
    {
        Active = true;
        label.Visible = true;
    }
}
