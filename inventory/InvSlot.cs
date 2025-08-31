using Godot;
using System;

public partial class InvSlot : Panel
{

	[Signal] public delegate void SlotInputEventHandler(int index, int buttonIndex, InvSlot invSlot);
	[Signal] public delegate void SlotHoverEventHandler(int index, InvSlot invSlot);
	[Signal] public delegate void SlotExitEventHandler(int index, InvSlot invSlot);
	[Export] public CenterContainer container;
	[Export] TextureRect texture;
	[Export] Label quantityLabel;
	public bool highlighted = false;
    //[Export] Color defaultModulate;


	public override void _Ready()
	{
		//defaultModulate = SelfModulate;
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

		if (slotdata.Quantity > 1)
		{
			quantityLabel.Show();
		}
		else quantityLabel.Hide();


	}
}
