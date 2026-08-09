using Terraria;
using System;
using Terraria.ModLoader;
using Terraria.ID;
using FrozenApocalypse.Content.TileEntities;
using Terraria.Graphics.Effects;
using FrozenApocalypse.Content.Buffs;

namespace FrozenApocalypse;

public class ColdDebuffPlayer : ModPlayer
{
    public int numZoneBoilers = 0;
    public bool boilerWarm => numZoneBoilers > 0;
    public bool wasBoilerWarm = false;

    public int ColdLevel { get; set; } = 0;
    public int WarmthLevel { get; set; } = 0;
    public int NetColdLevel => Math.Max(ColdLevel - WarmthLevel, 0);
    internal bool handWarmer = false;

    private int visualColdLevel = 0;
    private int VisualColdLevelTarget => NetColdLevel > 0 ? (coldLevelInternval * NetColdLevel) : -180;
    private const int coldLevelInternval = 60;

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
        if (ColdLevel > 0 && WarmthLevel < 1)
        {
            Player.AddBuff(BuffID.Chilled, 20);
        }
        if (NetColdLevel >= Hypothermia.MinColdLevel)
        {
            Player.AddBuff(ModContent.BuffType<Hypothermia>(), 20);
        }
    }

    public override void PostUpdate()
    {
        if (Main.dedServ)
        {
            return;
        }
        if (Player.whoAmI != Main.myPlayer)
        {
            return;
        }
        if (NetColdLevel > 0 && !Filters.Scene["FrozenApocalypse:ColdFilter"].Active)
        {
            Filters.Scene.Activate("FrozenApocalypse:ColdFilter");
        }
        if (NetColdLevel <= 0 && Filters.Scene["FrozenApocalypse:ColdFilter"].Active)
        {
            Filters.Scene["FrozenApocalypse:ColdFilter"].Deactivate();
        }
        if (!Filters.Scene["FrozenApocalypse:ColdFilter"].Active)
        {
            return;
        }
        UpdateFilter();
    }

    private void UpdateFilter()
    {
        if (visualColdLevel > VisualColdLevelTarget)
        {
            visualColdLevel--;
        }
        if (visualColdLevel < VisualColdLevelTarget)
        {
            visualColdLevel++;
        }
        if (VisualColdLevelTarget - visualColdLevel > 60)
        {
            visualColdLevel += 2;
        }
        if (VisualColdLevelTarget - visualColdLevel < -60)
        {
            visualColdLevel -= 2;
        }
        visualColdLevel = Math.Min(visualColdLevel, 420);
        int stackedLevels = visualColdLevel / coldLevelInternval;
        int remainderLevel = visualColdLevel % coldLevelInternval;
        float transitionAmount;
        if (visualColdLevel > VisualColdLevelTarget)
        {
            transitionAmount = MathF.Pow((float)remainderLevel / (float)coldLevelInternval, 2f);
        }
        else
        {
            transitionAmount = 1 - MathF.Pow(1 - ((float)remainderLevel / (float)coldLevelInternval), 2f);
        }
        Filters.Scene["FrozenApocalypse:ColdFilter"].GetShader().UseProgress((float)stackedLevels * 0.05f + transitionAmount * 0.05f - 0.8f);
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
            WarmthLevel += 2 + ColdLevel / 2;
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
                }
            }
        }
        wasBoilerWarm = boilerWarm;
        if (boilerWarm)
        {
            Player.AddBuff(ModContent.BuffType<BoilerBuff>(), 20, true);
        }
    }
}

