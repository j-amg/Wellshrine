using Godot;
using System;

public partial class Chest : StaticBody3D, IInteractable, IHoverable
{

    [Signal] public delegate void ToggleInventoryEventHandler(Chest inventoryOwner);
    [Export(PropertyHint.Enum, "chest,doorchest")] public string chestType;
    [Export] public InventoryData inventoryData;
    public Color ReticleModulate { get; set; }
    public bool Active { get; set; }

    public override void _Ready()
    {
        Active = true;
        ReticleModulate = new Color(0, 0, 1);
}

    public void Interact() => OnInteract();

    public virtual void OnInteract()
    {
        GD.Print("test");
        EmitSignal(SignalName.ToggleInventory, this);
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
