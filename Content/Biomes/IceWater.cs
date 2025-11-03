using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace FrozenApocalypse.Content.Biomes;

public class IceWaterStyle : ModWaterStyle
{
    public override int GetSplashDust() => DustID.Water_Snow;
    public override int ChooseWaterfallStyle() => ModContent.GetInstance<IceWaterfallStyle>().Slot;
    public override int GetDropletGore() => GoreID.WaterDripIce;
}

public class IceWaterfallStyle : ModWaterfallStyle
{
}
