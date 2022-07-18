﻿using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Consolaria.Content.Projectiles.Enemies;
using Terraria.DataStructures;

namespace Consolaria.Content.NPCs.Turkor
{
	[AutoloadBossHead]
	public class TurkortheUngratefulHead : ModNPC
	{
		private int turntimer = 0;
		private int timer = 0;

		private bool spawn = false;
		private bool charge = false;
		private bool projspam = false;
		private bool attackingphase = false;

		private float rotatepoint = 0;

		private bool chase = false;

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Turkor the Ungrateful Head");
			Main.npcFrameCount[NPC.type] = 3;

			NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData {
				SpecificallyImmuneTo = new int[] {
					BuffID.Poisoned,
					BuffID.Confused
				}
			};
			NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);

			NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
				Hide = true 
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
		}

		public override void SetDefaults() {
			int width = 50; int height = 100;
			NPC.Size = new Vector2(width, height);

			NPC.aiStyle = -1;
			NPC.netAlways = true;

			NPC.BossBar = null;

			NPC.damage = 40;
			NPC.defense = 10;
			NPC.lifeMax = 1200;

			NPC.dontTakeDamage = false;

			NPC.HitSound = SoundID.NPCHit7;
			NPC.DeathSound = new SoundStyle($"{nameof(Consolaria)}/Assets/Sounds/TurkorGobble");

			NPC.knockBackResist = 0f;
			NPC.noTileCollide = true;

			NPC.alpha = 255;

			NPC.lavaImmune = true;
			NPC.noGravity = true;
		}

		public override void ScaleExpertStats (int numPlayers, float bossLifeScale) {
			NPC.lifeMax = 2000 + (int) (numPlayers > 1 ? NPC.lifeMax * 0.2 * numPlayers : 0);
			NPC.damage = (int) (NPC.damage * 0.65f);
		}

		private Rectangle GetFrame(int number)
			=> new Rectangle(0, NPC.frame.Height * (number - 1), NPC.frame.Width, NPC.frame.Height);

		public override void FindFrame(int frameHeight) {
			if (!attackingphase || charge && timer > 230 || projspam) {
				if (!projspam && NPC.velocity.X * NPC.direction < 0 && turntimer < 15) {
					turntimer++;
					NPC.frame = GetFrame(3);
				}
				else {
					if (NPC.velocity.X * NPC.direction > 0) { turntimer = 0; }
					NPC.spriteDirection = NPC.direction;
					NPC.frameCounter += 0.08f;
					NPC.frameCounter %= 2;
					int frame = (int)NPC.frameCounter;
					NPC.frame.Y = frame * frameHeight;
				}
			}
			if (charge && timer <= 230) NPC.frame = GetFrame(3);	
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot) => charge;
		
		public override void AI() {
			NPC.direction = Main.player[NPC.target].Center.X < NPC.Center.X ? -1 : 1;
			if (!spawn) {
				NPC.realLife = NPC.whoAmI;
				int neck = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X, (int)NPC.position.Y, ModContent.NPCType<TurkorNeck>(), NPC.whoAmI, 0, NPC.whoAmI); //, 1, NPC.ai[1]);
				Main.npc[neck].localAI[0] = 30;
				Main.npc[neck].realLife = NPC.whoAmI;
				Main.npc[neck].ai[0] = NPC.whoAmI;
				Main.npc[neck].ai[1] = NPC.whoAmI;
				spawn = true;
			}
			if (!Main.npc[(int)NPC.ai[1]].active) {
				NPC.life = 0;
				NPC.HitEffect(0, 10.0);
				NPC.active = false;
			}
			if (NPC.alpha >= 0) NPC.alpha -= 15;
		
			timer++;
			if (Main.player[NPC.target].dead) {
				timer = 0;
				charge = false;
				NPC.TargetClosest(false);
				NPC.velocity.Y -= 0.1f;
				if (NPC.timeLeft > 10 && Main.player[NPC.target].dead) {
					NPC.timeLeft = 10;
					return;
				}
			}
			else if (!Main.player[NPC.target].dead) NPC.TargetClosest(true);

			if (timer > 200 && Main.rand.Next(50) == 0 && !attackingphase) {
				attackingphase = true;
				//pick random attack
				switch (Main.rand.Next(2)) {
					case 0:
						charge = true;
						break;

					case 1:
						projspam = true;
						break;

					default:
						break;
				}
				timer = 200;
				NPC.velocity *= 0.46f;
				NPC.rotation = 0;
			}

			//attack1: charge at player
			if (charge) {
				if (timer <= 230) {
					NPC.rotation = Vector2.UnitY.RotatedBy((double)(timer / 40f * 6.2f), default(Vector2)).Y * 0.2f;
				}
				if (timer >= 230) {
					NPC.rotation = 0;
					if (timer <= 230) SoundEngine.PlaySound(new SoundStyle($"{nameof(Consolaria)}/Assets/Sounds/TurkorGobble"), NPC.position); // SoundEngine.PlaySound(3, (int)NPC.position.X, (int)NPC.position.Y, 10);
					
					NPC.velocity.X *= 0.98f;
					NPC.velocity.Y *= 0.98f;
					Vector2 vector8 = new Vector2(NPC.position.X + (NPC.width * 0.5f), NPC.position.Y + (NPC.height * 0.5f)); {
						float rotation = (float)Math.Atan2((vector8.Y) - (Main.player[NPC.target].position.Y + (Main.player[NPC.target].height * 0.5f)), (vector8.X) - (Main.player[NPC.target].position.X + (Main.player[NPC.target].width * 0.5f)));
						NPC.velocity.X = (float)(Math.Cos(rotation) * 14) * -1;
						NPC.velocity.Y = (float)(Math.Sin(rotation) * 14) * -1;
					}
				}

				//if near player then bounce back
				if (timer >= 230 && Main.player[NPC.target].Distance(NPC.Center) <= 30) {
					timer = 0;
					charge = false;
					attackingphase = false;
					NPC.velocity.X *= -0.38f;
					NPC.velocity.Y *= -0.38f;
				}

				//if hit/running out of time then bounce back
				if (timer > 270 || timer > 230 && NPC.justHit) {
					timer = 0;
					charge = false;
					attackingphase = false;
					NPC.velocity.X *= -0.38f;
					NPC.velocity.Y *= -0.38f;
				}
			}

			//attck2: spawn feather around it self while slowly drift toward the player
			if (projspam) {
				if (NPC.spriteDirection == -1) NPC.rotation = rotatepoint;		
				else NPC.rotation = -rotatepoint;	
				if (rotatepoint <= 1.5f && timer < 360) rotatepoint += 0.1f;	
				if (!chase) {
					NPC.velocity.X *= 0.86f;
					NPC.velocity.Y *= 0.86f;
				}

				if (timer % 80 == 0 && rotatepoint >= 1.5f) {
					for (int i = 0; i < 3; i++)
						Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, Main.rand.Next(0, 8) * NPC.direction, -10 + Main.rand.Next(-3, 3), ModContent.ProjectileType<TurkorFeather>(), (int)(NPC.damage / 3), 1, Main.myPlayer, 0, 0);				
					NPC.velocity.Y = 5;
				}
				if (timer >= 360) {
					rotatepoint -= 0.1f;
					if (rotatepoint <= 0) {
						timer = 0;
						projspam = false;
						attackingphase = false;
						NPC.rotation = 0;
					}
				}
			}

			if (!charge) {
				if (!chase) {
					if (Main.player[NPC.target].Center.X - Main.rand.Next(-200, 201) < NPC.Center.X)
					{
						if (NPC.velocity.X > -6) NPC.velocity.X -= 0.08f;
					}
					else if (Main.player[NPC.target].Center.X - Main.rand.Next(-200, 201) > NPC.Center.X)
					{
						if (NPC.velocity.X < 6) NPC.velocity.X += 0.08f;
					}
					if (Main.player[NPC.target].Center.Y - Main.rand.Next(-150, 201) < NPC.Center.Y)
					{
						if (NPC.velocity.Y > -6) NPC.velocity.Y -= 0.14f;
					}
					else if (Main.player[NPC.target].Center.Y - Main.rand.Next(-150, 201) > NPC.Center.Y)
					{
						if (NPC.velocity.Y < 6) NPC.velocity.Y += 0.14f;
					}
				}
				else {
					if (Main.npc[(int)NPC.ai[1]].Center.X - Main.rand.Next(-200, 201) < NPC.Center.X)
					{
						if (NPC.velocity.X > -6) NPC.velocity.X -= 0.08f;
					}
					else if (Main.npc[(int)NPC.ai[1]].Center.X - Main.rand.Next(-200, 201) > NPC.Center.X)
					{
						if (NPC.velocity.X < 6) NPC.velocity.X += 0.08f;
					}
					if (Main.npc[(int)NPC.ai[1]].Center.Y - Main.rand.Next(-150, 201) < NPC.Center.Y)
					{
						if (NPC.velocity.Y > -6) NPC.velocity.Y -= 0.14f;
					}
					else if (Main.npc[(int)NPC.ai[1]].Center.Y - Main.rand.Next(-150, 201) > NPC.Center.Y)
					{
						if (NPC.velocity.Y < 6) NPC.velocity.Y += 0.14f;
					}
				}
				Vector2 vector101 = new Vector2(NPC.Center.X, NPC.Center.Y);
				float num855 = Main.npc[(int)NPC.ai[1]].Center.X - vector101.X;
				float num856 = Main.npc[(int)NPC.ai[1]].Center.Y - vector101.Y;
				float num857 = (float)Math.Sqrt((double)(num855 * num855 + num856 * num856));
				if (num857 > 600f) chase = true;		
				if (num857 <= 400 && chase) chase = false;	
			}
		}

		public override void HitEffect(int hitDirection, double damage) {
			if (NPC.life <= 0) {
				if (NPC.life <= 0) {
					Gore.NewGore(NPC.GetSource_Death(), NPC.position, new Vector2(Main.rand.Next(-3, 4), Main.rand.Next(-3, 4)), ModContent.Find<ModGore>("Consolaria/TurkorBeakGore").Type);
					Gore.NewGore(NPC.GetSource_Death(), NPC.position, new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, 6)), ModContent.Find<ModGore>("Consolaria/TurkorEyeGore").Type);
					Gore.NewGore(NPC.GetSource_Death(), NPC.position, new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, 6)), ModContent.Find<ModGore>("Consolaria/TurkorEyeGore").Type);
					Gore.NewGore(NPC.GetSource_Death(), NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), ModContent.Find<ModGore>("Consolaria/TurkorFeatherGore").Type);
					Gore.NewGore(NPC.GetSource_Death(), NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), ModContent.Find<ModGore>("Consolaria/TurkorFeatherGore").Type);
					Gore.NewGore(NPC.GetSource_Death(), NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), ModContent.Find<ModGore>("Consolaria/TurkorFeatherGore").Type);
				}
				for (int k = 0; k < 10; k++) {
					int dust_ = Dust.NewDust(NPC.position, NPC.width, NPC.height, 26, 3f * hitDirection, -3f, 0, default, 2f);
					Main.dust[dust_].velocity *= 0.2f;
				}
			}
		}
	}
}