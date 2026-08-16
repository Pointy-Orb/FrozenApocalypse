using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using FrozenApocalypse.Warmth;

namespace FrozenApocalypse.Content.Buffs;

public class Frostnip : ModBuff
{
    public const int MinColdLevel = 1;
    public const int MaxColdLevel = 8;

    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.GetModPlayer<FrostnipPlayer>().frostnip = true;
    }
}

public class FrostnipPlayer : ModPlayer
{
    public bool frostnip = false;

    public override void ResetEffects()
    {
        frostnip = false;
    }

    public override void UpdateLifeRegen()
    {
        if (!frostnip)
        {
            return;
        }
        Player.lifeRegenTime = 0;
        Player.lifeRegen = Math.Min(Player.lifeRegen, 0);
    }

    public override void PostUpdateBuffs()
    {
        if (!frostnip)
        {
            return;
        }
        Player.PotionDelayModifier *= Utils.Remap(Player.GetModPlayer<ColdDebuffPlayer>().NetColdLevel, Frostnip.MinColdLevel, Frostnip.MaxColdLevel, 1.2f, 4.3f, true);
    }
}
