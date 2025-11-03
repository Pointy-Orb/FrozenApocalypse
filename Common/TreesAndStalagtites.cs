using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.Enums;
using System;
using Terraria.ModLoader;
using Terraria.ID;
using static Mono.Cecil.Cil.OpCodes;
using MonoMod.Cil;
using FrozenApocalypse.Content.Tiles;

namespace FrozenApocalypse;

public class TreesAndStalagtites : ModSystem
{
    public override void Load()
    {
        IL_WorldGen.CheckTree += MakeModTilesValidForTrees;
        On_WorldGen.GetTreeLeaf += RightTreeLeaves;
        On_WorldGen.GetTreeType += RightTreeType;
        On_WorldGen.GetCommonTreeFoliageData += TreeData;
        On_WorldGen.KillTile_GetTreeDrops += DropBorealPlease;
        On_TileDrawing.GetTreeVariant += GetTreeVariant;
    }

    private static void MakeModTilesValidForTrees(ILContext il)
    {
        try
        {
            var c = new ILCursor(il);
            var targetLabel = c.DefineLabel();
            var varIndex = 3;
            c.GotoNext(i => i.MatchLdcI4(23));
            c.GotoPrev(i => i.MatchLdloc(varIndex));
            c.GotoNext(MoveType.After, i => i.MatchBeq(out targetLabel));
            c.Emit(Ldloc, varIndex);
            c.EmitDelegate<Func<int, bool>>((providedType) =>
            {
                if (!TileID.Sets.Snow.IndexInRange(providedType))
                {
                    return false;
                }
                return TileID.Sets.Snow[providedType];
            });
            c.Emit(Brtrue_S, targetLabel);
        }
        catch
        {
            MonoModHooks.DumpIL(ModContent.GetInstance<FrozenApocalypse>(), il);
        }
    }

    private static void RightTreeLeaves(On_WorldGen.orig_GetTreeLeaf orig, int x, Tile topTile, Tile t, ref int treeHeight, out int treeFrame, out int passStyle)
    {
        treeFrame = 0;
        passStyle = -1;
        orig(x, topTile, t, ref treeHeight, out treeFrame, out passStyle);
        if (TileID.Sets.Snow[t.TileType])
        {
            passStyle = 913;
            treeFrame += 10;
        }
    }

    private static TreeTypes RightTreeType(On_WorldGen.orig_GetTreeType orig, int tileType)
    {
        if (TileID.Sets.Snow[tileType])
        {
            return TreeTypes.Snow;
        }
        return orig(tileType);
    }

    private static void StalagtiteStyle(On_WorldGen.orig_GetDesiredStalagtiteStyle orig, int x, int j, out bool fail, out int desiredStyle, out int height, out int y)
    {
        fail = false;
        desiredStyle = 0;
        height = 1;
        y = j;
        orig(x, j, out fail, out desiredStyle, out height, out y);
        if (fail && TileID.Sets.Snow[desiredStyle])
        {
            fail = false;
            desiredStyle = 7;
        }
    }

    private static bool TreeData(On_WorldGen.orig_GetCommonTreeFoliageData orig, int i, int j, int xOffset, ref int treeFrame, ref int treeStyle, out int floorY, out int topTextureFrameWidth, out int topTextureFrameHeight)
    {
        floorY = j;
        topTextureFrameWidth = 80;
        topTextureFrameHeight = 80;
        var flag = orig(i, j, xOffset, ref treeFrame, ref treeStyle, out floorY, out topTextureFrameWidth, out topTextureFrameHeight);
        if (flag)
        {
            return true;
        }
        int k = 0;
        while (k < 100)
        {
            floorY = k + j;
            if (!WorldGen.InWorld(i + xOffset, floorY))
            {
                return false;
            }
            Tile tile = Main.tile[i + xOffset, floorY];
            if (TileID.Sets.Snow[tile.TileType])
            {
                treeStyle = 4;
                var num2 = WorldGen.TreeTops.GetTreeStyle(6);
                if (num2 == 0)
                {
                    treeStyle = 12;
                    if (i % 10 == 0)
                    {
                        treeStyle = 18;
                    }
                }
                if (num2 == 2 || num2 == 3 || num2 == 32 || num2 == 4 || num2 == 42 || num2 == 5 || num2 == 7)
                {
                    if (num2 % 2 == 0)
                    {
                        if (i < Main.maxTilesX / 2)
                        {
                            treeStyle = 16;
                        }
                        else
                        {
                            treeStyle = 17;
                        }
                    }
                    else if (i > Main.maxTilesX / 2)
                    {
                        treeStyle = 16;
                    }
                    else
                    {
                        treeStyle = 17;
                    }
                }
                return true;
            }
            k++;
        }
        return false;
    }

    private static void DropBorealPlease(On_WorldGen.orig_KillTile_GetTreeDrops orig, int i, int j, Tile tileCache, ref bool bonusWood, ref int dropItem, ref int secondaryItem)
    {
        orig(i, j, tileCache, ref bonusWood, ref dropItem, ref secondaryItem);
        WorldGen.GetTreeBottom(i, j, out int x, out int y);
        if (Main.tile[x, y].HasTile && TileID.Sets.Snow[Main.tile[x, y].TileType])
        {
            dropItem = ItemID.BorealWood;
        }
    }

    private static int GetTreeVariant(On_TileDrawing.orig_GetTreeVariant orig, int x, int y)
    {
        var variant = orig(x, y);
        if (variant != -1)
        {
            return variant;
        }
        if (!WorldGen.InWorld(x, y) || !Main.tile[x, y].HasTile)
        {
            return -1;
        }
        if (TileID.Sets.Snow[Main.tile[x, y].TileType])
        {
            return 3;
        }
        return -1;
    }
}
