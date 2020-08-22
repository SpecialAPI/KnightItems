using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.IO;
using ItemAPI;
using MonoMod.RuntimeDetour;
using Gungeon;
using Dungeonator;
using UnityEngine;

namespace KnightItems
{
    public class KnightItemsModule : ETGModule
    {
        public override void Init()
        {
        }

        public override void Start()
        {
            FakePrefabHooks.Init();
            ItemBuilder.Init();
            SoulVessel.Init();
            ShadeCloak.Init();
            Directory.CreateDirectory("HKItemsConfig");
            if (!File.Exists("HKItemsConfig/nailmode.json"))
            {
                File.WriteAllText("HKItemsConfig/nailmode.json", "normal");
            }
            if(!KnightItemsModule.AvailableNailModes.Contains(File.ReadAllText("HKItemsConfig/nailmode.json")))
            {
                File.WriteAllText("HKItemsConfig/nailmode.json", "normal");
            }
            KnightItemsModule.NailMode = File.ReadAllText("HKItemsConfig/nailmode.json");
            Hook attackHook = new Hook(
                typeof(Gun).GetMethod("Attack", BindingFlags.Public | BindingFlags.Instance),
                typeof(KnightItemsModule).GetMethod("AttackHook")
            );
            NailController.Add();
            EmptySoul.Init();
            SoulVesselGifter.Init();
            KnightItemsModule.trulyVengefulSynergy = new TrulyVengefulSynergy();
            KnightItemsModule.soulCloakSynergy = new SoulCloakSynergy();
            KnightItemsModule.verySharpShadowSynergy = new VerySharpShadowSynergy();
            KnightItemsModule.twoBladesSynergy = new TwoBladesSynergy();
            GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { trulyVengefulSynergy }).ToArray();
            GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { soulCloakSynergy }).ToArray();
            GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { verySharpShadowSynergy }).ToArray();
            GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { twoBladesSynergy }).ToArray();
            Hook synergyStringHook = new Hook(
                typeof(StringTableManager).GetMethod("GetSynergyString", BindingFlags.Public | BindingFlags.Static),
                typeof(KnightItemsModule).GetMethod("SynergyStringHook")
            );
            ETGModMainBehaviour.Instance.gameObject.AddComponent<SynergyTranslationUpdateBehaviour>();
        }

        public static string SynergyStringHook(Func<string, int, string> action, string key, int index = -1)
        {
            string value = action(key, index);
            if (string.IsNullOrEmpty(value))
            {
                value = key;
            }
            return value;
        }

        public static Gun.AttackResult AttackHook(Func<Gun,ProjectileData,GameObject,Gun.AttackResult> orig, Gun self, ProjectileData overrideProjectileData, GameObject overrideBulletObject)
        {
            if(self.GetComponent<NailController>() != null)
            {
                FieldInfo d = typeof(Gun).GetField("m_anim", BindingFlags.NonPublic | BindingFlags.Instance);
                if (self.GetComponent<NailController>().SwordCooldown <= 0 && !(d.GetValue(self) as tk2dSpriteAnimator).IsPlaying(self.shootAnimation) && !(d.GetValue(self) as tk2dSpriteAnimator).IsPlaying(self.reloadAnimation))
                {
                    if (KnightItemsModule.NailMode == "grubberfly")
                    {
                        bool flag2 = self.Volley != null && self.Volley.projectiles != null;
                        if (flag2)
                        {
                            foreach (ProjectileModule projectileModule in self.Volley.projectiles)
                            {
                                bool flag3 = projectileModule != self.DefaultModule;
                                if (flag3)
                                {
                                    bool flag4 = self.CurrentOwner.healthHaver.GetCurrentHealthPercentage() >= 1f;
                                    if (flag4)
                                    {
                                        self.ForceFireProjectile(projectileModule.GetCurrentProjectile());
                                    }
                                }
                            }
                        }
                    }
                    self.PreventNormalFireAudio = true;
                    if (((self.CurrentOwner as PlayerController).HasPickupID(417) || (self.CurrentOwner as PlayerController).HasPickupID(815)) && self.CurrentOwner.healthHaver.GetCurrentHealthPercentage() >= 1f)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            float offset;
                            if (i == 1)
                            {
                                offset = -30;
                            }
                            else
                            {
                                offset = 30;
                            }
                            Gun bulletSword = (Gun)PickupObjectDatabase.GetById(417);
                            float num = (!(self.CurrentOwner != null)) ? 1f : (self.CurrentOwner as PlayerController).stats.GetStatValue(PlayerStats.StatType.Accuracy);
                            num = ((!(self.CurrentOwner is DumbGunShooter) || !(self.CurrentOwner as DumbGunShooter).overridesInaccuracy) ? num : (self.CurrentOwner as DumbGunShooter).inaccuracyFraction);
                            float angleForShot = self.DefaultModule.GetAngleForShot(self.RuntimeModuleData[self.DefaultModule].alternateAngleSign, num, null);
                            Vector3 a = self.barrelOffset.position;
                            a = new Vector3(a.x, a.y, -1f);
                            GameObject gameObject = SpawnManager.SpawnProjectile(bulletSword.DefaultModule.GetCurrentProjectile().gameObject, a + Quaternion.Euler(0f, 0f, self.CurrentAngle) * self.DefaultModule.positionOffset, Quaternion.Euler(0f, 0f, self.CurrentAngle + angleForShot + offset), true);
                            Projectile projectile = gameObject.GetComponent<Projectile>();
                            if (projectile != null)
                            {
                                projectile.Owner = self.CurrentOwner;
                                projectile.Shooter = self.CurrentOwner.specRigidbody;
                                projectile.baseData.damage /= 2;
                            }
                        }
                    }
                    AkSoundEngine.PostEvent("Play_WPN_blasphemy_shot_01", self.gameObject);
                }
            }
            Gun.AttackResult value = orig(self, overrideProjectileData, overrideBulletObject);
            if (self.GetComponent<NailController>() != null)
            {
                FieldInfo d = typeof(Gun).GetField("HeroSwordCooldown", BindingFlags.NonPublic | BindingFlags.Instance);
                d.SetValue(self, 0);
            }
            return value;
        }

        public override void Exit()
        {
        }

        public static readonly List<string> AvailableNailModes = new List<string>
        {
            "normal", "hungry", "grubberfly", "super"
        };

        public static string NailMode = "normal";

        public static AdvancedSynergyEntry trulyVengefulSynergy = null;
        public static AdvancedSynergyEntry soulCloakSynergy = null;
        public static AdvancedSynergyEntry verySharpShadowSynergy = null;
        public static AdvancedSynergyEntry twoBladesSynergy = null;
    }
}
