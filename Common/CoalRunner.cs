using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;

namespace FrozenApocalypse;

public class CoalRunner
{
    private const float OreAmount = 300f / 252000f;

    public static void GenerateCoal(int topBorder, int bottomBorder)
    {
        for (int i = 0; i < (int)(Main.maxTilesX * Math.Abs(bottomBorder - topBorder) * OreAmount); i++)
        {
            int x = WorldGen.genRand.Next(0, Main.maxTilesX);
            int y = WorldGen.genRand.Next(topBorder, bottomBorder);
            if (!WorldGen.InWorld(x, y, 12))
            {
                continue;
            }
            if (TileID.Sets.SandBiome[Main.tile[x, y].TileType] <= 0 && !FrozenApocalypseIDs.TileSets.FrozenDesertTiles.Contains(Main.tile[x, y].TileType))
            {
                continue;
            }
            WorldGen.OreRunner(x, y, WorldGen.genRand.Next(5, 8), WorldGen.genRand.Next(3, 5), (ushort)ModContent.TileType<Content.Tiles.Coal>());
        }
    }
}
