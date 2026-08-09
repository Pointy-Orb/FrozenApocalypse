using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

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
}
