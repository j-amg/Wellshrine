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
    public AnimatedSprite3D magic;
    [Export]
    public Sprite3D label;
    [Export]
    public Label name;
    public bool Active {get; set;}
    public Color ReticleModulate { get; set; }
    public bool Tooltip { get; set; }
    public string TooltipText { get; set; }
    public float HoverRange { get; set; }

    public override void _Ready()
    {
        Active = true;
        ReticleModulate = new Color(0,0,1);
        magic.Modulate = magicModulate;
        Tooltip = true;
    }

    public void Deactivate()
    {
        Active = false;
        label.Visible = false;
        if (magic != null)magic.Visible = false;
    }

    public void Activate()
    {
        Active = true;
        label.Visible = true;
        if (magic != null) magic.Visible = true;
    }

    void IInteractable.Interact() => OnInteract();

    public virtual void OnInteract()
    { 
        Global.Singleton.PlaySound3D(GlobalPosition, pickUp);
        Global.Singleton.hud.Flash(new Color(1,1,0));
        EmitSignal(SignalName.ShrineInteracted);   
    }

    public void StartHover() => Global.Singleton.ShowTooltip(TooltipText);

    public void EndHover() => Global.Singleton.CloseTooltip();
}
