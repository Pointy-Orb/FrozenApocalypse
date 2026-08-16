using Terraria;
using FrozenApocalypse.Content.TileEntities;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using FrozenApocalypse.Content.Tiles;
using FrozenApocalypse.Content.Walls;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader.IO;
using System.Threading;

namespace FrozenApocalypse;

public class TileFreezing : ModSystem
{
    public static int BandHeight => Main.maxTilesY / 20;

    public static int UpperBand => bandY - BandHeight;

    public static int bandY = BandHeight;

    public static int ConvertsPerUpdate => Main.maxTilesY / 600;

    public static Dictionary<int, int> FreezableTiles = new();
    public static Dictionary<int, int> FreezableWalls = new();

    bool wasDay = true;

    public override void ClearWorld()
    {
        bandY = BandHeight * 2;
        wasDay = true;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        if (bandY > BandHeight * 2)
        {
            tag["bandY"] = bandY;
        }
    }

    public override void LoadWorldData(TagCompound tag)
    {
        if (tag.ContainsKey("bandY"))
        {
            bandY = tag.GetInt("bandY");
        }
    }

    public override void PostUpdateTime()
    {
        if (UpperBand > Main.maxTilesY)
        {
            return;
        }
        if (Main.dayTime && !wasDay)
        {
            bandY += BandHeight / 2;
            ThreadPool.QueueUserWorkItem(_ => CoalRunner.GenerateCoal(UpperBand - BandHeight, UpperBand));
        }
        for (int i = 0; i < ConvertsPerUpdate; i++)
        {
            int x = Main.rand.Next(0, Main.maxTilesX);
            int y = Main.rand.Next(UpperBand, bandY);
            AttemptTileFreeze(x, y, false);
            x = Main.rand.Next(0, Main.maxTilesX);
            y = Main.rand.Next(bandY + BandHeight / 2, bandY + BandHeight);
            if (Main.tile[x, y].HasTile && WorldGen.TileIsExposedToAir(x, y))
            {
                AttemptTileFreeze(x, y, false);
            }
        }
        wasDay = Main.dayTime;
    }

    public static void AttemptTileFreeze(int x, int y, bool noPeat = false)
    {
        Tile tile = Main.tile[x, y];
        foreach (BoilerEntity boiler in BoilerSystem.boilers)
        {
            if (boiler.TileInRange(x, y))
            {
                return;
            }
        }
        AttemptWallFreeze(x, y);
        if (!tile.HasTile)
        {
            return;
        }
        if (!FreezableTiles.ContainsKey(tile.TileType))
        {
            if (TileID.Sets.Leaves[tile.TileType])
            {
                WorldGen.KillTile(x, y);
            }
            if (TileID.Sets.Grass[tile.TileType])
            {
                tile.TileType = TileID.Dirt;
                AttemptTileFreeze(x, y);
            }
            if (Main.tileMoss[tile.TileType])
            {
                tile.TileType = TileID.Stone;
                AttemptTileFreeze(x, y);
            }
            return;
        }
        if (ModContent.GetModTile(FreezableTiles[tile.TileType]) is FrozenTile frozenTile && frozenTile.Hot && y > UpperBand)
        {
            return;
        }
        if (ModContent.GetModTile(FreezableTiles[tile.TileType]) is AutoloadFrostTile autoloadFrozenTile && autoloadFrozenTile.Hot && y > UpperBand)
        {
            return;
        }
        tile.TileType = (ushort)FreezableTiles[tile.TileType];
        WorldGen.SquareTileFrame(x, y);
        if (!WorldGen.InWorld(x, y - 1))
        {
            return;
        }
        var aboveTile = Main.tile[x, y - 1];
        if (aboveTile.HasTile && Main.tileCut[aboveTile.TileType])
        {
            WorldGen.KillTile(x, y - 1);
        }
        if (!WorldGen.InWorld(x, y + 1))
        {
            return;
        }
        var belowTile = Main.tile[x, y + 1];
        if (belowTile.HasTile && Main.tileCut[belowTile.TileType])
        {
            WorldGen.KillTile(x, y + 1);
        }
    }

    public static void AttemptWallFreeze(int x, int y)
    {
        foreach (BoilerEntity boiler in BoilerSystem.boilers)
        {
            if (boiler.TileInRange(x, y))
            {
                return;
            }
        }
        if (Main.tile[x, y].LiquidAmount > 0)
        {
            TryFreezeLiquid(x, y);
        }
        Tile tile = Main.tile[x, y];
        if (!FreezableWalls.ContainsKey(tile.WallType))
        {
            return;
        }
        if (ModContent.GetModWall(FreezableWalls[tile.WallType]) is AutoloadFrostWall frozenWall && frozenWall.Hot && y > UpperBand)
        {
            return;
        }
        tile.WallType = (ushort)FreezableWalls[tile.WallType];
    }

    private static void TryFreezeLiquid(int x, int y)
    {
        Tile tile = Main.tile[x, y];
        if (tile.LiquidAmount <= 0)
        {
            return;
        }
        if (tile.LiquidType == LiquidID.Water || tile.LiquidType == LiquidID.Honey)
        {
            int iceType = tile.LiquidType == LiquidID.Honey ? ModContent.GetInstance<FrozenApocalypse>().Find<ModTile>("FrostedHoneyBlock").Type : TileID.BreakableIce;
            int j = y;
            while (WorldGen.InWorld(x, j - 1) && Main.tile[x, j - 1].LiquidAmount > 0 && !(Main.tile[x, j - 1].HasTile && Main.tile[x, j].LiquidAmount >= byte.MaxValue))
            {
                j--;
            }
            if (Main.tile[x, j - 1].HasTile && Main.tile[x, j].LiquidAmount >= byte.MaxValue)
            {
                return;
            }
            if (BoilerSystem.TileInBoilerRange(x, j))
            {
                return;
            }
            Main.tile[x, j].LiquidAmount = 0;
            WorldGen.PlaceTile(x, j, iceType, true);
        }
        if (tile.LiquidType == LiquidID.Lava)
        {
            int lowerBound = y;
            int upperBound = y;
            while (WorldGen.InWorld(x, upperBound - 1) && Main.tile[x, upperBound - 1].LiquidAmount > 0)
            {
                if (!BoilerSystem.TileInBoilerRange(x, upperBound))
                {
                    var lTile = Main.tile[x, upperBound];
                    lTile.LiquidAmount = 0;
                    WorldGen.PlaceTile(x, upperBound, TileID.Stone, true);
                }
                upperBound--;
            }
            while (WorldGen.InWorld(x, lowerBound + 1) && Main.tile[x, lowerBound + 1].LiquidAmount > 0)
            {
                if (!BoilerSystem.TileInBoilerRange(x, lowerBound))
                {
                    var lTile = Main.tile[x, lowerBound];
                    lTile.LiquidAmount = 0;
                    WorldGen.PlaceTile(x, lowerBound, TileID.Stone, true);
                }
                lowerBound++;
            }
            for (int j = upperBound; j <= lowerBound; j++)
            {
                var i = x;
                while (WorldGen.InWorld(i - 1, j) && Main.tile[i - 1, j].LiquidAmount > 0)
                {
                    if (!BoilerSystem.TileInBoilerRange(i, j))
                    {
                        var lTile = Main.tile[i, j];
                        WorldGen.EmptyLiquid(i, j);
                        WorldGen.PlaceTile(i, j, TileID.Stone, true);
                    }
                    i--;
                }
                if (!BoilerSystem.TileInBoilerRange(i, j))
                {
                    WorldGen.PlaceTile(i, j, TileID.Stone, true);
                }
                if (WorldGen.InWorld(i - 1, j) && !BoilerSystem.TileInBoilerRange(i - 1, j))
                {
                    var lTile = Main.tile[i - 1, j];
                    WorldGen.EmptyLiquid(i, j);
                    lTile.Slope = SlopeType.Solid;
                    lTile.IsHalfBlock = false;
                }
                i = x;
                while (WorldGen.InWorld(i + 1, j) && Main.tile[i + 1, j].LiquidAmount > 0)
                {
                    if (!BoilerSystem.TileInBoilerRange(i, j))
                    {
                        var lTile = Main.tile[i, j];
                        WorldGen.EmptyLiquid(i, j);
                        WorldGen.PlaceTile(i, j, TileID.Stone, true);
                    }
                    i++;
                }
                if (WorldGen.InWorld(i + 1, j) && !BoilerSystem.TileInBoilerRange(i + 1, j))
                {
                    var lTile = Main.tile[i + 1, j];
                    WorldGen.EmptyLiquid(i, j);
                    lTile.Slope = SlopeType.Solid;
                    lTile.IsHalfBlock = false;
                }
                if (!BoilerSystem.TileInBoilerRange(i, j))
                {
                    WorldGen.PlaceTile(i, j, TileID.Stone, true);
                }
            }
            WorldGen.EmptyLiquid(x, y);
            WorldGen.PlaceTile(x, y, TileID.Stone, true);
            SoundEngine.PlaySound(SoundID.LiquidsWaterLava, new Vector2(x, y).ToWorldCoordinates());
        }
    }

    public static bool TryUnfreezeTile(int i, int j)
    {
        if (!WorldGen.InWorld(i, j))
        {
            return false;
        }
        Tile tile = Main.tile[i, j];
        if (FreezableWalls.ContainsValue(tile.WallType))
        {
            foreach (int key in FreezableWalls.Keys)
            {
                if (FreezableWalls[key] == tile.WallType)
                {
                    tile.WallType = (ushort)key;
                }
            }
        }
        if (!tile.HasTile)
        {
            return false;
        }
        if (tile.TileType == TileID.BreakableIce)
        {
            tile.HasTile = false;
            WorldGen.PlaceLiquid(i, j, (byte)LiquidID.Water, byte.MaxValue);
            WorldGen.SquareTileFrame(i, j);
        }
        if (!FreezableTiles.ContainsValue(tile.TileType))
        {
            return false;
        }
        int desiredType = tile.TileType;
        foreach (int key in FreezableTiles.Keys)
        {
            if (FreezableTiles[key] == tile.TileType)
            {
                desiredType = key;
                break;
            }
        }
        tile.TileType = (ushort)desiredType;
        if (tile.TileType == TileID.Dirt && WorldGen.TileIsExposedToAir(i, j) && j < Main.worldSurface && (tile.WallType == 0 || WallID.Sets.AllowsPlantsToGrow[tile.WallType]))
        {
            tile.TileType = TileID.Grass;
        }
        WorldGen.SquareTileFrame(i, j);
        return true;
    }
}

public class RandomUpdateFreeze : GlobalTile
{
    public override void RandomUpdate(int i, int j, int type)
    {
        if (j > TileFreezing.bandY)
        {
            return;
        }
        TileFreezing.AttemptTileFreeze(i, j, true);
    }
}

public class RandomUpdateWallFreeze : GlobalWall
{
    public override void RandomUpdate(int i, int j, int type)
    {
        if (j > TileFreezing.bandY)
        {
            return;
        }
        TileFreezing.AttemptWallFreeze(i, j);
    }
}
