using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using FrozenApocalypse.Content.Items;
using FrozenApocalypse.Content.TileEntities;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Audio;

namespace FrozenApocalypse;

public class Fuels : GlobalItem
{
    public static readonly Dictionary<int, int> fuels = new();

    public override void SetStaticDefaults()
    {
        fuels.Add(ItemID.Torch, 1);
        if (RecipeGroup.recipeGroupIDs.ContainsKey("Wood"))
        {
            int groupIndex = RecipeGroup.recipeGroupIDs["Wood"];
            RecipeGroup group = RecipeGroup.recipeGroups[groupIndex];
            foreach (int item in group.ValidItems)
            {
                fuels.Add(item, 5);
            }
        }
        fuels.Add(ModContent.ItemType<Peat>(), 20);
        fuels.Add(ModContent.ItemType<Coal>(), 45);
    }

    public override bool? UseItem(Item item, Player player)
    {
        if (!fuels.ContainsKey(item.type))
        {
            return null;
        }
        var pos = Main.MouseWorld.ToTileCoordinates();
        if (Main.tile[pos.X, pos.Y].TileType != ModContent.TileType<Boiler>())
        {
            return null;
        }
        if (!player.InInteractionRange(pos.X, pos.Y, TileReachCheckSettings.Simple))
        {
            return null;
        }
        BoilerEntity boiler;
        if (!TileEntity.TryGet<BoilerEntity>(pos.X, pos.Y, out boiler))
        {
            return null;
        }
        if (boiler.DecrementFuelRequirement() || boiler.timers.Count <= 0)
        {
            boiler.timers.Push(fuels[item.type] * 60);
        }
        else
        {
            var curTimer = boiler.timers.Pop();
            curTimer += fuels[item.type] * 60;
            boiler.timers.Push(curTimer);
        }
        SoundEngine.PlaySound(SoundID.Item34, Main.MouseWorld);
        for (int i = 0; i < 3; i++)
        {
            Dust.NewDustPerfect(Main.MouseWorld, DustID.Torch);
        }
        return true;
    }
}
