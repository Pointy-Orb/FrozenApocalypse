using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.ID;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FrozenApocalypse.Content.Tiles;

public class FrozenRegularTree : ModTree
{
    private Asset<Texture2D> texture;
    private Asset<Texture2D> branchesTexture;
    private Asset<Texture2D> topsTexture;

    public override Asset<Texture2D> GetTexture() => texture;
    public override Asset<Texture2D> GetBranchTextures() => branchesTexture;
    public override Asset<Texture2D> GetTopTextures() => topsTexture;

    public override int DropWood() => ItemID.Wood;

    public override TreePaintingSettings TreeShaderSettings => new TreePaintingSettings
    {
        UseSpecialGroups = true,
        SpecialGroupMinimalHueValue = 11f / 72f,
        SpecialGroupMaximumHueValue = 0.25f,
        SpecialGroupMinimumSaturationValue = 0.88f,
        SpecialGroupMaximumSaturationValue = 1f
    };

    public override void SetStaticDefaults()
    {
        GrowsOnTileId = [ModContent.TileType<EvilSnow>(), ModContent.TileType<Peat>()];
        texture = ModContent.Request<Texture2D>("FrozenApocalypse/Content/Tiles/Plants/FrozenRegularTree");
        branchesTexture = ModContent.Request<Texture2D>("FrozenApocalypse/Content/Tiles/Plants/FrozenRegularTree_Branches");
        topsTexture = ModContent.Request<Texture2D>("FrozenApocalypse/Content/Tiles/Plants/FrozenRegularTree_Tops");
    }

    public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
    {
    }

    public override int CreateDust()
    {
        return DustID.Snow;
    }
}
