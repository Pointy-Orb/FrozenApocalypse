using System;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using FrozenApocalypse.Content.Tiles;

namespace FrozenApocalypse;

public class FrozenTileCounts : ModSystem
{
    public int SnowWasteTileCount { get; set; }
    public int FrozenJungleTileCount { get; set; }
    public int FrozenDesertTileCount { get; set; }

    public int TotalTileCount { get; private set; }
    public int TotalFrozenTileCount { get; private set; }

    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
    {
        Reset();
        CountBiomeTiles(tileCounts);

        for (int i = 0; i < tileCounts.Length; i++)
        {
            TotalTileCount += tileCounts[i];
        }
        if (TotalTileCount >= 1500)
        {
            Main.SceneMetrics.SnowTileCount += TotalFrozenTileCount;
            return;
        }
        if (Main.LocalPlayer.GetModPlayer<ColdDebuffPlayer>().ColdLevel < 1)
        {
            return;
        }
        Main.SceneMetrics.SnowTileCount = 1501;
        SnowWasteTileCount = 301;
    }

    private void Reset()
    {
        SnowWasteTileCount = 0;
        FrozenJungleTileCount = 0;
        FrozenDesertTileCount = 0;
        TotalTileCount = 0;
        TotalFrozenTileCount = 0;
    }

    private void CountBiomeTiles(ReadOnlySpan<int> tileCounts)
    {
        SnowWasteTileCount += tileCounts[ModContent.TileType<EvilSnow>()];
        SnowWasteTileCount += tileCounts[ModContent.TileType<EvilIce>()];
        SnowWasteTileCount += tileCounts[TileID.BreakableIce];

        for (int i = 0; i < FrozenApocalypseIDs.TileSets.FrozenJungleTiles.Count; i++)
        {
            FrozenJungleTileCount += tileCounts[FrozenApocalypseIDs.TileSets.FrozenJungleTiles[i]];
        }
        for (int i = 0; i < FrozenApocalypseIDs.TileSets.FrozenDesertTiles.Count; i++)
        {
            FrozenDesertTileCount += tileCounts[FrozenApocalypseIDs.TileSets.FrozenDesertTiles[i]];
        }

        foreach (int frozenTile in TileFreezing.FreezableTiles.Values)
        {
            TotalFrozenTileCount += tileCounts[frozenTile];
        }
    }
}
