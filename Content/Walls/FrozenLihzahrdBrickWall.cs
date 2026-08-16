using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace FrozenApocalypse.Content.Walls;

public class FrozenLihzahrdBrickWall : ModWall
{
    public override void SetStaticDefaults()
    {
        TileFreezing.FreezableWalls.Add(WallID.LihzahrdBrick, Type);
        TileFreezing.FreezableWalls.Add(WallID.LihzahrdBrickUnsafe, Type);

        Main.wallBlend[Type] = WallID.LihzahrdBrickUnsafe;

        DustType = DustID.Ice;
        AddMapEntry(new Color(2, 30, 44));
    }
}
