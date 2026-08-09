using Terraria;
using System.Collections.Generic;
using FrozenApocalypse.Content.TileEntities;
using Terraria.ModLoader;
using Terraria.ID;

namespace FrozenApocalypse;

public class BoilerSystem : ModSystem
{
    public static List<BoilerEntity> boilers = new();

    public override void ClearWorld()
    {
        boilers.Clear();
    }

    public static bool TileInBoilerRange(int i, int j)
    {
        foreach (BoilerEntity boiler in boilers)
        {
            if (boiler.TileInRange(i, j))
            {
                return true;
            }
        }
        return false;
    }
}
