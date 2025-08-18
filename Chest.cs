using Godot;
using System;

public partial class Chest : StaticBody3D, IInteractable, IHoverable
{

    [Signal] public delegate void ToggleInventoryEventHandler(Chest inventoryOwner);
    public Color ReticleModulate { get; set; }
    public bool Active { get; set; }
    public bool Tooltip { get; set; }
    public string TooltipText { get; set; }

    [Export] public InventoryData inventoryData;

    public override void _Ready()
    {
        Active = true;
        ReticleModulate = new Color(0, 0, 1);
}

    public void Interact() => OnInteract();

    public virtual void OnInteract()
    {
        EmitSignal(SignalName.ToggleInventory, this);
        //GD.Print("chest interact");
    }

    public void StartHover()
    {
        //throw new NotImplementedException();
        return;
    }


    public void EndHover()
    {
        //throw new NotImplementedException();
        return;
    }


}
