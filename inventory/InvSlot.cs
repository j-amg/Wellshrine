using Godot;
using System;

public partial class InvSlot : PanelContainer
{

	[Signal] public delegate void SlotInputEventHandler(int index, int buttonIndex, InvSlot invSlot);
	[Signal] public delegate void SlotHoverEventHandler(int index, InvSlot invSlot);
	[Signal] public delegate void SlotExitEventHandler(int index, InvSlot invSlot);
	[Export] public CenterContainer container;
	[Export] TextureRect texture;
	[Export] Label quantityLabel;
	[Export] public bool transparentBackground = false;
	[Export] public ColorRect BG;

	public Color defaultModulate = new(1, 1, 1, 0);
	public bool highlighted = false;

	public override void _Ready()
	{
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
	}
    private void OnMouseEntered()
    {   
        EmitSignal(SignalName.SlotHover, GetIndex(), this);
    }

    private void OnMouseExited()
    {
        EmitSignal(SignalName.SlotExit, GetIndex(), this);
    }

    public override void _GuiInput(InputEvent @event)	{
		if (@event is InputEventMouseButton mbe && mbe.Pressed)
		{
			EmitSignal(SignalName.SlotInput, GetIndex(), (int)mbe.ButtonIndex, this);
		}
	}
	

	public void SetSlotData(SlotData slotdata)
	{
		texture.Texture = slotdata.itemData.texture;
		quantityLabel.Text = slotdata.Quantity.ToString();
		if (!transparentBackground)
		{
			defaultModulate = slotdata.itemData.rarity == 1 ? new Color(0.794f, 0.673f, 0.0f, 0.1f) : new Color(1, 1, 1, 0.1f);
			BG.Modulate = defaultModulate;
		}

		if (slotdata.Quantity > 1)
		{
			quantityLabel.Show();
		}
		else quantityLabel.Hide();


	}
}
