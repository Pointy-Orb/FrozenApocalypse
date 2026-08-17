using Terraria;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria.ModLoader;
using Terraria.ID;

namespace FrozenApocalypse.Content.Tiles;

public abstract class FrozenTile : ModTile
{
    public abstract int UnfrozenCounterpart { get; }
    public abstract Color MapColor { get; }

    public virtual int[] Fallbacks => empty;
    private int[] empty = new int[0];

    public virtual bool Ice => true;

    public virtual bool Hot => false;

    public new virtual int MinPick => 0;

    public sealed override void SetStaticDefaults()
    {
        TileFreezing.FreezableTiles.Add(UnfrozenCounterpart, Type);
        Main.tileSolid[Type] = Main.tileSolid[UnfrozenCounterpart];
        Main.tileMergeDirt[Type] = Main.tileMergeDirt[UnfrozenCounterpart];
        bool[] tileMerge = Main.tileMerge[UnfrozenCounterpart];
        for (int i = 0; i < tileMerge.Length; i++)
        {
            if (TileFreezing.FreezableTiles.ContainsKey(i))
            {
                tileMerge[TileFreezing.FreezableTiles[i]] = true;
            }
            if (tileMerge[i])
            {
                Main.tileMerge[i][Type] = true;
            }
        }
        Main.tileMerge[Type] = tileMerge;
        Main.tileBlockLight[Type] = Main.tileBlockLight[UnfrozenCounterpart];
        Main.tileBlendAll[Type] = Main.tileBlendAll[UnfrozenCounterpart];

        TileID.Sets.IceSkateSlippery[Type] = Ice;
        TileID.Sets.Ices[Type] = Ice;
        TileID.Sets.IcesSlush[Type] = Ice;
        TileID.Sets.Snow[Type] = !Ice;
        TileID.Sets.IcesSnow[Type] = true;

        if (TileID.Sets.Corrupt[UnfrozenCounterpart])
        {
            TileID.Sets.AddCorruptionTile(Type);
        }
        if (TileID.Sets.Crimson[UnfrozenCounterpart])
        {
            TileID.Sets.AddCrimsonTile(Type);
            TileID.Sets.Crimson[Type] = true;
        }
        if (TileID.Sets.Hallow[UnfrozenCounterpart])
        {
            TileID.Sets.Hallow[Type] = true;
            TileID.Sets.HallowBiome[Type] = TileID.Sets.HallowBiome[UnfrozenCounterpart];
            TileID.Sets.HallowBiomeSight[Type] = true;
            TileID.Sets.CanGrowCrystalShards[Type] = true;
        }
        if (TileID.Sets.JungleBiome[UnfrozenCounterpart] > 0)
        {
            FrozenApocalypseIDs.TileSets.FrozenJungleTiles.Add(Type);
        }
        if (TileID.Sets.isDesertBiomeSand[UnfrozenCounterpart])
        {
            FrozenApocalypseIDs.TileSets.FrozenDesertTiles.Add(Type);
        }

        TileLoader.RegisterConversionFallback(Type, UnfrozenCounterpart);

        VanillaFallbackOnModDeletion = (ushort)UnfrozenCounterpart;

        AddMapEntry(MapColor);

        base.MinPick = MinPick;
        HitSound = SoundID.Item50;
        DustType = DustID.Ice;
        PostSetStaticDefaults();
    }

    public sealed override void PostSetupTileMerge()
    {
        PostSetupContent();
        for (int i = 0; i < Fallbacks.Length; i++)
        {
            if (TileFreezing.FreezableTiles.ContainsKey(Fallbacks[i]))
            {
                continue;
            }
            TileFreezing.FreezableTiles.Add(Fallbacks[i], Type);
        }
    }

    public virtual void PostSetupContent()
    {

    }

    public virtual void PostSetStaticDefaults() { }

    public override void OnTileConverted(int i, int j, int fromType, int toType, int conversionType)
    {
        if (fromType != Type)
        {
            return;
        }
        TileFreezing.AttemptTileFreeze(i, j, true, false);
    }
}

public abstract class FrozenTileItem : ModItem
{
    public abstract int Tile { get; }

    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(Tile);
    }

    public override void AddRecipes()
    {
        var frozenTile = ModContent.GetModTile(Tile) as FrozenTile;
        if (frozenTile == null)
        {
            return;
        }
        Recipe.Create(frozenTile.UnfrozenCounterpart)
            .AddIngredient(this)
            .AddTile(TileID.Furnaces)
            .Register();
        CreateRecipe()
            .AddIngredient(frozenTile.UnfrozenCounterpart)
            .AddTile(TileID.IceMachine)
            .Register();
    }
}
