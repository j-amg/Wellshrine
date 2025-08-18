using Godot;
using System;

public partial class InvSlot : Panel
{

    [Signal]
    public delegate void SlotInputEventHandler(int index, int buttonIndex);
    [Signal] public delegate void SlotHoverEventHandler(int index);

    [Export] public CenterContainer container;

    [Export] TextureRect texture;

    [Export] Label quantityLabel;

    public override void _Ready()
    {
        MouseEntered += OnMouseEntered;
    }

    private void OnMouseEntered()
    {
        EmitSignal(SignalName.SlotHover, GetIndex());
    }

    public override void _GuiInput(InputEvent @event)
    {
        //GD.Print("Mouse entered slot");
        if (@event is InputEventMouseButton mbe && mbe.Pressed)
        {
            EmitSignal(SignalName.SlotInput, GetIndex(), (int)mbe.ButtonIndex);
        }
    }
    

    internal void SetSlotData(SlotData slotdata)
    {
        ItemData itemData = slotdata.itemData;
        texture.Texture = itemData.texture;
        quantityLabel.Text = slotdata.Quantity.ToString();

        if (slotdata.Quantity > 1)
        {
            quantityLabel.Show();
        }
        else quantityLabel.Hide();


    }
}
