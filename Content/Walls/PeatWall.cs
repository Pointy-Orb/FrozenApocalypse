using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace FrozenApocalypse.Content.Walls;

public class PeatWall : ModWall
{
    public override void SetStaticDefaults()
    {
        Main.wallBlend[Type] = WallID.JungleUnsafe;

        DustType = DustID.Ice;
        AddMapEntry(new Color(62, 70, 50));
    }
}
