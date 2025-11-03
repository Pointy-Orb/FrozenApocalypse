using Terraria;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Light;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Graphics.Effects;
using System;
using MonoMod.Cil;
using static Mono.Cecil.Cil.OpCodes;
using Terraria.ModLoader;
using Terraria.ID;

namespace FrozenApocalypse.Content.Biomes;

//This code was adapted from the Depths Mod, full of people who are better at coding than me
public class FrozenHellBackground : ModSystem
{
    static float[] UWBGAlpha = new float[2];
    private static int UWBGStyle;
    private static Asset<Texture2D>[][] UWBGTexture = new Asset<Texture2D>[2][];
    static Color[] UWBGBottomColor = new Color[2];

    public override void Load()
    {
        IL_Main.DrawBG += UWBGInsert;
        IL_Main.DrawCapture += UWBGInsertCapture;
        On_TileLightScanner.ApplyHellLight += ReduceFrozenHellLight;
    }

    public override void Unload()
    {
        IL_Main.DrawBG -= UWBGInsert;
        IL_Main.DrawCapture -= UWBGInsertCapture;
        On_TileLightScanner.ApplyHellLight -= ReduceFrozenHellLight;
    }

    public override void PostSetupContent()
    {
        if (!Main.dedServ)
        {
            UWBGTexture[0] = new Asset<Texture2D>[14];
            UWBGTexture[1] = new Asset<Texture2D>[14];
            for (int i = 0; i < 14; i++)
            {
                UWBGTexture[0][i] = TextureAssets.Underworld[i];
                UWBGTexture[1][i] = ModContent.Request<Texture2D>("FrozenApocalypse/Assets/Backgrounds/FrozenHell/BG_" + i);
            }
            UWBGBottomColor[0] = new Color(11, 3, 7);
            UWBGBottomColor[1] = new Color(2, 1, 6);
        }
    }

    private static void UWBGInsert(ILContext il)
    {
        ILCursor c = new ILCursor(il);
        c.GotoNext(MoveType.After, i => i.MatchLdarg0(), i => i.MatchLdcI4(0), i => i.MatchCall<Main>("DrawUnderworldBackground"));
        c.EmitDelegate(() =>
        {
            DrawUnderworldBackground(false);
        });
    }

    private static void UWBGInsertCapture(ILContext il)
    {
        ILCursor c = new ILCursor(il);
        c.GotoNext(MoveType.After, i => i.MatchLdarg0(), i => i.MatchLdcI4(1), i => i.MatchCall<Main>("DrawUnderworldBackground"));
        c.EmitDelegate(() =>
        {
            DrawUnderworldBackground(true);
        });
    }

    private static void DrawUnderworldBackground(bool flat)
    {
        if (Main.screenPosition.Y + (float)Main.screenHeight < (float)(Main.maxTilesY - 200) * 16f)
        {
            return;
        }

        UWBGStyle = FrozenHell.PlayerInBiomeForVisuals(Main.LocalPlayer) ? 1 : 0;
        //Fade the background in or out if it changes
        for (int i = 0; i < 2; i++)
        {
            if (UWBGStyle != i)
            {
                UWBGAlpha[i] = Math.Max(UWBGAlpha[i] - 0.05f, 0f);
            }
            else
            {
                UWBGAlpha[i] = Math.Min(UWBGAlpha[i] + 0.05f, 1f);
            }
        }
        // >>: bitwise shift right. Shifts the bits of the left number by the right number, effectively dividing it by 2^n
        Vector2 screenOffset = Main.screenPosition + new Vector2((float)(Main.screenWidth >> 1), (float)(Main.screenHeight >> 1));
        float pushUp = (Main.GameViewMatrix.Zoom.Y - 1f) * 0.5f * 200f;
        SkyManager.Instance.ResetDepthTracker();
        for (int i = 4; i >= 0; i--)
        {
            bool flag = false;
            for (int j = 0; j < 2; j++)
            {
                if (UWBGAlpha[j] > 0f && j != UWBGStyle)
                {
                    DrawUnderworldBackgroundLayer(flat, screenOffset, pushUp, i, j, flat ? 1f : UWBGAlpha[j]);
                    flag = true;
                }
            }
            DrawUnderworldBackgroundLayer(flat, screenOffset, pushUp, i, UWBGStyle, flag ? UWBGAlpha[UWBGStyle] : 1f);
        }
        if (!Main.mapFullscreen)
        {
            SkyManager.Instance.DrawRemainingDepth(Main.spriteBatch);
        }
    }

    private static void DrawUnderworldBackgroundLayer(bool flat, Vector2 screenOffset, float pushUp, int layerTextureIndex, int Style, float Alpha)
    {
        if (Style == 0)
        {
            return;
        }
        int num = Main.underworldBG[layerTextureIndex];
        Asset<Texture2D> asset = UWBGTexture[Style][num];
        if (!asset.IsLoaded)
        {
            Main.Assets.Request<Texture2D>(asset.Name);
        }
        Texture2D value = asset.Value;
        Vector2 vec = new Vector2((float)value.Width, (float)value.Height) * 0.5f;
        float num7 = (flat ? 1f : ((float)(layerTextureIndex * 2) + 3f));
        Vector2 vector = new(1f / num7);
        Rectangle value2 = new(0, 0, value.Width, value.Height);
        float num8 = 1.3f;
        Vector2 zero = Vector2.Zero;
        int num9 = 0;
        switch (num)
        {
            case 1:
                {
                    int num14 = (int)(Main.GlobalTimeWrappedHourly * 8f) % 4;
                    value2 = new((num14 >> 1) * (value.Width >> 1), num14 % 2 * (value.Height >> 1), value.Width >> 1, value.Height >> 1);
                    vec *= 0.5f;
                    zero.Y += 175f;
                    break;
                }
            case 2:
                zero.Y += 100f;
                break;
            case 3:
                zero.Y += 75f;
                break;
            case 4:
                num8 = 0.5f;
                zero.Y -= 0f;
                break;
            case 5:
                zero.Y += num9;
                break;
            case 6:
                {
                    int num13 = (int)(Main.GlobalTimeWrappedHourly * 8f) % 4;
                    value2 = new(num13 % 2 * (value.Width >> 1), (num13 >> 1) * (value.Height >> 1), value.Width >> 1, value.Height >> 1);
                    vec *= 0.5f;
                    zero.Y += num9;
                    zero.Y += -60f;
                    break;
                }
            case 7:
                {
                    int num12 = (int)(Main.GlobalTimeWrappedHourly * 8f) % 4;
                    value2 = new(num12 % 2 * (value.Width >> 1), (num12 >> 1) * (value.Height >> 1), value.Width >> 1, value.Height >> 1);
                    vec *= 0.5f;
                    zero.Y += num9;
                    zero.X -= 400f;
                    zero.Y += 90f;
                    break;
                }
            case 8:
                {
                    int num11 = (int)(Main.GlobalTimeWrappedHourly * 8f) % 4;
                    value2 = new(num11 % 2 * (value.Width >> 1), (num11 >> 1) * (value.Height >> 1), value.Width >> 1, value.Height >> 1);
                    vec *= 0.5f;
                    zero.Y += num9;
                    zero.Y += 90f;
                    break;
                }
            case 9:
                zero.Y += num9;
                zero.Y -= 30f;
                break;
            case 10:
                zero.Y += 250f * num7;
                break;
            case 11:
                zero.Y += 100f * num7;
                break;
            case 12:
                zero.Y += 20f * num7;
                break;
            case 13:
                {
                    zero.Y += 20f * num7;
                    int num10 = (int)(Main.GlobalTimeWrappedHourly * 8f) % 4;
                    value2 = new(num10 % 2 * (value.Width >> 1), (num10 >> 1) * (value.Height >> 1), value.Width >> 1, value.Height >> 1);
                    vec *= 0.5f;
                    break;
                }
        }
        if (flat)
        {
            num8 *= 1.5f;
        }
        vec *= num8;
        SkyManager.Instance.DrawToDepth(Main.spriteBatch, 1f / vector.X);
        if (flat)
        {
            zero.Y += (float)(UWBGTexture[Style][0].Height() >> 1) * 1.3f - vec.Y;
        }
        zero.Y -= pushUp;
        float num2 = num8 * (float)value2.Width;
        int num3 = (int)((float)(int)(screenOffset.X * vector.X - vec.X + zero.X - (float)(Main.screenWidth >> 1)) / num2);
        vec = vec.Floor();
        int num4 = (int)Math.Ceiling((float)Main.screenWidth / num2);
        int num5 = (int)(num8 * ((float)(value2.Width - 1) / vector.X));
        Vector2 vec2 = (new Vector2((float)((num3 - 2) * num5), (float)Main.UnderworldLayer * 16f) + vec - screenOffset) * vector + screenOffset - Main.screenPosition - vec + zero;
        vec2 = vec2.Floor();
        while (vec2.X + num2 < 0f)
        {
            num3++;
            vec2.X += num2;
        }
        for (int i = num3 - 2; i <= num3 + 4 + num4; i++)
        {
            Color color = Color.Lerp(Color.White, Color.DarkSlateGray, Utils.GetLerpValue(FrozenHell.minimumAsh, FrozenHell.minimumAsh + 60, (float)ModContent.GetInstance<FrozenHellTileCount>().frozenAshCount));
            float num16 = (float)(int)color.R * Alpha;
            float num17 = (float)(int)color.G * Alpha;
            float num18 = (float)(int)color.B * Alpha;
            float num19 = (float)(int)color.A * Alpha;
            color = new((int)(byte)num16, (int)(byte)num17, (int)(byte)num18, (int)(byte)num19);

            Color color2 = UWBGBottomColor[Style];
            float num116 = (float)(int)color2.R * Alpha;
            float num117 = (float)(int)color2.G * Alpha;
            float num118 = (float)(int)color2.B * Alpha;
            float num119 = (float)(int)color2.A * Alpha;
            color2 = new((int)(byte)num116, (int)(byte)num117, (int)(byte)num118, (int)(byte)num119);

            Main.spriteBatch.Draw(value, vec2, (Rectangle?)value2, color, 0f, Vector2.Zero, num8, (SpriteEffects)0, 0f);
            if (layerTextureIndex == 0)
            {
                int num6 = (int)(vec2.Y + (float)value2.Height * num8);
                Main.spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle((int)vec2.X, num6, (int)((float)value2.Width * num8), Math.Max(0, Main.screenHeight - num6)), color2);
            }
            vec2.X += num2;
        }
    }

    private static void ReduceFrozenHellLight(On_TileLightScanner.orig_ApplyHellLight orig, TileLightScanner self, Tile tile, int x, int y, ref Vector3 lightColor)
    {
        orig(self, tile, x, y, ref lightColor);
        if (!FrozenHell.PlayerInBiomeForVisuals(Main.LocalPlayer))
        {
            return;
        }
        var frozenHellLightColor = new Vector3(0f, 0f, 0.05f);
        if ((!tile.HasTile || !Main.tileNoSunLight[tile.TileType] || ((tile.Slope != 0 || tile.IsHalfBlock) && Main.tile[x, y - 1].LiquidAmount == 0 && Main.tile[x, y + 1].LiquidAmount == 0 && Main.tile[x - 1, y].LiquidAmount == 0 && Main.tile[x + 1, y].LiquidAmount == 0)) && (Main.wallLight[tile.WallType] || tile.WallType == 73 || tile.WallType == 227) && tile.LiquidAmount < 200 && (!tile.IsHalfBlock || Main.tile[x, y - 1].LiquidAmount < 200))
        {
            lightColor = frozenHellLightColor;
        }
    }
}
