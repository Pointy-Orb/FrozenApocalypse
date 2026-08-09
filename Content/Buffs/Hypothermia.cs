using Terraria;
using System;
using Terraria.ModLoader;
using MonoMod.Cil;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Utilities;
using static Mono.Cecil.Cil.OpCodes;

namespace FrozenApocalypse.Content.Buffs;

public class Hypothermia : ModBuff
{
    public const int MinColdLevel = 3;
    public const int MaxColdLevel = 12;

    public override void Load()
    {
        IL_Player.UpdateLifeRegen += HypothermiaDeathIL;
    }

    public override void Unload()
    {
        IL_Player.UpdateLifeRegen -= HypothermiaDeathIL;
    }

    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        Main.buffNoSave[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.GetModPlayer<HypothermiaPlayer>().hypothermia = true;
    }

    private static void HypothermiaDeathIL(ILContext il)
    {
        try
        {
            ILCursor c = new ILCursor(il);
            ILLabel statementBreakLabel = il.DefineLabel();
            ILLabel elseJumpLabel = il.DefineLabel();
            ILLabel hypothermiaLabel = il.DefineLabel();
            c.Index = il.Instrs.Count - 1;
            c.GotoPrev(i => i.MatchLdarg0());
            c.MarkLabel(statementBreakLabel);
            c.GotoPrev(i => i.MatchBrfalse(out elseJumpLabel));
            c.Remove();
            c.Emit(Brfalse_S, hypothermiaLabel);
            c.GotoNext(MoveType.After, i => i.MatchBr(out _));

            c.Emit(Ldarg_0);
            c.EmitDelegate<Func<Player, bool>>((player) => player.GetModPlayer<HypothermiaPlayer>().hypothermia);
            c.Emit(Brfalse_S, elseJumpLabel);

            c.GotoPrev(i => i.MatchLdarg0());
            c.MarkLabel(hypothermiaLabel);
            c.GotoNext(MoveType.After, i => i.MatchBrfalse(elseJumpLabel));

            c.Emit(Ldarg_0);
            c.EmitDelegate<Action<Player>>((Player player) =>
            {
                PlayerDeathReason deathReason = PlayerDeathReason.ByCustomReason(NetworkText.FromKey($"Mods.FrozenApocalypse.DeathMessages.FreezeDeath{Main.rand.Next(5)}", player.name));
                player.KillMe(deathReason, 10, 0, false);
            });
            c.Emit(Br_S, statementBreakLabel);
        }
        catch
        {
            MonoModHooks.DumpIL(ModContent.GetInstance<FrozenApocalypse>(), il);
            ModContent.GetInstance<FrozenApocalypse>().Logger.Error("Hook HypothermiaDeathIL failed to load. IL log dumped.");
        }
    }
}

public class HypothermiaPlayer : ModPlayer
{
    public bool hypothermia;

    public override void ResetEffects()
    {
        hypothermia = false;
    }

    public int DebuffDamage => (int)Utils.Remap(Player.GetModPlayer<ColdDebuffPlayer>().NetColdLevel, Hypothermia.MinColdLevel, Hypothermia.MaxColdLevel, 2, 6);

    public override void UpdateBadLifeRegen()
    {
        if (!hypothermia)
        {
            return;
        }
        Player.lifeRegenTime = 0;
        Player.lifeRegen -= DebuffDamage;
    }
}
