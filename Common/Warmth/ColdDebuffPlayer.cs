using Terraria;
using Terraria.Audio;
using System;
using Terraria.ModLoader;
using Terraria.ID;
using FrozenApocalypse.Content.TileEntities;
using Terraria.DataStructures;
using FrozenApocalypse.Content.Buffs;

namespace FrozenApocalypse.Warmth;

public class ColdDebuffPlayer : ModPlayer
{
    public int numZoneBoilers = 0;
    public bool boilerWarm => numZoneBoilers > 0;
    public bool wasBoilerWarm = false;

    public int ColdLevel { get; set; } = 0;
    public int WarmthLevel { get; set; } = 0;
    public int NetColdLevel => Math.Max(ColdLevel - WarmthLevel, 0);
    internal bool handWarmer = false;

    public override void ResetEffects()
    {
        handWarmer = false;
        WarmthLevel = 0;
        ColdLevel = 0;
        numZoneBoilers = 0;
    }

    public override void PreUpdateBuffs()
    {
        ManageBoiler();
    }

    public override void PostUpdateEquips()
    {
        CalculateColdLevel();
        CalculateWarmthLevel();
        if ((ColdLevel > 0 && WarmthLevel < 1))
        {
            Player.AddBuff(BuffID.Chilled, 20);
        }
        if (NetColdLevel >= 5 && WarmthLevel <= 9)
        {
            Player.buffImmune[BuffID.Chilled] = false;
            Player.AddBuff(BuffID.Chilled, 20);
        }
        if (NetColdLevel >= Hypothermia.MinColdLevel)
        {
            Player.AddBuff(ModContent.BuffType<Hypothermia>(), 20);
        }
        if (NetColdLevel >= Frostnip.MinColdLevel)
        {
            Player.AddBuff(ModContent.BuffType<Frostnip>(), 20);
        }
    }

    private void CalculateColdLevel()
    {
        int playerY = Player.Center.ToTileCoordinates().Y;
        if (playerY > TileFreezing.UpperBand)
        {
            return;
        }
        if (Player.ZoneDesert && Player.ZoneOverworldHeight && Main.dayTime)
        {
            return;
        }
        if (Player.lavaWet)
        {
            return;
        }
        ColdLevel = 1 + (TileFreezing.UpperBand - playerY) / TileFreezing.BandHeight;
    }

    private void CalculateWarmthLevel()
    {
        if (Player.HasBuff(BuffID.Warmth))
        {
            WarmthLevel += 4;
        }
        WarmthLevel += 4 * numZoneBoilers;
        if (Player.HasBuff(BuffID.Campfire))
        {
            WarmthLevel += 1;
        }
        if (Player.HasBuff(BuffID.Tipsy))
        {
            WarmthLevel += 1;
        }
        if (ItemID.Sets.Torches[Player.HeldItem.type] && (!Player.wet || ItemID.Sets.WaterTorches[Player.HeldItem.type]))
        {
            WarmthLevel += 1;
        }
        if (handWarmer)
        {
            WarmthLevel += 4;
        }
        if (Player.armor[0].type == ItemID.EskimoHood || Player.armor[0].type == ItemID.PinkEskimoHood)
        {
            WarmthLevel += 2;
        }
        if (Player.armor[1].type == ItemID.EskimoCoat || Player.armor[1].type == ItemID.PinkEskimoCoat)
        {
            WarmthLevel += 3;
        }
        if (Player.armor[2].type == ItemID.EskimoPants || Player.armor[2].type == ItemID.PinkEskimoPants)
        {
            WarmthLevel += 1;
        }
    }

    private void ManageBoiler()
    {
        foreach (BoilerEntity boiler in BoilerSystem.boilers)
        {
            if (boiler.EntityInRange(Player.Center))
            {
                numZoneBoilers++;
                if (!wasBoilerWarm)
                {
                    boiler.DrawRange();
                    SoundEngine.PlaySound(SoundID.MaxMana);
                }
            }
        }
        wasBoilerWarm = boilerWarm;
        if (boilerWarm)
        {
            Player.AddBuff(ModContent.BuffType<BoilerBuff>(), 20, true);
        }
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        r = Utils.Remap(NetColdLevel, 0, 10, r, r * 0.3f, true);
        g = Utils.Remap(NetColdLevel, 0, 10, g, g * 0.5f, true);
        b = Utils.Remap(NetColdLevel, 0, 10, b, 1, true);
    }
}

