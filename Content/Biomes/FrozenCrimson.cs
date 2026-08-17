using Terraria;
using Terraria.ModLoader;
using FrozenApocalypse.Warmth;

namespace FrozenApocalypse.Content.Biomes;

public class FrozenCrimson : ModBiome
{
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<FrozenCrimsonBackground>();

    public override ModWaterStyle WaterStyle => ModContent.GetInstance<IceWaterStyle>();

    public override bool IsBiomeActive(Player player)
    {
        return ModContent.GetInstance<FrozenTileCounts>().FrozenCrimsonTileCount > 200 && !(player.ZoneCrimson && player.GetModPlayer<ColdDebuffPlayer>().boilerWarm);
    }

    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

    public override int Music => Main.LocalPlayer.Center.Y > Main.worldSurface * 16 + (double)(Main.screenHeight / 2)
        ? MusicLoader.GetMusicSlot(Mod, "Assets/Music/FrozenUndergroundCrimson")
        : MusicLoader.GetMusicSlot(Mod, "Assets/Music/FrozenCrimson");
}

