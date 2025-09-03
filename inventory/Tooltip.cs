using Godot;
using System;
using System.Linq;

public partial class Tooltip : PanelContainer
{
    [Export] public Label itemNameLabel;
    [Export] public Label itemTypeLabel;
    [Export] public Label itemLevelLabel;
    [Export] public Label itemDescriptionLabel;
    [Export] public VBoxContainer itemAffixContainer;
    [Export] public HBoxContainer itemLevelContainer;
    private PackedScene affixLabelScene;

    public override void _Ready()
    {
        affixLabelScene = GD.Load<PackedScene>("res://affixToolTipLabel.tscn");
    }

    public void SetItem(ItemData itemData)
    {
        itemNameLabel.Text = itemData.name;
        itemTypeLabel.Text = itemData.Type.ToString();
        itemDescriptionLabel.Text = itemData.description;

        // clear container
        foreach (Control c in itemAffixContainer.GetChildren())
        {
            itemAffixContainer.RemoveChild(c);
        }
        itemLevelContainer.Hide();
        itemAffixContainer.Hide();

        //populate if affixes exist
        if (itemData is ItemEquipmentData equipment)
        {
            itemAffixContainer.Show();
            itemLevelLabel.Text = equipment.level.ToString();
            itemLevelContainer.Show();
            foreach (ItemAffix affix in equipment.affixes.Cast<ItemAffix>())
            {
                AffixToolTipLabel affixLabel = affixLabelScene.Instantiate<AffixToolTipLabel>();
                affixLabel.typeLabel.Text = affix.TargetType.ToString();
                affixLabel.valueLabel.Text = affix.attributeModifier.Value.ToString();
                affixLabel.modTypeLabel.Text = affix.attributeModifier.ModType.ToString();
                itemAffixContainer.AddChild(affixLabel);
            }
        }
        Size = new Vector2(0, 0);
    }

}
