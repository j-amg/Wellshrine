using Godot;
using System;

public partial class Tooltip : PanelContainer
{
    [Export] public Label itemName;
    [Export] public Label itemDescription;

    public void SetItem(ItemData itemData)
    {
        itemName.Text = itemData.name;
        itemDescription.Text = itemData.description;
    }

}
