using Consolaria.Content.Buffs;
using Consolaria.Content.NPCs.Bosses.Turkor;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Consolaria.Content.Items.Summons {
    public class CursedStuffing : ModItem {
        public override void SetStaticDefaults () {
            Item.ResearchUnlockCount = 3;
            ItemID.Sets.SortingPriorityBossSpawns [Type] = 12;
        }

        public override void SetDefaults () {
            int width = 34; int height = 28;
            Item.Size = new Vector2(width, height);

            Item.maxStack = 9999;

            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ItemRarityID.Blue;

            Item.useAnimation = 30;
            Item.useTime = 30;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }

        public override bool CanUseItem (Player player)
            => player.HasBuff(ModContent.BuffType<PetTurkey>()) && !NPC.AnyNPCs(ModContent.NPCType<TurkortheUngrateful>());

        public override bool? UseItem (Player player) {
            if (player.whoAmI == Main.myPlayer) {
                player.ClearBuff(ModContent.BuffType<PetTurkey>());
                int type = ModContent.NPCType<TurkortheUngrateful>();
                Vector2 spawnPosition = new Vector2((int) player.Center.X, player.Center.Y);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
            }
            return true;
        }
    }
}