using Terraria;
using Terraria.Localization;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria.Audio;
using FrozenApocalypse.Content.Items;
using Terraria.DataStructures;
using Terraria.ObjectData;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace FrozenApocalypse.Content.TileEntities;

public class Boiler : ModTile
{
    private static Asset<Texture2D> glowTexture;

    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLighted[Type] = true;
        TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
        TileID.Sets.PreventsTileHammeringIfOnTopOfIt[Type] = true;
        TileID.Sets.AvoidedByMeteorLanding[Type] = true;
        TileID.Sets.AvoidedByNPCs[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
        TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.newTile.HookPostPlaceMyPlayer = ModContent.GetInstance<BoilerEntity>().Generic_HookPostPlaceMyPlayer;
        TileObjectData.addTile(Type);

        AnimationFrameHeight = 54;
        glowTexture = ModContent.Request<Texture2D>("FrozenApocalypse/Content/TileEntities/Boiler_Glow");

        AddMapEntry(Color.DarkSlateGray, Language.GetText("Mods.FrozenApocalypse.Items.BoilerItem.DisplayName"));
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY)
    {
        if (TileEntity.TryGet<BoilerEntity>(i, j, out BoilerEntity entity))
        {
            BoilerSystem.boilers.Remove(entity);
        }
        ModContent.GetInstance<BoilerEntity>().Kill(i, j);
    }

    public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
    {
        var frame = Main.tileFrameCounter[Type] / 5;
        if (TileEntity.TryGet<BoilerEntity>(i, j, out BoilerEntity entity))
        {
            if (entity.timers.Count > 0)
            {
                frameYOffset = AnimationFrameHeight + frame * AnimationFrameHeight;
            }
            else
            {
                frameYOffset = 0;
            }
        }
    }

    public override void AnimateTile(ref int frame, ref int frameCounter)
    {
        frameCounter++;
        if (frameCounter >= 15)
        {
            frameCounter = 0;
        }
    }

    public override void MouseOver(int i, int j)
    {
        if (Fuels.fuels.ContainsKey(Main.LocalPlayer.HeldItem.type))
        {
            Main.LocalPlayer.cursorItemIconEnabled = true;
            Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<BoilerItem>();
        }
    }

    public override void PlaceInWorld(int i, int j, Item item)
    {
        if (TileEntity.TryGet<BoilerEntity>(i, j, out BoilerEntity entity))
        {
            BoilerSystem.boilers.Add(entity);
        }
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
        if (TileEntity.TryGet<BoilerEntity>(i, j, out BoilerEntity entity))
        {
            if (entity.timers.Count <= 0)
            {
                return;
            }
        }
        Tile tile = Main.tile[i, j];
        Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
        var frame = Main.tileFrameCounter[Type] / 5;
        int frameYOffset = frame * AnimationFrameHeight + AnimationFrameHeight;
        spriteBatch.Draw(
            glowTexture.Value,
            new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 2) + zero,
            new Rectangle(tile.TileFrameX, tile.TileFrameY + frameYOffset, 16, 16),
            Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        if (TileEntity.TryGet<BoilerEntity>(i, j, out BoilerEntity entity))
        {
            if (entity.timers.Count <= 0)
            {
                r = 0;
                g = 0;
                b = 0;
                return;
            }
        }
        r = 1f;
        g = 0.6f;
        b = 0f;
        var brightMult = Utils.Remap(entity.timers.Count, 0, 30, 0, 1);
        r *= brightMult;
        g *= brightMult;
    }
}

public class BoilerEntity : ModTileEntity
{
    public Stack<int> timers = new();

    public int FuelUntilRangeIncrease { get; private set; } = 0;
    public Point16 Center => new Point16(Position.X + 1, Position.Y + 1);
    public Vector2 WorldCenter => Center.ToWorldCoordinates();

    public override bool IsTileValidForEntity(int x, int y)
    {
        Tile tile = Main.tile[x, y];
        return tile.HasTile && tile.TileType == ModContent.TileType<Boiler>();
    }

    public override void Update()
    {
        if (timers.Count <= 0)
        {
            return;
        }
        var curTimer = timers.Pop();
        if (curTimer > 0)
        {
            curTimer--;
            timers.Push(curTimer);
        }
        else
        {
            FuelUntilRangeIncrease = GetNextFuelQuota();
            DrawRange();
        }
        int tries = 0;
        var Center = new Point16(Position.X + 1, Position.Y + 1);
        int unfreezeX = Main.rand.Next(Center.X - timers.Count, Center.X + timers.Count + 1);
        int unfreezeY = Main.rand.Next(Center.Y - timers.Count, Center.Y + timers.Count + 1);
        while (!TileInRange(unfreezeX, unfreezeY) && tries < 100)
        {
            unfreezeX = Main.rand.Next(Center.X - timers.Count, Center.X + timers.Count + 1);
            unfreezeY = Main.rand.Next(Center.Y - timers.Count, Center.Y + timers.Count + 1);
            tries++;
        }
        if (tries >= 100)
        {
            return;
        }
        TileFreezing.TryUnfreezeTile(unfreezeX, unfreezeY);
    }

    public bool TileInRange(int x, int y)
    {
        int distX = x - Center.X;
        int distY = y - Center.Y;
        return distX * distX + distY * distY <= timers.Count * timers.Count;
    }

    public bool EntityInRange(float x, float y)
    {
        float distX = x - WorldCenter.X;
        float distY = y - WorldCenter.Y;
        return distX * distX + distY * distY <= (timers.Count * 16) * (timers.Count * 16);
    }

    public bool DecrementFuelRequirement()
    {
        FuelUntilRangeIncrease--;
        var reachedZero = FuelUntilRangeIncrease <= 0;
        if (reachedZero)
        {
            FuelUntilRangeIncrease = GetNextFuelQuota();
            DrawRange();
        }
        return reachedZero;
    }

    private int GetNextFuelQuota()
    {
        return (int)MathF.Ceiling((timers.Count * timers.Count) / 100);
    }

    public override void SaveData(TagCompound tag)
    {
        tag["timers"] = timers.ToArray();
        if (FuelUntilRangeIncrease != GetNextFuelQuota())
        {
            tag["FuelUntilRangeIncrease"] = FuelUntilRangeIncrease;
        }
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.ContainsKey("timers"))
        {
            var timerArray = tag.GetIntArray("timers");
            for (int i = 0; i < timerArray.Length; i++)
            {
                timers.Push(timerArray[i]);
            }
        }
        if (tag.ContainsKey("FuelUntilRangeIncrease"))
        {
            FuelUntilRangeIncrease = tag.GetInt("FuelUntilRangeIncrease");
        }
        else
        {
            FuelUntilRangeIncrease = GetNextFuelQuota();
        }
        BoilerSystem.boilers.Add(this);
    }

    public void DrawRange()
    {
        int x = 0;
        int y = timers.Count * 16;
        int d = 3 - 2 * timers.Count * 16;
        while (x <= y)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 pos;
                switch (i)
                {
                    case 1:
                        pos = new Vector2(WorldCenter.X - x, WorldCenter.Y + y);
                        break;
                    case 2:
                        pos = new Vector2(WorldCenter.X + x, WorldCenter.Y - y);
                        break;
                    case 3:
                        pos = new Vector2(WorldCenter.X - x, WorldCenter.Y - y);
                        break;
                    case 4:
                        pos = new Vector2(WorldCenter.X + y, WorldCenter.Y + x);
                        break;
                    case 5:
                        pos = new Vector2(WorldCenter.X - y, WorldCenter.Y + x);
                        break;
                    case 6:
                        pos = new Vector2(WorldCenter.X + y, WorldCenter.Y - x);
                        break;
                    case 7:
                        pos = new Vector2(WorldCenter.X - y, WorldCenter.Y - x);
                        break;
                    case 0:
                    default:
                        pos = new Vector2(WorldCenter.X + x, WorldCenter.Y + y);
                        break;
                }
                var dust = Dust.NewDustPerfect(pos, DustID.Torch, Vector2.Zero);
                dust.noGravity = true;
            }
            if (d < 0)
                d += 4 * x + 6;
            else
            {
                d += 4 * (x - y) + 10;
                y--;
            }
            x++;
        }
    }
}

public class Fuels : GlobalItem
{
    public static readonly Dictionary<int, int> fuels = new();

    public override void SetStaticDefaults()
    {
        fuels.Add(ItemID.Torch, 1);
        if (RecipeGroup.recipeGroupIDs.ContainsKey("Wood"))
        {
            int groupIndex = RecipeGroup.recipeGroupIDs["Wood"];
            RecipeGroup group = RecipeGroup.recipeGroups[groupIndex];
            foreach (int item in group.ValidItems)
            {
                fuels.Add(item, 10);
            }
        }
        fuels.Add(ModContent.ItemType<Peat>(), 30);
        fuels.Add(ItemID.LavaBucket, 60);
        fuels.Add(ItemID.Meteorite, 120);
        fuels.Add(ItemID.Hellstone, 120);
    }

    public override bool? UseItem(Item item, Player player)
    {
        if (!fuels.ContainsKey(item.type))
        {
            return null;
        }
        var pos = Main.MouseWorld.ToTileCoordinates();
        if (Main.tile[pos.X, pos.Y].TileType != ModContent.TileType<Boiler>())
        {
            return null;
        }
        if (!player.InInteractionRange(pos.X, pos.Y, TileReachCheckSettings.Simple))
        {
            return null;
        }
        BoilerEntity boiler;
        if (!TileEntity.TryGet<BoilerEntity>(pos.X, pos.Y, out boiler))
        {
            return null;
        }
        if (boiler.DecrementFuelRequirement() || boiler.timers.Count <= 0)
        {
            boiler.timers.Push(fuels[item.type] * 60);
        }
        else
        {
            var curTimer = boiler.timers.Pop();
            curTimer += fuels[item.type] * 60;
            boiler.timers.Push(curTimer);
        }
        SoundEngine.PlaySound(SoundID.Item20, Main.MouseWorld);
        return true;
    }
}

public class BoilerItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Boiler>());
        Item.width = 30;
        Item.height = 24;
        Item.rare = ItemRarityID.Green;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddRecipeGroup(RecipeGroupID.IronBar, 12)
            .AddIngredient(ItemID.Torch, 9)
            .AddTile(TileID.Anvils)
            .Register();
    }
}
