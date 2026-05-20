using Godot;
using System;
using System.Linq;
using System.Text.RegularExpressions;
public partial class Tooltip : PanelContainer
{
    [Export] public Label itemNameLabel;
    [Export] public Label itemTypeLabel;

    [Export] public Label itemDescriptionLabel;
    [Export] public Label spellTrigger;
    [Export] public Label manaCost;
    [Export] public Label spellCastTime;
    [Export] public Label spellChargeTime;
    [Export] public Label itemLevelLabel;

    [Export] public HBoxContainer spellTriggerContainer;
    [Export] public HBoxContainer manaCostContainer;
    [Export] public HBoxContainer spellCastTimeContainer;
    [Export] public HBoxContainer spellChargeTimeContainer;
    [Export] public HBoxContainer itemLevelContainer;
    [Export] public VBoxContainer itemAffixContainer;

    [Export] public PanelContainer headerContainer;
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

        // test

        // set loc to centred on slot
        Vector2 loc = slot.GlobalPosition + localSlotCentre - localToolTipCentre;

        // spawn directly above item
        loc += new Vector2(0, (-Size.Y + -slotSize.Y) / 2);

        // calculate offset
        float offsetX = 0;
        float offsetY = 0;

        //top (dont need to calculate bottom since tooltip is always above item)
        // try to left of item, if that clips, move right
        if (loc.Y < 0)
        {
            offsetY += Size.Y;
            if ((loc.X - Size.X / 2 - slotSize.X / 2) < 0) offsetX += Size.X - slotSize.X / 2;
            else offsetX -= Size.X / 2 + slotSize.X / 2;
        }
        else
        {
            if (loc.X + Size.X > viewportSize.X) offsetX -= loc.X + Size.X - viewportSize.X;
            if (loc.X < 0) offsetX -= loc.X;
        }
        return loc + new Vector2(offsetX, offsetY);
    }

    public void SetItem(ItemData itemData)
    {
        headerContainer.Modulate = itemData.rarity != 0 ? new Color(0.89f, 0.671f, 0.0f) : new Color(1, 1, 1);
        itemNameLabel.Text = itemData.name;
        itemTypeLabel.Text = itemData.Type.ToString();
        itemDescriptionLabel.Text = itemData.description;

        manaCostContainer.Hide();
        spellTriggerContainer.Hide();
        spellCastTimeContainer.Hide();
        spellChargeTimeContainer.Hide();
        itemLevelContainer.Hide();
        itemAffixContainer.Hide();

        // clear container
        foreach (Node c in itemAffixContainer.GetChildren()) itemAffixContainer.RemoveChild(c);

        if (itemData is ItemSpellData spell)
        {
            itemAffixContainer.Show();
            itemLevelLabel.Text = spell.level.ToString();
            itemLevelContainer.Show();

            manaCostContainer.Show();
            spellTriggerContainer.Show();
            spellCastTimeContainer.Show();
            if (spell.spell.triggerType != Spell.SpellTriggerType.Instant)
            {
                spellChargeTimeContainer.Show();
                spellChargeTime.Text = spell.spell.chargeTime.ToString() + "s";
            }

            manaCost.Text = spell.spell.manaCost.ToString();
            spellTrigger.Text = CamelCaseToText(spell.spell.triggerType.ToString());
            spellCastTime.Text = spell.spell.castTime.ToString() + "s";

            foreach (DamageData damageData in spell.spell.damageDatas)
            {
                if (damageData == null) continue;

                Label label = affixLabelScene.Instantiate<Label>();
                label.Text = damageData.amountMin + " to " + damageData.amountMax + " " + damageData.type + " Damage";
                itemAffixContainer.AddChild(label);
            }

            foreach (EntityEffect effect in spell.spell.entityEffects)
            {
                if (effect == null) continue;

                Label label = affixLabelScene.Instantiate<Label>();
                label.Text = "Applies " + effect.effectName;
                itemAffixContainer.AddChild(label);
            }
        }

        //populate if affixes exist
        if (itemData is ItemEquipmentData equipment)
        {
            itemAffixContainer.Show();
            itemLevelLabel.Text = equipment.level.ToString();
            itemLevelContainer.Show();
            foreach (ItemAffix affix in new ItemAffix[] { equipment.prefix, equipment.suffix })
            {
                if (affix == null) continue;

                Label label = affixLabelScene.Instantiate<Label>();
                switch (affix.attributeModifier.ModType.ToString())
                {
                    case "Flat":
                        label.Text = affix.attributeModifier.Value >= 0 ? "+" : "-";
                        label.Text += affix.attributeModifier.Value.ToString() + " to ";
                        label.Text += CamelCaseToText(affix.TargetType.ToString());
                        break;
                    case "PercentAdd":
                        label.Text = affix.attributeModifier.Value.ToString() + "% ";
                        label.Text += affix.attributeModifier.Value >= 0 ? "increased " : "decreased ";
                        label.Text += CamelCaseToText(affix.TargetType.ToString());
                        break;
                    case "PercentMult":
                        label.Text = affix.attributeModifier.Value >= 0 ? "+" : "-";
                        label.Text = affix.attributeModifier.Value.ToString() + "% ";
                        label.Text += "Total ";
                        label.Text += CamelCaseToText(affix.TargetType.ToString());
                        break;

                }
                itemAffixContainer.AddChild(label);
            }
        }
        Size = new Vector2(0, 0); // shrink tooltip
    }

    [GeneratedRegex("(\\B[A-Z])")]
    private static partial Regex MyRegex();
    public static string CamelCaseToText(string input) => MyRegex().Replace(input, " $1");
}
