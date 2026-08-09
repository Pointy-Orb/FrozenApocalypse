using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using FrozenApocalypse.Content.Tiles;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace FrozenApocalypse
{
    // Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
    public class FrozenApocalypse : Mod
    {
        public static class TileSets
        {
            public static bool[] SandIce = new bool[TileLoader.TileCount];
        }

        public override void PostSetupContent()
        {
            TileSets.SandIce = new bool[TileLoader.TileCount];
            TileSets.SandIce[ModContent.TileType<SandIce>()] = true;
            TileSets.SandIce[ModContent.TileType<EbonsandIce>()] = true;
            TileSets.SandIce[ModContent.TileType<CrimsandIce>()] = true;
            TileSets.SandIce[ModContent.TileType<PearlsandIce>()] = true;
        }

        public override void Load()
        {
            if (Main.dedServ)
            {
                return;
            }
            Asset<Effect> coldFilter = this.Assets.Request<Effect>("Assets/Effects/ColdShader");
            Filters.Scene["FrozenApocalypse:ColdFilter"] = new Filter(new ScreenShaderData(coldFilter, "ColdFilter"), EffectPriority.High);
            Filters.Scene["FrozenApocalypse:ColdFilter"].GetShader().UseColor(Color.Aqua);
            Filters.Scene["FrozenApocalypse:ColdFilter"].GetShader().UseSecondaryColor(Color.DodgerBlue);
            Asset<Texture2D> noise = this.Assets.Request<Texture2D>("Assets/Noise");
            Filters.Scene["FrozenApocalypse:ColdFilter"].GetShader().UseImage(noise);
            Filters.Scene["FrozenApocalypse:ColdFilter"].GetShader().UseIntensity(0.4f);
            Filters.Scene["FrozenApocalypse:ColdFilter"].GetShader().UseOpacity(0.2f);
        }
    }
}
