using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;
using System;

namespace FrozenApocalypse.Content.Gores;

public abstract class SmokeGore : ModGore
{
    public override void SetStaticDefaults()
    {
        ChildSafety.SafeGore[Type] = true;
    }

    private float baseScale;

    public override void OnSpawn(Gore gore, IEntitySource source)
    {
        gore.timeLeft = 70;
        baseScale = gore.scale;
    }

    public override bool Update(Gore gore)
    {
        if (gore.timeLeft > 0)
        {
            gore.timeLeft--;
        }
        gore.position += gore.velocity;
        gore.rotation += MathF.Atan2(gore.velocity.X, gore.velocity.Y) * 0.0025f;
        var goreTilePos = gore.position.ToTileCoordinates();
        gore.scale = baseScale + 0.1f * MathF.Sin(gore.timeLeft / 43f * 6.28318f);
        if (gore.timeLeft <= 0)
        {
            baseScale -= 0.01f;
        }
        if (baseScale <= 0)
        {
            gore.active = false;
        }
        return false;
    }
}

public class Smoke1 : SmokeGore
{

}

public class Smoke2 : SmokeGore
{

}

public class Smoke3 : SmokeGore
{

}
