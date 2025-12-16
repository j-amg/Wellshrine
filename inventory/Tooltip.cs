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
        affixLabelScene = GD.Load<PackedScene>("res://inventory/affixToolTipLabel.tscn");
    }

    public Vector2 FindSpawnPosition(InvSlot slot)
    {
        Vector2 slotSize = slot.Size;
        Vector2 localSlotCentre = new(slotSize.X / 2, slotSize.Y / 2);
        Vector2 localToolTipCentre = new(Size.X / 2, Size.Y / 2);
        Vector2 viewportSize = GetViewport().GetVisibleRect().Size;

        // set loc to centred on slot
        Vector2 loc = slot.GlobalPosition + localSlotCentre - localToolTipCentre;

        // spawn directly above item
        loc += new Vector2(0, (-Size.Y + -slotSize.Y)/2);

        // calculate offset
        float offsetX = 0;
        float offsetY = 0;

        //if (0 - 45 < 0) offsetY += Mathf.Abs(0 - 45);

        //top (dont need to calculate bottom since tooltip is always above item)
        // try to left of item, if that clips, move right
        if (loc.Y < 0)
        {
            offsetY += Size.Y;
            if ((loc.X - Size.X/2 - slotSize.X / 2)! < 0)
            {
                offsetX += Size.X - slotSize.X / 2;
            }
            else
            {
                offsetX -= Size.X/2 + slotSize.X / 2;
            }
        }
        else
        {
            //right
            if (loc.X + Size.X > viewportSize.X) offsetX -= loc.X + Size.X - viewportSize.X;

            // left
            if (loc.X < 0) offsetX -= loc.X;
        } 

        return loc + new Vector2(offsetX, offsetY);
    }

    public void SetItem(ItemData itemData)
    {
        itemNameLabel.Text = itemData.name;
        itemTypeLabel.Text = itemData.Type.ToString();
        itemDescriptionLabel.Text = itemData.description;

        // clear container
        foreach (Node c in itemAffixContainer.GetChildren())
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
                if (affix != null)
                {
                    Label label = affixLabelScene.Instantiate<Label>();
                    switch(affix.attributeModifier.ModType.ToString())
                    {
                        case "Flat":
                            label.Text = affix.attributeModifier.Value >= 0 ? "+" : "-";
                            label.Text += affix.attributeModifier.Value.ToString() + " to ";
                            label.Text += affix.TargetType.ToString();
                            break;
                        case "PercentAdd":
                            label.Text = affix.attributeModifier.Value.ToString() + "% ";
                            label.Text += affix.attributeModifier.Value >= 0 ? "increased " : "decreased ";
                            label.Text += affix.TargetType.ToString();
                            break;
                        case "PercentMult":
                        label.Text = affix.attributeModifier.Value >= 0 ? "+" : "-";
                            label.Text = affix.attributeModifier.Value.ToString() + "% ";
                            label.Text += "Total ";
                            label.Text += affix.TargetType.ToString();
                            break;
                        
                    }
                    itemAffixContainer.AddChild(label);
                }

            }
        }
        Size = new Vector2(0, 0);
    }

}
