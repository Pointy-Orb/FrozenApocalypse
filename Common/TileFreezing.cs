using Terraria;
using FrozenApocalypse.Content.TileEntities;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using FrozenApocalypse.Content.Tiles;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace FrozenApocalypse;

public class TileFreezing : ModSystem
{
    public static int BandHeight => Main.maxTilesY / 40;

    public static int UpperBand => bandY - BandHeight;

    public static int bandY = BandHeight;

    public static int ConvertsPerUpdate => Main.maxTilesY / 600;

    public static Dictionary<int, int> FreezableTiles = new()
    {
        { TileID.Silt, TileID.Slush }
    };

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
        }
        for (int i = 0; i < ConvertsPerUpdate; i++)
        {
            int x = Main.rand.Next(0, Main.maxTilesX);
            int y = Main.rand.Next(UpperBand, bandY);
            AttemptTileFreeze(x, y, false);
            var pos = Main.LocalPlayer.Center.ToTileCoordinates();
        }
        wasDay = Main.dayTime;
    }

    public override void PostUpdateWorld()
    {
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
        for (int i = x - 1; i <= x + 1; i++)
        {
            bool froze = false;
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (!WorldGen.InWorld(i, j))
                {
                    continue;
                }
                if (Main.tile[i, j].LiquidAmount > 0)
                {
                    TryFreezeLiquid(i, j);
                    froze = true;
                    break;
                }
            }
            if (froze)
            {
                break;
            }
        }
        if (!tile.HasTile)
        {
            return;
        }
        if (!FreezableTiles.ContainsKey(tile.TileType))
        {
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
        tile.TileType = (ushort)FreezableTiles[tile.TileType];
        WorldGen.Reframe(x, y);
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

    private static void TryFreezeLiquid(int x, int y)
    {
        Tile tile = Main.tile[x, y];
        if (tile.LiquidAmount <= 0)
        {
            return;
        }
        if (tile.LiquidType == LiquidID.Water)
        {
            int j = y;
            while (WorldGen.InWorld(x, j - 1) && Main.tile[x, j - 1].LiquidAmount > 0)
            {
                j--;
            }
            if (Main.tile[x, j - 1].TileType == TileID.BreakableIce)
            {
                return;
            }
            Main.tile[x, j].LiquidAmount = 0;
            WorldGen.PlaceTile(x, j, TileID.BreakableIce, true);
        }
        if (tile.LiquidType == LiquidID.Lava)
        {
            int lowerBound = y;
            int upperBound = y;
            while (WorldGen.InWorld(x, upperBound - 1) && Main.tile[x, upperBound - 1].LiquidAmount > 0)
            {
                var lTile = Main.tile[x, upperBound];
                lTile.LiquidAmount = 0;
                WorldGen.PlaceTile(x, upperBound, TileID.Stone, true);
                upperBound--;
            }
            while (WorldGen.InWorld(x, lowerBound + 1) && Main.tile[x, lowerBound + 1].LiquidAmount > 0)
            {
                var lTile = Main.tile[x, lowerBound];
                lTile.LiquidAmount = 0;
                WorldGen.PlaceTile(x, lowerBound, TileID.Stone, true);
                lowerBound++;
            }
            for (int j = upperBound; j <= lowerBound; j++)
            {
                var i = x;
                while (WorldGen.InWorld(i - 1, j) && Main.tile[i - 1, j].LiquidAmount > 0)
                {
                    var lTile = Main.tile[i, j];
                    lTile.LiquidAmount = 0;
                    WorldGen.PlaceTile(i, j, TileID.Stone, true);
                    i--;
                }
                WorldGen.PlaceTile(i, j, TileID.Stone, true);
                if (WorldGen.InWorld(i - 1, j))
                {
                    var lTile = Main.tile[i - 1, j];
                    lTile.LiquidAmount = 0;
                    lTile.Slope = SlopeType.Solid;
                    lTile.IsHalfBlock = false;
                }
                i = x;
                while (WorldGen.InWorld(i + 1, j) && Main.tile[i + 1, j].LiquidAmount > 0)
                {
                    var lTile = Main.tile[i, j];
                    lTile.LiquidAmount = 0;
                    WorldGen.PlaceTile(i, j, TileID.Stone, true);
                    i++;
                }
                if (WorldGen.InWorld(i + 1, j))
                {
                    var lTile = Main.tile[i + 1, j];
                    lTile.LiquidAmount = 0;
                    lTile.Slope = SlopeType.Solid;
                    lTile.IsHalfBlock = false;
                }
                WorldGen.PlaceTile(i, j, TileID.Stone, true);
            }
            tile.LiquidAmount = 0;
            WorldGen.PlaceTile(x, y, TileID.Stone, true);
            SoundEngine.PlaySound(SoundID.LiquidsWaterLava, new Vector2(x, y).ToWorldCoordinates());
        }
    }

    public static void TryUnfreezeTile(int i, int j)
    {
        if (!WorldGen.InWorld(i, j))
        {
            return;
        }
        Tile tile = Main.tile[i, j];
        if (tile.TileType == TileID.BreakableIce)
        {
            WorldGen.KillTile(i, j);
        }
        if (!FreezableTiles.ContainsValue(tile.TileType))
        {
            return;
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
        WorldGen.Reframe(i, j);
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
