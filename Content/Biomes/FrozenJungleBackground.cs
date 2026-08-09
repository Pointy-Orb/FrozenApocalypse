using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace FrozenApocalypse.Content.Biomes;

public class FrozenJungleBackground : ModSurfaceBackgroundStyle
{
    public override int ChooseMiddleTexture()
    {
        return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Backgrounds/FrozenJungle/Front");
    }

    public override int ChooseFarTexture()
    {
        return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Backgrounds/FrozenJungle/Middle");
        //return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Assets/Backgrounds/FrozenJungle/Back");
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
