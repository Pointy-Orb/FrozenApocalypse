using Terraria;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Enums;

namespace FrozenApocalypse.Content.Tiles;

public class IcyPalmTree : ModPalmTree
{
    private static Asset<Texture2D> texture;
    private static Asset<Texture2D> topsTexture;

    public override TreeTypes CountsAsTreeType => TreeTypes.Snow;

    public override int DropWood()
    {
        return ItemID.BorealWood;
    }

    public override void SetStaticDefaults()
    {
        GrowsOnTileId = [ModContent.TileType<SandIce>(), ModContent.TileType<EbonsandIce>(), ModContent.TileType<CrimsandIce>(), ModContent.TileType<PearlsandIce>()];
        texture = ModContent.Request<Texture2D>("FrozenApocalypse/Content/Tiles/Plants/IcyPalmTree");
        topsTexture = ModContent.Request<Texture2D>("FrozenApocalypse/Content/Tiles/Plants/IcyPalmTree_Tops");
    }

    public override TreePaintingSettings TreeShaderSettings => new TreePaintingSettings
    {
        UseSpecialGroups = true,
        SpecialGroupMinimalHueValue = 11f / 72f,
        SpecialGroupMaximumHueValue = 0.25f,
        SpecialGroupMinimumSaturationValue = 0.88f,
        SpecialGroupMaximumSaturationValue = 1f
    };

    public override Asset<Texture2D> GetTexture() => texture;
    public override Asset<Texture2D> GetTopTextures() => topsTexture;
    public override Asset<Texture2D> GetOasisTopTextures() => GetTopTextures();
}
