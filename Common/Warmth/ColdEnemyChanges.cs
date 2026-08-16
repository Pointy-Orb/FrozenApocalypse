using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace FrozenApocalypse.Warmth;

public class ColdEnemyChanges : GlobalNPC
{
	public override void SetDefaults(NPC entity)
	{
		if(entity.type == NPCID.ZombieEskimo)
		{
			entity.coldDamage = true;
		}
	}
}
