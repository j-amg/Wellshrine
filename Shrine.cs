using Godot;
using System;
using System.Linq;

public partial class Shrine : StaticBody3D, IInteractable, IHoverable
{
    [Signal]
    public delegate void ShrineInteractedEventHandler();
    [Export]
    public Color magicModulate;
    [Export]
	public AudioStream pickUp;
    [Export]
    private AnimatedSprite3D magic;
    [Export]
    public Sprite3D label;
    [Export]
    public Label name;
    public bool Highlighted {get; set;}
    public bool Active {get; set;}
    public Color ReticleModulate { get; set; }

    public override void _Ready()
    {
        Active = true;
        ReticleModulate = new Color(0,0,1);
        magic.Modulate = magicModulate;
        magic?.Play("idle");
        AddToGroup("shrines");
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
        magic.Visible = true;
    }

    void IInteractable.Interact() => OnInteract();

    public virtual void OnInteract()
    { 
        Global.Singleton.PlaySound2D(pickUp);
        Global.Singleton.hud.Flash(new Color(1,1,0));
        EmitSignal(SignalName.ShrineInteracted);   
    }
}
