using Terraria;
using System.Collections.Generic;
using FrozenApocalypse.Content.Tiles;
using Terraria.ModLoader;
using Terraria.ID;

namespace FrozenApocalypse;

public class FrozenApocalypseIDs
{
    public class TileSets
    {
        public static bool[] SandIce = TileID.Sets.Factory.CreateBoolSet(ModContent.TileType<SandIce>(), ModContent.TileType<EbonsandIce>(), ModContent.TileType<CrimsandIce>(), ModContent.TileType<PearlsandIce>());
        public static List<int> FrozenJungleTiles = new();
        public static List<int> FrozenDesertTiles = new();
    }

    public class BuffSets
    {
        public static List<int> NullifiedByWarmth = new() { BuffID.Chilled, BuffID.Frozen };
    }
}
