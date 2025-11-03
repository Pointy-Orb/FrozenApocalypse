using Terraria;
using FrozenApocalypse.Content.Tiles;
using Terraria.ModLoader;
using Terraria.ID;
using System;

namespace FrozenApocalypse.Content.Biomes;

public class FrozenOcean : ModBiome
{
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<FrozenOceanBackground>();

    public override ModWaterStyle WaterStyle => ModContent.GetInstance<IceWaterStyle>();

    public override bool IsBiomeActive(Player player)
    {
        return player.ZoneBeach && ModContent.GetInstance<FrozenOceanTileCount>().frozenOceanTiles > 40;
    }

    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

    public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/frozensurface");
}

public class FrozenOceanTileCount : ModSystem
{
    public int frozenOceanTiles;

    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
    {
        frozenOceanTiles = tileCounts[ModContent.TileType<SandIce>()];
        frozenOceanTiles += tileCounts[ModContent.TileType<EbonsandIce>()];
        frozenOceanTiles += tileCounts[ModContent.TileType<CrimsandIce>()];
        frozenOceanTiles += tileCounts[ModContent.TileType<PearlsandIce>()];
        frozenOceanTiles += tileCounts[ModContent.TileType<EvilSnow>()];
        frozenOceanTiles += tileCounts[TileID.BreakableIce];
    }
}

