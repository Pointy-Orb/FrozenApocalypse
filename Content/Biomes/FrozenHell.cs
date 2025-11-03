using Terraria;
using FrozenApocalypse.Content.Tiles;
using Terraria.ModLoader;
using Terraria.ID;
using System;

namespace FrozenApocalypse.Content.Biomes;

public class FrozenHell : ModBiome
{
    public override ModWaterStyle WaterStyle => ModContent.GetInstance<IceWaterStyle>();

    public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/FrozenHell");

    public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

    public const int minimumAsh = 200;

    public override string BackgroundPath => "FrozenApocalypse/Assets/Backgrounds/FrozenHell/MapBG";
    public override string MapBackground => BackgroundPath;

    public static bool PlayerInBiomeForVisuals(Player player)
    {
        return ModContent.GetInstance<FrozenHellTileCount>().frozenAshCount >= minimumAsh;
    }

    public override bool IsBiomeActive(Player player)
    {
        if (!player.ZoneUnderworldHeight)
        {
            return false;
        }
        if (ModContent.GetInstance<FrozenHellTileCount>().frozenAshCount < minimumAsh)
        {
            return false;
        }
        return true;
    }

    public override void SpecialVisuals(Player player, bool isActive)
    {
        if (!isActive)
        {
            return;
        }
        player.ManageSpecialBiomeVisuals("HeatDistortion", false);
    }
}

public class FrozenHellTileCount : ModSystem
{
    public int frozenAshCount { get; private set; }

    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
    {
        frozenAshCount = tileCounts[ModContent.TileType<FrozenAsh>()];
        frozenAshCount -= tileCounts[TileID.Ash];
    }
}
