using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace FrozenApocalypse.Content.Items;

public class Coal : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Coal>());
        Item.width = 20;
        Item.height = 12;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(silver: 4, copper: 75);
    }
}
