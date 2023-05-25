using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Consolaria.Content.Items.Weapons.Melee {
	public class Tizona : ModItem {
		public override void SetStaticDefaults ()
			=> Item.ResearchUnlockCount = 1;

		public override void SetDefaults () {
			int width = 50; int height = width;
			Item.Size = new Vector2(width, height);

			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = Item.useAnimation = 24;

			Item.noMelee = true;
			Item.autoReuse = true;
			Item.shootsEveryUse = true;

			Item.DamageType = DamageClass.Melee;
			Item.damage = 104;
			Item.knockBack = 5;
			Item.crit = 5;

			Item.value = Item.buyPrice(gold: 5, silver: 50);
			Item.rare = ItemRarityID.Lime;
			Item.UseSound = SoundID.Item1;

			Item.shoot = ModContent.ProjectileType<Projectiles.Friendly.Tizona>();
			Item.shootSpeed = 12f;
		}

		public override bool Shoot (Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, new Vector2(player.direction, 0f), type, damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax * 2f);
			return false;
		}
	}
}