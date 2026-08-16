using Terraria;
using System.Linq;
using Terraria.UI.Chat;
using Terraria.GameContent;
using System.Runtime.CompilerServices;
using Terraria.Localization;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ObjectData;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader.IO;
using FrozenApocalypse.Content.Gores;

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
        TileID.Sets.PreventsSandfall[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
        TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.newTile.HookPostPlaceMyPlayer = ModContent.GetInstance<BoilerEntity>().Generic_HookPostPlaceMyPlayer;
        TileObjectData.newTile.LavaDeath = false;
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
        else if (TileEntity.TryGet<BoilerEntity>(i, j, out BoilerEntity entity) && entity.timers.Count > 0)
        {
            Main.LocalPlayer.cursorItemIconEnabled = true;
            Main.LocalPlayer.cursorItemIconID = ItemID.LivingFireBlock;
        }
    }

    public override bool RightClick(int i, int j)
    {
        BoilerEntity entity;
        if (!TileEntity.TryGet<BoilerEntity>(i, j, out entity))
        {
            return false;
        }
        if (entity.timers.Count <= 0)
        {
            return false;
        }
        entity.DrawRange();
        SoundEngine.PlaySound(SoundID.MaxMana);
        return true;
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
        var alphaMult = Utils.Remap(entity.timers.Count, 1, 20, 0, 1);
        spriteBatch.Draw(
            glowTexture.Value,
            new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 2) + zero,
            new Rectangle(tile.TileFrameX, tile.TileFrameY + frameYOffset, 16, 16),
            Color.White * alphaMult, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        if (Main.hideUI)
        {
            return;
        }
        int frameX = Main.tile[i, j].TileFrameX;
        int frameY = Main.tile[i, j].TileFrameY;
        if (frameX > 17)
        {
            return;
        }
        if (frameY > 17)
        {
            return;
        }
        Vector2 textPos = new Vector2((i + 1) * 16 - (int)Main.screenPosition.X + 8, j * 16 - (int)Main.screenPosition.Y + 10) + zero;
        int curTimerValue = entity.timers.ToArray().Sum() / 60;
        var timer = TimeSpan.FromSeconds(curTimerValue);
        DefaultInterpolatedStringHandler handler = new(0, 1);
        if (timer.Hours > 0)
        {
            handler.AppendFormatted($"{timer.Hours}:");
        }
        handler.AppendFormatted($"{timer.Minutes.ToString(timer.Hours > 0 ? "D2" : "")}:");
        handler.AppendFormatted($"{timer.Seconds.ToString("D2")}");
        string text = handler.ToStringAndClear();
        var font = FontAssets.MouseText.Value;
        float size = 0.7f;
        Vector2 origin = font.MeasureString(text) * new Vector2(0.5f, 0.5f);
        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, text, textPos, Color.White, 0f, origin, Vector2.One * size);
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
            if (timers.Count > 0)
            {
                DrawRange();
                SoundEngine.PlaySound(SoundID.NPCHit22, WorldCenter);
            }
            else
            {
                SoundEngine.PlaySound(SoundID.LiquidsWaterLava, WorldCenter);
                var rand = Main.rand.Next(3);
                int id;
                switch (rand)
                {
                    case 1:
                        id = ModContent.GoreType<Smoke2>();
                        break;
                    case 2:
                        id = ModContent.GoreType<Smoke3>();
                        break;
                    default:
                    case 0:
                        id = ModContent.GoreType<Smoke1>();
                        break;
                }
                Gore.NewGorePerfect(new EntitySource_TileUpdate(Position.X, Position.Y), Position.ToWorldCoordinates(), new Vector2(0, -1), id, Main.rand.NextFloat(0.8f, 1.0f));
            }
        }
        AttemptUnfreeze();
    }

    private void AttemptUnfreeze(bool goAgain = true)
    {
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
        bool froze = TileFreezing.TryUnfreezeTile(unfreezeX, unfreezeY);
        if (!froze && goAgain)
        {
            AttemptUnfreeze(false);
        }
    }

    public bool TileInRange(int x, int y)
    {
        int distX = x - Center.X;
        int distY = y - Center.Y;
        return distX * distX + distY * distY <= timers.Count * timers.Count;
    }

    public bool EntityInRange(Vector2 pos)
    {
        float distX = pos.X - WorldCenter.X;
        float distY = pos.Y - WorldCenter.Y;
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
        tag["FuelUntilRangeIncrease"] = FuelUntilRangeIncrease;
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.ContainsKey("timers"))
        {
            var timerArray = tag.GetIntArray("timers");
            for (int i = timerArray.Length - 1; i >= 0; i--)
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
        double radians = 0;
        while (radians < MathHelper.PiOver4)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 pos = Center.ToWorldCoordinates();
                double relativeRadians = radians + i * MathHelper.PiOver4;
                pos += new Vector2((float)(timers.Count * Math.Sin(relativeRadians) * 16), (float)(timers.Count * Math.Cos(relativeRadians) * 16));
                Dust dust = Dust.NewDustPerfect(pos, DustID.AmberBolt, Vector2.Zero);
                dust.noGravity = true;
            }
            radians += (Math.PI * 0.0625) / (double)timers.Count;
        }
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
