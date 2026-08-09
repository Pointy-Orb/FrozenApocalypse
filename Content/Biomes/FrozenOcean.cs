using Terraria;
using Terraria.ModLoader;

namespace FrozenApocalypse.Content.Biomes;

public class FrozenOcean : ModBiome
{
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<FrozenOceanBackground>();

    public override ModWaterStyle WaterStyle => ModContent.GetInstance<IceWaterStyle>();

    public override bool IsBiomeActive(Player player)
    {
        return player.ZoneBeach && ModContent.GetInstance<FrozenTileCounts>().FrozenDesertTileCount > 40;
    }

    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

    public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/frozensurface");
}

