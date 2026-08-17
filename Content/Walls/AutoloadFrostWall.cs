using Terraria;
using System.Linq;
using System.Collections.Generic;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System.Reflection;
using System;

namespace FrozenApocalypse.Content.Walls;

public class AutoloadFrostWall : ModWall
{
    public readonly int UnfrozenCounterpart;
    private readonly int fallback;
    public bool Hot { get; }

    public override string Name => $"Frosted{WallID.Search.GetName(UnfrozenCounterpart)}Wall";

    public override string Texture => textureOverride ?? $"Terraria/Images/Wall_{UnfrozenCounterpart}";
    private readonly string textureOverride;

    private readonly Color? mapColorOverride = null;

    public AutoloadFrostWall(int originalType, int fallback = -1, bool hot = false, string customTexture = null, Color? mapColorOverride = null)
    {
        UnfrozenCounterpart = originalType;
        this.fallback = fallback;
        Hot = hot;
        textureOverride = customTexture;
        this.mapColorOverride = mapColorOverride;
    }

    public override void SetStaticDefaults()
    {
        TileFreezing.FreezableWalls.Add(UnfrozenCounterpart, Type);
        if (fallback > 0)
        {
            TileFreezing.FreezableWalls.Add(fallback, Type);
        }

        WallLoader.RegisterConversionFallback(Type, UnfrozenCounterpart);
        VanillaFallbackOnModDeletion = (ushort)UnfrozenCounterpart;

        WallID.Sets.Corrupt[Type] = WallID.Sets.Corrupt[UnfrozenCounterpart];
        WallID.Sets.Crimson[Type] = WallID.Sets.Crimson[UnfrozenCounterpart];
        WallID.Sets.Hallow[Type] = WallID.Sets.Hallow[UnfrozenCounterpart];

        if (mapColorOverride != null)
        {
            AddMapEntry(mapColorOverride.Value);
            return;
        }

        Color[] ColorLookup = (Color[])typeof(MapHelper).GetField("colorLookup", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        Color frostedColor = ColorLookup[MapHelper.wallLookup[UnfrozenCounterpart]];
        frostedColor.R = (byte)Math.Min((float)frostedColor.R + Color.Aqua.R * 0.2f, (float)byte.MaxValue);
        frostedColor.G = (byte)Math.Min((float)frostedColor.G + Color.Aqua.G * 0.2f, (float)byte.MaxValue);
        frostedColor.B = (byte)Math.Min((float)frostedColor.B + Color.Aqua.B * 0.2f, (float)byte.MaxValue);
        frostedColor.R = (byte)Math.Min((float)frostedColor.R + Color.Blue.R * 0.2f, (float)byte.MaxValue);
        frostedColor.G = (byte)Math.Min((float)frostedColor.G + Color.Blue.G * 0.2f, (float)byte.MaxValue);
        frostedColor.B = (byte)Math.Min((float)frostedColor.B + Color.Blue.B * 0.2f, (float)byte.MaxValue);
        AddMapEntry(frostedColor);
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
        if (textureOverride != null)
        {
            return;
        }
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);
        Tile tile = Main.tile[i, j];

        Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
        Rectangle srect = new Rectangle(tile.WallFrameX, tile.WallFrameY + Main.wallFrame[tile.WallType] * 180, 32, 32);

        Vector2 pos = new Vector2(i * 16 - (int)Main.screenPosition.X - 8, j * 16 - (int)Main.screenPosition.Y - 8) + zero;
        spriteBatch.Draw(
            TextureAssets.Wall[Type].Value,
            pos,
            srect,
            Lighting.GetColor(i, j).MultiplyRGB(Color.Aqua) * 0.7f, 0f, default, 1f, SpriteEffects.None, 0f);
        spriteBatch.Draw(
            TextureAssets.Wall[Type].Value,
            pos,
            srect,
            Lighting.GetColor(i, j).MultiplyRGB(Color.Blue) * 0.7f, 0f, default, 1f, SpriteEffects.None, 0f);
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);
    }

    public override void OnWallConverted(int i, int j, int fromType, int toType, int conversionType)
    {
        if (fromType != Type)
        {
            return;
        }
        TileFreezing.AttemptWallFreeze(i, j, false);
    }
}

public class AutoloadWallLoader : ILoadable
{
    public void Load(Mod mod)
    {
        mod.AddContent(new AutoloadFrostWall(WallID.HiveUnsafe, WallID.Hive));
        mod.AddContent(new AutoloadFrostWall(WallID.LivingWoodUnsafe, WallID.LivingWood));
        mod.AddContent(new AutoloadFrostWall(WallID.SpiderEcho, WallID.SpiderUnsafe));
        for (int i = 0; i < WallLoader.WallCount; i++)
        {
            if (i < WallID.Sets.Conversion.Sandstone.Length && WallID.Sets.Conversion.Sandstone[i])
            {
                mod.AddContent(new AutoloadFrostWall(i, -1, true));
            }
            if (i < WallID.Sets.Conversion.HardenedSand.Length && WallID.Sets.Conversion.HardenedSand[i])
            {
                mod.AddContent(new AutoloadFrostWall(i, -1, true));
            }
        }
        mod.AddContent(new AutoloadFrostWall(WallID.EbonstoneUnsafe, WallID.EbonstoneEcho, false, "FrozenApocalypse/Content/Walls/EbonfrostWall"));
        mod.AddContent(new AutoloadFrostWall(WallID.CrimstoneUnsafe, WallID.CrimstoneEcho, false, "FrozenApocalypse/Content/Walls/CrimfrostWall", new Color(10, 0, 0)));
        mod.AddContent(new AutoloadFrostWall(WallID.PearlstoneBrickUnsafe, WallID.PearlstoneEcho, false, "FrozenApocalypse/Content/Walls/PearlfrostWall"));
    }

    public void Unload()
    {

    }
}
