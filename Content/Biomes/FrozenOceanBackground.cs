using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace FrozenApocalypse.Content.Biomes;

public class FrozenOceanBackground : ModSurfaceBackgroundStyle
{
    public override int ChooseMiddleTexture()
    {
        return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Backgrounds/FrozenOcean/BG" + Main.oceanBG);
    }

    public override void ModifyFarFades(float[] fades, float transitionSpeed)
    {
        for (int i = 0; i < fades.Length; i++)
        {
            if (i == Slot)
            {
                fades[i] += transitionSpeed;
                if (fades[i] > 1f)
                {
                    fades[i] = 1f;
                }
            }
            else
            {
                fades[i] -= transitionSpeed;
                if (fades[i] < 0f)
                {
                    fades[i] = 0f;
                }
            }
        }
    }
}
