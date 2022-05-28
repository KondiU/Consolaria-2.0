﻿using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Consolaria.Content.Projectiles.Enemies;
using Terraria.ModLoader.Utilities;

namespace Consolaria.Content.NPCs
{
    public class SpectralElemental : ModNPC
    {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Spectral Elemental");
            Main.npcFrameCount[NPC.type] = 15;

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Poisoned
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
                Velocity = 1f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults() {
            int width = 40; int height = 24;
            NPC.Size = new Vector2(width, height);

            NPC.lifeMax = 400;
            NPC.damage = 40;

            NPC.defense = 30;
            NPC.knockBackResist = 0.6f;

            NPC.value = Item.buyPrice(silver: 10);

            NPC.aiStyle = 3;
            AIType = NPCID.ChaosElemental;
            AnimationType = NPCID.ChaosElemental;

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath6;

            //banner = NPC.type;
            //bannerItem = mod.ItemType("SpectralElementalBanner");
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundHallow,
                new FlavorTextBestiaryInfoElement("Once living beings infused by the blessing, they erratically make chase with an uncontrollable surge of luminous power.")
            });
        }

        public override void AI() {
            Player player = Main.player[NPC.target];
            NPC.localAI[0]++;
            Vector2 playerPos = new(player.position.X, player.position.Y);
            if (NPC.localAI[0] >= 300 + Main.rand.Next(0, 180) && Vector2.Distance(NPC.Center, player.Center) < 300f) {
                for (int I = 0; I < 20; I++)
                    Dust.NewDust(NPC.position, NPC.width - (Main.rand.Next(NPC.width)), NPC.height - (Main.rand.Next(NPC.height)), DustID.BlueFairy, (float)Main.rand.Next(-5, 5), Main.rand.Next(-5, 5), 100, default, 1.1f);    
                Teleport(playerPos);
                NPC.localAI[0] = 0;
            }
            Lighting.AddLight(NPC.Center, new Vector3(0f, 0.7f, 0.9f));
        }

        private void Teleport(Vector2 playerPosition) {
            Player player = Main.player[NPC.target];
            Vector2 teleportTo = new(playerPosition.X + 140 * player.direction, playerPosition.Y);
            Vector2 teleportFrom = new(NPC.position.X, NPC.position.Y);
            Vector2 NormalizedVec = new(0, -2f);
            NormalizedVec.Normalize();
            if (!Collision.SolidCollision(teleportTo, 16, 16)) {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), teleportFrom, NormalizedVec * 4f, ModContent.ProjectileType<SpectralBomb>(), 30, 4, player.whoAmI);
                NPC.position = teleportTo;
            }
        }

        public override void HitEffect(int hitDirection, double damage) {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, 185, 2.5f * (float)hitDirection, -2.5f, 0, default, 0.8f);
            if (NPC.life <= 0) {
                for (int i = 0; i < 25; i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 185, 2.5f * (float)hitDirection, -2.5f, 0, default, 1f);      
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
            SpriteEffects effects = (NPC.spriteDirection == -1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 origin = new Vector2((float)(texture.Width / 2), (float)(texture.Height / Main.npcFrameCount[NPC.type] / 2));
            spriteBatch.Draw(texture, new Vector2(NPC.position.X - Main.screenPosition.X + (float)(NPC.width / 2) - (float)texture.Width * NPC.scale / 2f + origin.X * NPC.scale, NPC.position.Y - Main.screenPosition.Y + (float)NPC.height - (float)texture.Height * NPC.scale / (float)Main.npcFrameCount[NPC.type] + 4f + origin.Y * NPC.scale), new Rectangle?(NPC.frame), Color.White, NPC.rotation, origin, NPC.scale, effects, 0f);
            for (int i = 1; i < NPC.oldPos.Length; i++) {
                Color color = Lighting.GetColor((int)((double)NPC.position.X + (double)NPC.width * 0.5) / 16, (int)(((double)NPC.position.Y + (double)NPC.height * 0.5) / 16.0));
                Color color2 = color;
                color2 = Color.Lerp(color2, Color.LightSkyBlue, 0.8f);
                color2 = NPC.GetAlpha(color2);
                color2 *= (float)(NPC.oldPos.Length - i) / 15f;
                spriteBatch.Draw(texture, new Vector2(NPC.position.X - Main.screenPosition.X + (float)(NPC.width / 2) - (float)texture.Width * NPC.scale / 2f + origin.X * NPC.scale, NPC.position.Y - Main.screenPosition.Y + (float)NPC.height - (float)texture.Height * NPC.scale / (float)Main.npcFrameCount[NPC.type] + 4f + origin.Y * NPC.scale) - NPC.velocity * (float)i * 0.5f, new Rectangle?(NPC.frame), color2, NPC.rotation, origin, NPC.scale, effects, 0f);
            }
            return true;
        }

        /*public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("Consolaria/Assets/Textures/NPCs/SpectralElemental_Glow");
            SpriteEffects Effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY), NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, Effects, 0);
        }*/

        public override void ModifyNPCLoot(NPCLoot npcLoot) {
            var elementalDropRules = Main.ItemDropsDB.GetRulesForNPCID(NPCID.ChaosElemental, true);
            foreach (var elementalDropRule in elementalDropRules)
                npcLoot.Add(elementalDropRule);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) 
            => (spawnInfo.Player.ZoneHallow && spawnInfo.SpawnTileY < Main.rockLayer) ? SpawnCondition.EnchantedSword.Chance * 0.33f : 0f;    
    }
}