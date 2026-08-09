using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using FrozenApocalypse.Content.Tiles;

namespace FrozenApocalypse.Content.Items;

public class Peat : ModItem
{
    public override void SetDefaults()
    {
        Item.height = 16;
        Item.width = 20;
        Item.rare = ItemRarityID.Blue;
        Item.maxStack = Item.CommonMaxStack;
        Item.useTime = 10;
        Item.useAnimation = 10;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.autoReuse = true;
        Item.consumable = true;
    }
}
