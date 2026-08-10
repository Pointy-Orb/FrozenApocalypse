using Terraria;
using Terraria.ModLoader;
using Terraria.Enums;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using ReLogic.Content;

namespace FrozenApocalypse;

public class ColdSceneEffect : ModSceneEffect
{
    public static ScreenShaderData coldShader;

    public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

    private int visualColdLevel = 0;
    internal const int coldLevelInternval = 60;
    private const float coldStageIncrement = 0.08f;
    private const float coldOffset = 0.7f;

    private const float baseIntensity = 0.4f;

    public override void Load()
    {
        Asset<Effect> coldFilter = Mod.Assets.Request<Effect>("Assets/Effects/ColdShader");
        Asset<Texture2D> noise = Mod.Assets.Request<Texture2D>("Assets/Noise");
        coldShader = new ScreenShaderData(coldFilter, "ColdFilter")
            .UseColor(Color.Aqua)
            .UseSecondaryColor(Color.DodgerBlue)
            .UseImage(noise)
            .UseIntensity(baseIntensity)
            .UseOpacity(0.2f);
        Filters.Scene["FrozenApocalypse:ColdFilter"] = new Filter(coldShader, EffectPriority.Medium);
    }

    public override bool IsSceneEffectActive(Player player)
    {
        return player.GetModPlayer<ColdDebuffPlayer>().NetColdLevel > 0;
    }

    public override void SpecialVisuals(Player player, bool isActive)
    {
        player.ManageSpecialBiomeVisuals("FrozenApocalypse:ColdFilter", isActive);
        int visualColdLevelTarget = VisualColdLevelTargetForPlayer(player);
        if (visualColdLevel > visualColdLevelTarget)
        {
            visualColdLevel--;
        }
        if (visualColdLevel < visualColdLevelTarget)
        {
            visualColdLevel++;
        }
        if (visualColdLevelTarget - visualColdLevel > 60)
        {
            visualColdLevel += 2;
        }
        if (visualColdLevelTarget - visualColdLevel < -60)
        {
            visualColdLevel -= 2;
        }
        int extraVisualColdLevel = Math.Max(0, visualColdLevel - 420);
        int screenCoverLevel = Math.Min(visualColdLevel, 420);
        int stackedLevels = screenCoverLevel / coldLevelInternval;
        int remainderLevel = screenCoverLevel % coldLevelInternval;
        float transitionAmount;
        if (screenCoverLevel > visualColdLevelTarget)
        {
            transitionAmount = MathF.Pow((float)remainderLevel / (float)coldLevelInternval, 2f);
        }
        else
        {
            transitionAmount = 1 - MathF.Pow(1 - ((float)remainderLevel / (float)coldLevelInternval), 2f);
        }
        coldShader.UseProgress((float)stackedLevels * coldStageIncrement + transitionAmount * coldStageIncrement - coldOffset);
        coldShader.UseIntensity(Math.Clamp(baseIntensity + (float)extraVisualColdLevel / (coldLevelInternval * 10), 0, 1));
    }

    private int VisualColdLevelTargetForPlayer(Player player) => player.GetModPlayer<ColdDebuffPlayer>().NetColdLevel > 0 ? (ColdSceneEffect.coldLevelInternval * player.GetModPlayer<ColdDebuffPlayer>().NetColdLevel) : -180;
}
