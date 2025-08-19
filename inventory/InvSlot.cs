using Godot;
using System;

public partial class InvSlot : Panel
{

	[Signal] public delegate void SlotInputEventHandler(int index, int buttonIndex);
	[Signal] public delegate void SlotHoverEventHandler(int index);
	[Signal] public delegate void SlotExitEventHandler(int index);

	[Export] public CenterContainer container;

	[Export] TextureRect texture;
	[Export] Label quantityLabel;
    [Export] Color defaultModulate;
    
    public ItemData itemData;

	public override void _Ready()
    {
        defaultModulate = SelfModulate;
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }
    private void OnMouseEntered()
    {
        if (itemData != null || (Global.Singleton.inventory.grabbedSlotData != null && itemData == null))
        {
            SelfModulate = new Color(0, 1, 0, defaultModulate.A);
        }    
        EmitSignal(SignalName.SlotHover, GetIndex());
    }

    private void OnMouseExited()
    {
    	SelfModulate = defaultModulate;
        EmitSignal(SignalName.SlotExit, GetIndex());
    }

    public override void _GuiInput(InputEvent @event)	{
		if (@event is InputEventMouseButton mbe && mbe.Pressed)
		{
			EmitSignal(SignalName.SlotInput, GetIndex(), (int)mbe.ButtonIndex);
		}
	}
	

	public void SetSlotData(SlotData slotdata)
	{
		itemData = slotdata.itemData;
		texture.Texture = itemData.texture;
		quantityLabel.Text = slotdata.Quantity.ToString();

		if (slotdata.Quantity > 1)
		{
			quantityLabel.Show();
		}
		else quantityLabel.Hide();


	}
}
