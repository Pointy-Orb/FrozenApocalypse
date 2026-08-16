using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.ID;

namespace FrozenApocalypse.Content.Tiles;

public class Coal : ModTile
{
    public override void SetStaticDefaults()
    {
        TileID.Sets.Ore[Type] = true;
        TileID.Sets.FriendlyFairyCanLureTo[Type] = true;
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileSpelunker[Type] = true;
        Main.tileOreFinderPriority[Type] = 281;

        LocalizedText neem = CreateMapEntryName();
        AddMapEntry(new Color(36, 36, 36), neem);

        DustType = DustID.Ash;
        HitSound = SoundID.Tink;

        VanillaFallbackOnModDeletion = TileID.Sandstone;
    }
}
