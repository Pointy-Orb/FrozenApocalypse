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

    public virtual bool Ice => true;

    public virtual bool Hot => false;

    public new virtual int MinPick => 0;

    public virtual int? CorruptionTile => null;
    public virtual int? CrimsonTile => null;
    public virtual int? HallowTile => null;
    public virtual int? PurityTile => null;
    public virtual bool powderImmune => false;

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
        TileID.Sets.Conversion.Snow[Type] = !Ice;
        TileID.Sets.IcesSnow[Type] = true;

        //temporary
        TileID.Sets.SnowBiome[Type] = 1;

        if (TileID.Sets.Corrupt[UnfrozenCounterpart])
        {
            TileID.Sets.AddCorruptionTile(Type);
        }
        if (TileID.Sets.Crimson[UnfrozenCounterpart])
        {
            TileID.Sets.AddCrimsonTile(Type);
        }
        if (TileID.Sets.Hallow[Type])
        {
            TileID.Sets.Hallow[Type] = true;
            TileID.Sets.HallowBiome[Type] = 1;
            TileID.Sets.HallowBiomeSight[Type] = true;
            TileID.Sets.CanGrowCrystalShards[Type] = true;
        }

        VanillaFallbackOnModDeletion = (ushort)UnfrozenCounterpart;

        AddMapEntry(MapColor);

        base.MinPick = MinPick;
        HitSound = SoundID.Item50;
        DustType = DustID.Ice;
        PostSetStaticDefaults();
    }

    public virtual void PostSetStaticDefaults() { }

    public override void Convert(int i, int j, int conversionType)
    {
        Tile tile = Main.tile[i, j];
        if (conversionType == BiomeConversionID.Corruption && CorruptionTile != null)
        {
            tile.TileType = (ushort)CorruptionTile;
        }
        if (conversionType == BiomeConversionID.Crimson && CrimsonTile != null)
        {
            tile.TileType = (ushort)CrimsonTile;
        }
        if (conversionType == BiomeConversionID.Hallow && HallowTile != null)
        {
            tile.TileType = (ushort)HallowTile;
        }
        if ((conversionType == BiomeConversionID.Purity || (!powderImmune && conversionType == BiomeConversionID.PurificationPowder)) && PurityTile != null)
        {
            tile.TileType = (ushort)PurityTile;
        }
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
