using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using System.Collections.Generic;
using Terraria.Localization;
using FrozenApocalypse.Warmth;

namespace FrozenApocalypse;

public class VanillaChanges : GlobalItem
{
    public override void UpdateAccessory(Item item, Player player, bool hideVisual)
    {
        if (item.type == ItemID.HandWarmer)
        {
            player.GetModPlayer<ColdDebuffPlayer>().handWarmer = true;
        }
    }

    public static List<int> warmingItems = new()
    {
        ItemID.HandWarmer,
        ItemID.EskimoHood,
        ItemID.PinkEskimoHood,
        ItemID.EskimoCoat,
        ItemID.PinkEskimoCoat,
        ItemID.EskimoPants,
        ItemID.PinkEskimoPants,
    };

    private static LocalizedText KeepsWarm;
    private static LocalizedText Wear;
    private static LocalizedText Equip;
    private static LocalizedText Drink;

    public override void SetStaticDefaults()
    {
        KeepsWarm = Language.GetText("Mods.FrozenApocalypse.CommonTooltips.Warming");
        Wear = Language.GetText("Mods.FrozenApocalypse.CommonTooltips.Wear");
        Equip = Language.GetText("Mods.FrozenApocalypse.CommonTooltips.Equip");
        Drink = Language.GetText("Mods.FrozenApocalypse.CommonTooltips.Drink");
    }

    public override void AddRecipes()
    {
        Recipe.Create(ItemID.HandWarmer)
            .AddIngredient(ItemID.Silk, 6)
            .AddIngredient(ItemID.FlinxFur, 2)
            .AddTile(TileID.Loom)
            .Register();

        Recipe.Create(ItemID.LavaBucket)
            .AddIngredient(ItemID.EmptyBucket)
            .AddIngredient(ItemID.StoneBlock)
            .AddTile(TileID.Hellforge)
            .Register();
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (!warmingItems.Contains(item.type) && item.buffType != BuffID.Warmth && item.buffType != BuffID.Tipsy)
        {
            return;
        }
        int i = 0;
        int tooltipLine = -1;
        while ((tooltipLine = tooltips.FindIndex(l => l.Name == $"Tooltip{i}")) > -1 && i < tooltips.Count)
        {
            i++;
        }
        tooltipLine = tooltips.FindIndex(l => l.Name == $"Tooltip{i - 1}");
        if (tooltipLine < 0)
        {
            tooltipLine = 2;
        }
        LocalizedText verbing = Wear;
        if (item.accessory)
        {
            verbing = Equip;
        }
        if (item.useStyle == ItemUseStyleID.DrinkLiquid)
        {
            verbing = Drink;
        }
        tooltips.Insert(tooltipLine + 1, new TooltipLine(Mod, "WarmingEffect", KeepsWarm.Format(verbing)));
    }
}
