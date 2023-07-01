using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Consolaria.Content.Items.Pets {
    public class MysteriousPackage : PetItem 
	{
		public override void SetStaticDefaults () 
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults () {
			Item.DefaultToVanitypet(ModContent.ProjectileType<Content.Projectiles.Friendly.Pets.Drone>(), ModContent.BuffType<Content.Buffs.Drone>());

			int width = 32; int height = 32;
			Item.Size = new Vector2(width, height);

			Item.rare = 5;
			Item.value = Item.buyPrice(gold: 50);
		}
	}
}