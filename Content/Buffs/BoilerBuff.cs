using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;

namespace FrozenApocalypse.Content.Buffs;

public class BoilerBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.buffNoSave[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
    {
        tip = Description.Format(Main.LocalPlayer.GetModPlayer<ColdDebuffPlayer>().numZoneBoilers);
    }
}
