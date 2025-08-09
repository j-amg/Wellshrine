using Godot;
using System;

public partial class Inv : NinePatchRect
{
    [Export]
    public string invName;
    public Label invLabel;
    public InvContainer invContainer;
    public InvSlot[] invSlots;

    public PackedScene slotScene;

    [Export]
    public int InvSize { get; set; }

    public override void _Ready()
    {
        invLabel = GetChild<Label>(0);
        invContainer = GetChild<InvContainer>(1);
        slotScene = GD.Load<PackedScene>("res://inventory/inv_slot.tscn");
    }

    public void AddItem(InvItem item)
    {
        
    }
    public void SetInvSize(int size)
    {
        InvSize = size;
        CustomMinimumSize = new Vector2(size * 4 / 3, 16);

        for (int i = 0; i < size; i++)
        {
            invSlots[i] = slotScene.Instantiate<InvSlot>();
        }

        foreach (Node n in invContainer.GetChildren())
        {
            n.QueueFree();
        }

        foreach (InvSlot s in invSlots)
        {
            invContainer.AddChild(s);
        }
    }
}
