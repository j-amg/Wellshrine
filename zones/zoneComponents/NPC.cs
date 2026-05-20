using System;
using Godot;
public partial class NPC : CharacterBody3D, IHoverable, IInteractable
{
    [Export]
    public string displayName;
    [Export]
    public AnimatedSprite3D sprite;
    [Export]
    public Sprite3D labelSprite;
    [Export]
    public string[] dialogue;
    public bool Active { get; set; }
    public Color ReticleModulate { get; set; }
    public bool Tooltip { get; set; }
    public string TooltipText { get; set; }
    public float HoverRange { get; set; }
    public bool talking = false;

    public override void _Ready()
    {
        base._Ready();
        //Global.Singleton.DialogueFinished += OnDialogueFinished;
        ReticleModulate = new Color(0,0,1);
        Active = true;
        Tooltip = false;
        TooltipText = "";
        HoverRange = 1000;
        sprite.Play("idle");
    }

    private void OnDialogueFinished() => talking = false;

    void IInteractable.Interact() => OnInteract();

    public virtual void OnInteract()
    {
        if (dialogue != null)
        {
            talking = true;
            labelSprite.Visible = false;
            //Global.Singleton.EnterDialogue(dialogue, displayName, false);
        }
    }

    public void StartHover()
    {
        if (!talking) labelSprite.Visible = true;
    }

    public void EndHover() => labelSprite.Visible = false;
}