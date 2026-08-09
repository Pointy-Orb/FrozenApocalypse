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

public class AutoloadFrostTile : ModTile
{
    public readonly int UnfrozenCounterpart;

    public override string Name => $"Frosted{TileID.Search.GetName(UnfrozenCounterpart)}";

    public override string Texture => $"Terraria/Images/Tiles_{UnfrozenCounterpart}";

    public AutoloadFrostTile(int originalType)
    {
        UnfrozenCounterpart = originalType;
    }

    public override void SetStaticDefaults()
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
        Main.tileBlockLight[Type] = Main.tileBlockLight[UnfrozenCounterpart];
        Main.tileBlendAll[Type] = Main.tileBlendAll[UnfrozenCounterpart];
        TileID.Sets.IceSkateSlippery[Type] = true;
        TileID.Sets.Ices[Type] = true;

        if (TileID.Sets.JungleBiome[UnfrozenCounterpart] > 0)
        {
            FrozenApocalypseIDs.TileSets.FrozenJungleTiles.Add(Type);
        }
        if (TileID.Sets.isDesertBiomeSand[UnfrozenCounterpart])
        {
            FrozenApocalypseIDs.TileSets.FrozenDesertTiles.Add(Type);
        }

        VanillaFallbackOnModDeletion = (ushort)UnfrozenCounterpart;

        Color[] ColorLookup = (Color[])typeof(MapHelper).GetField("colorLookup", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        Color frostedColor = ColorLookup[MapHelper.TileToLookup(UnfrozenCounterpart, 0)];
        frostedColor.R = (byte)Math.Min((float)frostedColor.R + Color.Aqua.R * 0.2f, (float)byte.MaxValue);
        frostedColor.G = (byte)Math.Min((float)frostedColor.G + Color.Aqua.G * 0.2f, (float)byte.MaxValue);
        frostedColor.B = (byte)Math.Min((float)frostedColor.B + Color.Aqua.B * 0.2f, (float)byte.MaxValue);
        frostedColor.R = (byte)Math.Min((float)frostedColor.R + Color.Blue.R * 0.2f, (float)byte.MaxValue);
        frostedColor.G = (byte)Math.Min((float)frostedColor.G + Color.Blue.G * 0.2f, (float)byte.MaxValue);
        frostedColor.B = (byte)Math.Min((float)frostedColor.B + Color.Blue.B * 0.2f, (float)byte.MaxValue);
        AddMapEntry(frostedColor);
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);
        return true;
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
        Tile tile = Main.tile[i, j];

        Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);


        spriteBatch.Draw(
            TextureAssets.Tile[Type].Value,
            new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
            new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16),
            Lighting.GetColor(i, j).MultiplyRGB(Color.Aqua) * 0.7f, 0f, default, 1f, SpriteEffects.None, 0f);
        spriteBatch.Draw(
            TextureAssets.Tile[Type].Value,
            new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
            new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16),
            Lighting.GetColor(i, j).MultiplyRGB(Color.Blue) * 0.7f, 0f, default, 1f, SpriteEffects.None, 0f);
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);
    }
}

public class AutoloadFrostTileItem : ModItem
{
    public readonly int UnfrozenCounterpart;
    public readonly int UnfrozenCounterpartTile;

    protected override bool CloneNewInstances => true;

    public override string Name => $"Frosted{ItemID.Search.GetName(UnfrozenCounterpart)}";

    private int nameIndex;
    public override LocalizedText DisplayName => Language.GetText($"Mods.FrozenApocalypse.Items.AutoloadFrostTileItem.DisplayName{nameIndex}").WithFormatArgs(Language.GetText($"ItemName.{ItemID.Search.GetName(UnfrozenCounterpart)}"));

    public override LocalizedText Tooltip => Language.GetOrRegister("Mods.FrozenApocalypse.Items.AutoloadFrostTileItem.Tooltip");

    public override string Texture => $"Terraria/Images/Item_{UnfrozenCounterpart}";

    public AutoloadFrostTileItem(int originalItem, int nameIndex = 0)
    {
        this.nameIndex = nameIndex;
        UnfrozenCounterpart = originalItem;
        Item reference = new Item(UnfrozenCounterpart);
        if (reference.createTile > -1)
        {
            UnfrozenCounterpartTile = reference.createTile;
        }
    }

    public override void SetStaticDefaults()
    {
        Item reference = new Item(UnfrozenCounterpart);
        Item.ResearchUnlockCount = reference.ResearchUnlockCount;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(TileFreezing.FreezableTiles[UnfrozenCounterpartTile]);
        Item reference = new Item(UnfrozenCounterpart);
        Item.rare = reference.rare;
        Item.value = reference.value;
    }

    public override void AddRecipes()
    {
        Recipe.Create(UnfrozenCounterpart)
            .AddIngredient(Type)
            .AddTile(TileID.Furnaces)
            .Register();
    }

    public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Item == Main.mouseItem ? Main.SamplerStateForCursor : default, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
        spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, drawColor.MultiplyRGB(Color.Aqua) * 0.7f, 0, origin, scale, SpriteEffects.None, 0f);
        spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, drawColor.MultiplyRGB(Color.Blue) * 0.7f, 0, origin, scale, SpriteEffects.None, 0f);
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
    }

    public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
    {
        Main.GetItemDrawFrame(Item.type, out var itemTexture, out var frame);
        Vector2 origin = frame.Size() / 2f;
        Vector2 position = Item.Bottom - Main.screenPosition - new Vector2(0, origin.Y);
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, lightColor.MultiplyRGB(Color.Aqua) * 0.7f, 0, origin, scale, SpriteEffects.None, 0f);
        spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, lightColor.MultiplyRGB(Color.Blue) * 0.7f, 0, origin, scale, SpriteEffects.None, 0f);
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
    }
}

public class AutoloadTileLoader : ILoadable
{
    private int[] vanillaSands = new int[]
    {
        TileID.Sand,
        TileID.Ebonsand,
        TileID.Crimsand,
        TileID.Pearlsand
    };

    public void Load(Mod mod)
    {
        mod.AddContent(new AutoloadFrostTile(TileID.Hive));
        mod.AddContent(new AutoloadFrostTile(TileID.LivingWood));
        mod.AddContent(new AutoloadFrostTile(TileID.LivingMahogany));

        mod.AddContent(new AutoloadFrostTile(TileID.ClayBlock));
        mod.AddContent(new AutoloadFrostTileItem(ItemID.ClayBlock));
        mod.AddContent(new AutoloadFrostTile(TileID.HoneyBlock));
        mod.AddContent(new AutoloadFrostTileItem(ItemID.HoneyBlock));
        for (int i = 0; i < TileLoader.TileCount; i++)
        {
            if (i < TileID.Sets.Conversion.Sand.Length && TileID.Sets.Conversion.Sand[i] && !vanillaSands.Contains(i))
            {
                mod.AddContent(new AutoloadFrostTile(i));
                AddTileForItem(i, mod, 1);
            }
            if (i < TileID.Sets.Conversion.HardenedSand.Length && TileID.Sets.Conversion.HardenedSand[i])
            {
                mod.AddContent(new AutoloadFrostTile(i));
                AddTileForItem(i, mod);
            }
            if (i < TileID.Sets.Conversion.Sandstone.Length && TileID.Sets.Conversion.Sandstone[i])
            {
                mod.AddContent(new AutoloadFrostTile(i));
                AddTileForItem(i, mod);
            }
            if (i < TileID.Sets.Conversion.MushroomGrass.Length && TileID.Sets.Conversion.MushroomGrass[i])
            {
                mod.AddContent(new AutoloadFrostTile(i));
            }
        }
    }

    private void AddTileForItem(int tileType, Mod mod, int nameIndex = 0)
    {
        Item reference = new();
        for (int i = 0; i < ItemLoader.ItemCount; i++)
        {
            reference.SetDefaults(i);
            if (reference.createTile == tileType)
            {
                mod.AddContent(new AutoloadFrostTileItem(i, nameIndex));
                break;
            }
        }
    }

    public void Unload()
    {

    }
}
