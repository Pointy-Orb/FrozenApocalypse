using Microsoft.Xna.Framework;
using Terraria;
using FrozenApocalypse.Content.Tiles;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using System.Linq;

namespace FrozenApocalypse.Content.Biomes;

public class SnowWaste : ModBiome
{
    public override ModWaterStyle WaterStyle => ModContent.GetInstance<IceWaterStyle>();

    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<SnowWasteBackground>();

    public override bool IsBiomeActive(Player player)
    {
        return ModContent.GetInstance<FrozenTileCounts>().SnowWasteTileCount > 300;
    }

    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;

    public override int Music => (!Main.swapMusic == Main.drunkWorld && !Main.remixWorld) ? MusicID.OtherworldlyIce : MusicLoader.GetMusicSlot(Mod, "Assets/Music/frozensurface");

    public override void SpecialVisuals(Player player, bool isActive)
    {
        if (!isActive)
        {
            return;
        }
        player.ManageSpecialBiomeVisuals("Blizzard", Main.UseStormEffects && (player.ZoneOverworldHeight || player.ZoneSkyHeight) && !player.behindBackWall && !player.GetModPlayer<ColdDebuffPlayer>().boilerWarm);
    }
}
