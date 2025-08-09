using Godot;
using System;

public partial class GroundItem : Area3D, IInteractable, IHoverable
{
    public Color ReticleModulate { get; set; }
    public bool Active { get; set; }
    public bool Tooltip { get; set; }
    public string TooltipText { get; set; }
    public float HoverRange { get; set; }

    private Color beamColor = new(0, 0, 0);

    private int rarity;
    private string type;

    private int level;

    public override void _Ready()
    {
        Active = true;
        ReticleModulate = new Color(1, 1, 0);
        AddToGroup("items");
        HoverRange = Global.Singleton.interactionRange;
        Tooltip = true;
        TooltipText = "[PH] grab item name and type"
        + "Rarity: " + rarity;

    }

    public static GroundItem InitGroundItem(Vector3 position)
	{
        GroundItem item = Global.Singleton.item.Instantiate<GroundItem>();
        item.rarity = (int)Mathf.Abs(GD.Randi() % 5);
        item.level = Global.Singleton.currentLevel;
		item.Position = position;
		return item;
	}

    void IInteractable.Interact() => OnInteract();

    public virtual void OnInteract()
    {
        GD.Print("Picked up item!");
        CallDeferred("queue_free");
    }


    public void StartHover() => Global.Singleton.ShowTooltip(TooltipText);

    public void EndHover() => Global.Singleton.CloseTooltip();
}
