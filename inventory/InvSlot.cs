using Godot;
using System;

public partial class InvSlot : Panel
{

    [Signal]
    public delegate void SlotInputEventHandler(int index);

    [Export] public CenterContainer container;

    [Export] TextureRect texture;

    [Export] Label quantityLabel;
    public override void _GuiInput(InputEvent @event)
    {
        //GD.Print("Mouse entered slot");
        if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == MouseButton.Left && mbe.Pressed)
        {
            EmitSignal(SignalName.SlotInput, GetIndex());
            GD.Print("Left mouse button was pressed!");
        }
    }

    internal void SetSlotData(SlotData slotdata)
    {
        ItemData itemData = slotdata.itemData;
        texture.Texture = itemData.texture;

        if (slotdata.Quantity > 1)
        {
            quantityLabel.Visible = true;
            quantityLabel.Text = slotdata.Quantity.ToString();
        }
        

    }
}
