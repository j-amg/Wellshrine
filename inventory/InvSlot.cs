using Godot;
using System;

public partial class InvSlot : TextureRect
{
    [Export]
    public CenterContainer container;
    public InvItem item;
    public void SetItem(InvItem newItem)
    {
        item = newItem;
    }

    public void PickItem()
    {
        item.Pick();
        container.RemoveChild(item);
        item = null;
    }
}
