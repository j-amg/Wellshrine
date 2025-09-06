using Godot;
using System;

public partial class GroundItem : RigidBody3D, IInteractable, IHoverable
{
    public Color ReticleModulate { get; set; }
    public bool Active { get; set; }
    public bool Tooltip { get; set; }
    public string TooltipText { get; set; }
    private Color beamColor = new(0, 0, 0);

    [Export] public SlotData slotData;

    public override void _Ready()
    {
        Active = true;
        ReticleModulate = new Color(1, 1, 0);
        Tooltip = true;
        AddToGroup("items");

        if (slotData != null)
        {
            TooltipText = slotData.itemData.name;
            if (slotData.Quantity > 1) TooltipText += " x" + slotData.Quantity;
        }
    }

    public static GroundItem InitGroundItem(SlotData slot, Vector3 position)
	{
        GroundItem item = Global.Singleton.item.Instantiate<GroundItem>();
        item.slotData = slot;
		item.Position = position;
        item.TooltipText = slot.itemData.name;
		return item;
	}

    void IInteractable.Interact() => OnInteract();

    public virtual void OnInteract()
    {
        Global.Singleton.inventory.inventoryData.PickUpSlotData(slotData);
        CallDeferred("queue_free");
    }


    public void StartHover() => Global.Singleton.ShowTooltip(slotData.itemData);

    public void EndHover() => Global.Singleton.CloseTooltip();
}
