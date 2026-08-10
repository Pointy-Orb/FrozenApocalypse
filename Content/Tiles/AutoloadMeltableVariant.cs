using Terraria;
using System.Linq;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System.Reflection;
using System;

namespace FrozenApocalypse.Content.Tiles;

public class AutoloadMeltableVariant : ModTile
{
    public readonly int UnfrozenCounterpart;
    public readonly int UnmeltableVariant;

    public override string Name => $"Meltable{TileID.Search.GetName(UnmeltableVariant)}";

    public override string Texture => $"Terraria/Images/Tiles_{UnmeltableVariant}";

    public AutoloadMeltableVariant(int unmeltableVariant, int unfrozenCounterpart)
    {
        UnfrozenCounterpart = unfrozenCounterpart;
        UnmeltableVariant = unmeltableVariant;
    }

    public override void SetStaticDefaults()
    {
        Item reference = new();
        for (int i = 0; i < ItemLoader.ItemCount; i++)
        {
            reference.SetDefaults(i);
            if (reference.createTile == UnmeltableVariant)
            {
                RegisterItemDrop(i);
                break;
            }
        }
        TileFreezing.FreezableTiles.Add(UnfrozenCounterpart, Type);
        Main.tileSolid[Type] = Main.tileSolid[UnmeltableVariant];
        Main.tileMergeDirt[Type] = Main.tileMergeDirt[UnmeltableVariant];
        bool[] tileMerge = Main.tileMerge[UnmeltableVariant];
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
        Main.tileBlockLight[Type] = Main.tileBlockLight[UnmeltableVariant];
        Main.tileBlendAll[Type] = Main.tileBlendAll[UnmeltableVariant];
        TileID.Sets.IceSkateSlippery[Type] = TileID.Sets.IceSkateSlippery[UnmeltableVariant];
        TileID.Sets.Ices[Type] = TileID.Sets.Ices[UnmeltableVariant];

        if (TileID.Sets.JungleBiome[UnmeltableVariant] > 0)
        {
            FrozenApocalypseIDs.TileSets.FrozenJungleTiles.Add(Type);
        }
        if (TileID.Sets.isDesertBiomeSand[UnmeltableVariant])
        {
            FrozenApocalypseIDs.TileSets.FrozenDesertTiles.Add(Type);
        }

        VanillaFallbackOnModDeletion = (ushort)UnfrozenCounterpart;

        Color[] ColorLookup = (Color[])typeof(MapHelper).GetField("colorLookup", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        Color frostedColor = ColorLookup[MapHelper.TileToLookup(UnmeltableVariant, 0)];
        AddMapEntry(frostedColor);
    }
}

public class AutoloadMeltableLoader : ILoadable
{
    public void Load(Mod mod)
    {
        mod.AddContent(new AutoloadMeltableVariant(TileID.Slush, TileID.Silt));
    }

    public void Unload()
    {
    }
}
