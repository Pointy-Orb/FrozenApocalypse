using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using FrozenApocalypse.Content.TileEntities;
using FrozenApocalypse.Content.Buffs;
using System;
using Microsoft.Xna.Framework;

namespace FrozenApocalypse.Warmth;

public class ColdDebuffNPC : GlobalNPC
{
    public static ColdDebuffNPC GetInstance(NPC npc) => npc.GetGlobalNPC<ColdDebuffNPC>();

    public override bool InstancePerEntity => true;

    public int numZoneBoilers = 0;
    public bool boilerWarm => numZoneBoilers > 0;
    public bool wasBoilerWarm = false;

    public int ColdLevel { get; set; } = 0;
    public int WarmthLevel { get; set; } = 0;
    public static int NetColdLevel(NPC npc) => Math.Max(npc.GetGlobalNPC<ColdDebuffNPC>().ColdLevel - npc.GetGlobalNPC<ColdDebuffNPC>().WarmthLevel, 0);

    public override void ResetEffects(NPC npc)
    {
        ColdDebuffNPC coldNPC = npc.GetGlobalNPC<ColdDebuffNPC>();
        coldNPC.WarmthLevel = 0;
        coldNPC.ColdLevel = 0;
        coldNPC.numZoneBoilers = 0;
    }

    private void ManageBoiler(NPC npc)
    {
        ColdDebuffNPC coldNPC = npc.GetGlobalNPC<ColdDebuffNPC>();
        foreach (BoilerEntity boiler in BoilerSystem.boilers)
        {
            if (boiler.EntityInRange(npc.Center))
            {
                coldNPC.numZoneBoilers++;
                if (!coldNPC.wasBoilerWarm)
                {
                    boiler.DrawRange();
                }
            }
        }
        coldNPC.wasBoilerWarm = coldNPC.boilerWarm;
        if (coldNPC.boilerWarm)
        {
            npc.AddBuff(ModContent.BuffType<BoilerBuff>(), 20, true);
        }
    }

    private void CalculateWarmthLevel(NPC npc)
    {
        ColdDebuffNPC coldNPC = npc.GetGlobalNPC<ColdDebuffNPC>();
        if (npc.HasBuff(BuffID.Warmth))
        {
            coldNPC.WarmthLevel += 4;
        }
        coldNPC.WarmthLevel += 4 * coldNPC.numZoneBoilers;
        if (npc.HasBuff(BuffID.Campfire))
        {
            coldNPC.WarmthLevel += 1;
        }
        if (npc.HasBuff(BuffID.Tipsy))
        {
            coldNPC.WarmthLevel += 1;
        }
    }

    internal void CalculateColdLevel(NPC npc)
    {
        ColdDebuffNPC coldNPC = npc.GetGlobalNPC<ColdDebuffNPC>();
        int npcY = npc.Center.ToTileCoordinates().Y;
        if (npcY > TileFreezing.UpperBand)
        {
            return;
        }
        if (npc.lavaWet)
        {
            return;
        }
        coldNPC.ColdLevel = 1 + (TileFreezing.UpperBand - npcY) / TileFreezing.BandHeight;
    }

    public override bool PreAI(NPC npc)
    {
        ManageBoiler(npc);
        return true;
    }

    public override void PostAI(NPC npc)
    {
        ColdDebuffNPC coldNPC = npc.GetGlobalNPC<ColdDebuffNPC>();
        CalculateColdLevel(npc);
        CalculateWarmthLevel(npc);
        if ((coldNPC.ColdLevel > 0 && coldNPC.WarmthLevel < 1))
        {
            npc.AddBuff(BuffID.Chilled, 20);
        }
        if (NetColdLevel(npc) >= 5 && coldNPC.WarmthLevel <= 9)
        {
            npc.buffImmune[BuffID.Chilled] = false;
            npc.AddBuff(BuffID.Chilled, 20);
        }
        if (NetColdLevel(npc) >= Hypothermia.MinColdLevel)
        {
            npc.AddBuff(ModContent.BuffType<Hypothermia>(), 20);
        }
        if (NetColdLevel(npc) > 0 && !GetInstance(npc).boilerWarm && Main.rand.NextBool(Math.Max(800 - NetColdLevel(npc) * 10, 1)))
        {
            npc.AddBuff(BuffID.Frozen, 3600);
        }
    }

    public override void DrawEffects(NPC npc, ref Color drawColor)
    {
        if (npc.coldDamage)
        {
            return;
        }
        ColdDebuffNPC coldNPC = npc.GetGlobalNPC<ColdDebuffNPC>();
        Vector3 vColor = drawColor.ToVector3();
        vColor.X = Utils.Remap(NetColdLevel(npc), 0, 10, vColor.X, vColor.X * 0.3f, true);
        vColor.Y = Utils.Remap(NetColdLevel(npc), 0, 10, vColor.Y, vColor.Y * 0.5f, true);
        vColor.Z = Utils.Remap(NetColdLevel(npc), 0, 10, vColor.Z, 1, true);
        Color light = Lighting.GetColor((int)((double)npc.position.X + (double)npc.width * 0.5) / 16, (int)(((double)npc.position.Y + (double)npc.height * 0.5) / 16.0));
        drawColor = new Color(vColor).MultiplyRGB(light);
    }
}
