using Godot;
using System;

public partial class Tooltip : PanelContainer
{
    [Export] public Label itemName;
    [Export] public Label itemDescription;

    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition = GetGlobalMousePosition() + new Vector2(5, 5);
    }

    public void ShowItem(ItemData itemData)
    {
        GD.Print("test");
        itemName.Text = itemData.name;
        itemDescription.Text = itemData.description;
        Show();
    }

    public void Remove()
    {
        Hide();
    }
}
