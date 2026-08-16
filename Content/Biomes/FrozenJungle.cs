using Terraria;
using Terraria.ModLoader;
using FrozenApocalypse.Warmth;

namespace FrozenApocalypse.Content.Biomes;

public class FrozenJungle : ModBiome
{
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<FrozenJungleBackground>();

    public override ModWaterStyle WaterStyle => ModContent.GetInstance<IceWaterStyle>();

    public override bool IsBiomeActive(Player player)
    {
        return ModContent.GetInstance<FrozenTileCounts>().FrozenJungleTileCount > 400 && !(player.ZoneJungle && player.GetModPlayer<ColdDebuffPlayer>().boilerWarm);
    }

    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;

    public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/FrozenJungle");
}

