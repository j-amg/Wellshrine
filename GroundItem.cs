using Godot;
using System;

public partial class GroundItem : RigidBody3D, IInteractable, IHoverable
{
    public Color ReticleModulate { get; set; }
    public bool Active { get; set; }
    public bool Tooltip { get; set; }
    public string TooltipText { get; set; }
    private Color beamColor = new(0, 0, 0);

    SlotData slotData = new();

    public override void _Ready()
    {
        Active = true;
        ReticleModulate = new Color(1, 1, 0);
        AddToGroup("items");
        Tooltip = true;
        TooltipText = "[PH] grab item name and type"
        + "Rarity: ";
        slotData.itemData = GD.Load<ItemData>("res://inventory/items/gold.tres");
    }

    public static GroundItem InitGroundItem(Vector3 position)
	{
        GroundItem item = Global.Singleton.item.Instantiate<GroundItem>();
        
		item.Position = position;
		return item;
	}

    void IInteractable.Interact() => OnInteract();

    public virtual void OnInteract()
    {
        Global.Singleton.inventory.inventoryData.PickUpSlotData(slotData);
        CallDeferred("queue_free");
    }


    public void StartHover() => Global.Singleton.ShowTooltip(TooltipText);

    public void EndHover() => Global.Singleton.CloseTooltip();
}
