using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;
using System.Collections.Generic;

namespace FrozenApocalypse.Warmth;

public class FreezeDebuffChanges : GlobalBuff
{
    public override void Update(int type, NPC npc, ref int buffIndex)
    {
        if (type != BuffID.Frozen)
        {
            return;
        }
        if ((npc.lavaWet || npc.onFire || npc.onFire2 || npc.onFire3) && Main.netMode != NetmodeID.MultiplayerClient)
        {
            npc.DelBuff(buffIndex);
        }
        if (Main.LocalPlayer.TalkNPC == npc)
        {
            Main.npcChatText = "...";
        }
        FreezeDebuffNPC.GetInstance(npc).frozen = true;
    }

}

public class FreezeDebuffNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public static FreezeDebuffNPC GetInstance(NPC npc) => npc.GetGlobalNPC<FreezeDebuffNPC>();

    public bool frozen = false;

    int timeAirborne = 0;

    public override void SetDefaults(NPC entity)
    {
        entity.buffImmune[BuffID.Frozen] |= entity.coldDamage || entity.boss;
        if (NPCID.Sets.ProjectileNPC[entity.type]) entity.buffImmune[BuffID.Frozen] = true;
        if (NPCID.Sets.BelongsToInvasionOldOnesArmy[entity.type]) entity.buffImmune[BuffID.Frozen] = true;
        if (NPCID.Sets.ShouldBeCountedAsBoss[entity.type]) entity.buffImmune[BuffID.Frozen] = true;
        if (entity.aiStyle == NPCAIStyleID.MartianSaucer) entity.buffImmune[BuffID.Frozen] = true;
        if (entity.DoesntDespawnToInactivity()) entity.buffImmune[BuffID.Frozen] = true;
        if (entity.aiStyle == NPCAIStyleID.Worm) entity.buffImmune[BuffID.Frozen] = true;
    }

    public override void OnSpawn(NPC npc, IEntitySource source)
    {
        ColdDebuffNPC coldNPC = ColdDebuffNPC.GetInstance(npc);
        ModContent.GetInstance<ColdDebuffNPC>().CalculateColdLevel(npc);
    }

    public override void ResetEffects(NPC npc)
    {
        GetInstance(npc).frozen = false;
    }

    public override bool PreAI(NPC npc)
    {
        if (!GetInstance(npc).frozen)
        {
            GetInstance(npc).timeAirborne = 0;
            return true;
        }
        if (ColdDebuffNPC.GetInstance(npc).boilerWarm)
        {
            GetInstance(npc).frozen = false;
            int frozenIndex = npc.FindBuffIndex(BuffID.Frozen);
            if (frozenIndex < 0)
            {
                return true;
            }
            npc.DelBuff(frozenIndex);
            return true;
        }
        if (npc.noTileCollide)
        {
            npc.velocity *= 0.9f;
            return false; //Prevent npcs that ignore tiles from falling through the world
        }
        npc.velocity.X *= 0.9f;
        if (npc.position.Y == npc.oldPosition.Y)
        {
            GetInstance(npc).timeAirborne = 0;
        }
        GetInstance(npc).timeAirborne++;
        npc.frameCounter = 0;
        npc.velocity.Y = Math.Min(npc.gravity * timeAirborne, npc.maxFallSpeed);
        return false;
    }

    public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (!GetInstance(npc).frozen)
        {
            return;
        }
        Vector2 icePos = npc.Center - screenPos;
        icePos -= new Vector2(TextureAssets.Frozen.Width() / 2, TextureAssets.Frozen.Height() / 2);
        Color iceColor = Lighting.GetColor((int)((double)npc.position.X + (double)npc.width * 0.5) / 16, (int)(((double)npc.position.Y + (double)npc.height * 0.5) / 16.0));
        spriteBatch.Draw(TextureAssets.Frozen.Value, icePos, iceColor);
    }

    public override bool PreChatButtonClicked(NPC npc, bool firstButton)
    {
        return !GetInstance(npc).frozen;
    }
}
