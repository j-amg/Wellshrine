using Godot;
using System;

public partial class Container : StaticBody3D, IInteractable, IHoverable
{
	public bool Active { get; set; }
	public Color ReticleModulate { get; set; }

    public override void _Ready()
    {
        Active = true;
    }

	public void Interact() => OnInteract();

    public virtual void OnInteract()
    {
        ItemData item = Global.Singleton.GenerateItem();
        SlotData slotData = new() { itemData = item };
        GroundItem groundItem = GroundItem.InitGroundItem(slotData, Position + new Vector3(0, .5f, -.5f));
        GetTree().CurrentScene.CallDeferred("add_child", groundItem);
	}

	public void EndHover()
	{

	}

	public void StartHover()
	{

	}
}
