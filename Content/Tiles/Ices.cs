using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.ID;
using System.Collections.Generic;

namespace FrozenApocalypse.Content.Tiles;

public class SandIce : FrozenTile
{
    public override int UnfrozenCounterpart => TileID.Sand;
    public override Color MapColor => new Color(196, 191, 171);
    public override bool Hot => true;
    public override int? CorruptionTile => ModContent.TileType<EbonsandIce>();
    public override int? CrimsonTile => ModContent.TileType<CrimsandIce>();
    public override int? HallowTile => ModContent.TileType<PearlsandIce>();
}
public class SandIceItem : FrozenTileItem { public override int Tile => ModContent.TileType<SandIce>(); }

public class EbonsandIce : FrozenTile
{
    public override int UnfrozenCounterpart => TileID.Ebonsand;
    public override Color MapColor => new Color(155, 148, 164);
    public override bool Hot => true;
    public override int? PurityTile => ModContent.TileType<SandIce>();
    public override int? CrimsonTile => ModContent.TileType<CrimsandIce>();
    public override int? HallowTile => ModContent.TileType<PearlsandIce>();
}
public class EbonsandIceItem : FrozenTileItem { public override int Tile => ModContent.TileType<EbonsandIce>(); }

public class PearlsandIce : FrozenTile
{
    public override int UnfrozenCounterpart => TileID.Pearlsand;
    public override Color MapColor => new Color(250, 238, 241);
    public override bool Hot => true;
    public override int? CorruptionTile => ModContent.TileType<EbonsandIce>();
    public override int? CrimsonTile => ModContent.TileType<CrimsandIce>();
    public override int? PurityTile => ModContent.TileType<SandIce>();
    public override bool powderImmune => true;
}
public class PearlsandIceItem : FrozenTileItem { public override int Tile => ModContent.TileType<PearlsandIce>(); }

public class CrimsandIce : FrozenTile
{
    public override int UnfrozenCounterpart => TileID.Crimsand;
    public override Color MapColor => new Color(167, 165, 161);
    public override bool Hot => true;
    public override int? CorruptionTile => ModContent.TileType<EbonsandIce>();
    public override int? PurityTile => ModContent.TileType<SandIce>();
    public override int? HallowTile => ModContent.TileType<PearlsandIce>();
}
public class CrimsandIceItem : FrozenTileItem { public override int Tile => ModContent.TileType<CrimsandIce>(); }


public class Permafrost : FrozenTile
{
    public override int UnfrozenCounterpart => TileID.Mud;
    public override Color MapColor => new Color(103, 156, 161);

    public override void PostSetStaticDefaults()
    {
        FrozenApocalypseIDs.TileSets.FrozenJungleTiles.Add(Type);
    }
}
public class PermafrostItem : FrozenTileItem { public override int Tile => ModContent.TileType<Permafrost>(); }

public class FrozenAsh : FrozenTile { public override int UnfrozenCounterpart => TileID.Ash; public override Color MapColor => new Color(105, 119, 122); }
public class FrozenAshItem : FrozenTileItem { public override int Tile => ModContent.TileType<FrozenAsh>(); }


public class Ebonfrost : FrozenTile
{
    public override int UnfrozenCounterpart => TileID.Ebonstone;
    public override Color MapColor => new Color(101, 115, 129);
    public override int? PurityTile => ModContent.TileType<EvilIce>();
    public override int? CrimsonTile => ModContent.TileType<Crimfrost>();
    public override int? HallowTile => ModContent.TileType<Pearlfrost>();
}
public class EbonfrostItem : FrozenTileItem { public override int Tile => ModContent.TileType<Ebonfrost>(); }

public class Pearlfrost : FrozenTile
{
    public override int UnfrozenCounterpart => TileID.Pearlstone;
    public override Color MapColor => new Color(211, 205, 209);
    public override int? CorruptionTile => ModContent.TileType<Ebonfrost>();
    public override int? CrimsonTile => ModContent.TileType<Crimfrost>();
    public override int? PurityTile => ModContent.TileType<EvilIce>();
    public override bool powderImmune => true;
}
public class PearlfrostItem : FrozenTileItem { public override int Tile => ModContent.TileType<Pearlfrost>(); }

public class Crimfrost : FrozenTile
{
    public override int UnfrozenCounterpart => TileID.Crimstone;
    public override Color MapColor => new Color(207, 117, 131);
    public override int? CorruptionTile => ModContent.TileType<Ebonfrost>();
    public override int? PurityTile => ModContent.TileType<EvilIce>();
    public override int? HallowTile => ModContent.TileType<Pearlfrost>();
}
public class CrimfrostItem : FrozenTileItem { public override int Tile => ModContent.TileType<Crimfrost>(); }

public class Peat : FrozenTile
{
    public override Color MapColor => new Color(72, 80, 60);

    public override int UnfrozenCounterpart => TileID.JungleGrass;

    public override int[] Fallbacks => jungleGrasses.ToArray();
    private List<int> jungleGrasses = new();

    public override bool Ice => false;

    public override IEnumerable<Item> GetItemDrops(int i, int j)
    {
        var peatItem = new Item(ModContent.ItemType<Items.Peat>(), Main.rand.Next(1, 3));
        yield return peatItem;
    }

    public override void PostSetupContent()
    {
        for (int i = 0; i < TileID.Sets.Conversion.JungleGrass.Length; i++)
        {
            if (!TileID.Sets.Conversion.JungleGrass[i])
            {
                continue;
            }
            jungleGrasses.Add(i);
        }
    }
}

public class FrozenLihzahrdBrick : FrozenTile
{
    public override int UnfrozenCounterpart => TileID.LihzahrdBrick;

    public override int MinPick => 200;

    public override Color MapColor => new Color(54, 178, 207);

    public override void PostSetStaticDefaults()
    {
        Main.tileMergeDirt[Type] = false;
    }
}
public class FrozenLihzahrdBrickItem : FrozenTileItem { public override int Tile => ModContent.TileType<FrozenLihzahrdBrick>(); }

public class EvilSnow : FrozenTile
{
    public override string Texture => "Terraria/Images/Tiles_147";

    public override int UnfrozenCounterpart => TileID.Dirt;

    public override Color MapColor => new Color(211, 236, 241);

    public override bool Ice => false;

    public override void PostSetStaticDefaults()
    {
        Main.tileMerge[Type][TileID.SnowBlock] = true;
        Main.tileMerge[TileID.SnowBlock][Type] = true;
        Main.tileMergeDirt[Type] = true;
        VanillaFallbackOnModDeletion = TileID.SnowBlock;
    }

    public override IEnumerable<Item> GetItemDrops(int i, int j)
    {
        yield return new Item(ItemID.SnowBlock);
    }
}

public class EvilIce : FrozenTile
{
    public override string Texture => "Terraria/Images/Tiles_161";

    public override int UnfrozenCounterpart => TileID.Stone;

    public override Color MapColor => new Color(144, 195, 232);

    public override void PostSetStaticDefaults()
    {
        Main.tileMerge[Type][TileID.IceBlock] = true;
        Main.tileMerge[TileID.IceBlock][Type] = true;
        Main.tileMerge[TileID.Stone][Type] = true;
        VanillaFallbackOnModDeletion = TileID.IceBlock;
    }

    public override IEnumerable<Item> GetItemDrops(int i, int j)
    {
        yield return new Item(ItemID.IceBlock);
    }

    public override int? CorruptionTile => ModContent.TileType<Ebonfrost>();
    public override int? CrimsonTile => ModContent.TileType<Crimfrost>();
    public override int? HallowTile => ModContent.TileType<Pearlfrost>();
}
