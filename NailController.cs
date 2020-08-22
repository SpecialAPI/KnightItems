using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Text;
using Gungeon;
using UnityEngine;
using ItemAPI;

namespace KnightItems
{
    public class NailController : GunBehaviour
    {
        public static void Add()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("The Nail", "nail");
            Game.Items.Rename("outdated_gun_mods:the_nail", "spapi:the_nail");
            gun.gameObject.AddComponent<NailController>();
            GunExt.SetShortDescription(gun, "Trusty, Not Safe");
            GunExt.SetLongDescription(gun, "A well worn sword from far away.\n\nAs melee weapons are rejected in Gungeon, this thing is far from being safe.");
            GunExt.SetupSprite(gun, null, "nail_idle_001", 1);
            GunExt.SetAnimationFPS(gun, gun.shootAnimation, 16);
            GunExt.SetAnimationFPS(gun, gun.idleAnimation, 1);
            GunExt.SetAnimationFPS(gun, gun.reloadAnimation, 1);
            GunExt.AddProjectileModuleFrom(gun, "wonderboy", true, true);
            if(KnightItemsModule.NailMode == "super" || KnightItemsModule.NailMode == "grubberfly")
            {
                GunExt.AddProjectileModuleFrom(gun, "boltRifle", true, false);
                foreach (ProjectileModule mod in gun.Volley.projectiles)
                {
                    if (mod != null)
                    {
                        if (mod != gun.DefaultModule)
                        {
                            Gun gun3 = (Gun)ETGMod.Databases.Items["boltRifle"];
                            if (gun3 != null)
                            {
                                mod.projectiles.Add(gun3.DefaultModule.GetCurrentProjectile());
                            }
                            mod.angleVariance = 0;
                        }
                    }
                }
            }
            gun.SetBaseMaxAmmo(100);
            gun.reloadTime = 0f;
            gun.DefaultModule.cooldownTime = 1.5f;
            gun.InfiniteAmmo = true;
            gun.DefaultModule.numberOfShotsInClip = int.MaxValue;
            gun.quality = PickupObject.ItemQuality.SPECIAL;
            gun.encounterTrackable.EncounterGuid = "the_nail";
            ETGMod.Databases.Items.Add(gun, null, "ANY");
            gun.CanBeDropped = false;
            gun.InfiniteAmmo = true;
            ItemBuilder.AddPassiveStatModifier(gun, PlayerStats.StatType.Curse, 1, StatModifier.ModifyMethod.ADDITIVE);
            Gun gun2 = (Gun)ETGMod.Databases.Items["wonderboy"];
            gun.muzzleFlashEffects = gun2.muzzleFlashEffects;
            gun.IsHeroSword = true;
            bool doesntblank = KnightItemsModule.NailMode == "normal" || KnightItemsModule.NailMode == "grubberfly";
            gun.HeroSwordDoesntBlank = doesntblank;
            gun.DefaultModule.GetCurrentProjectile().baseData.damage = 20;
        }

        public void Update()
        {
            FieldInfo d = typeof(Gun).GetField("HeroSwordCooldown", BindingFlags.NonPublic | BindingFlags.Instance);
            this.SwordCooldown = (float)d.GetValue(this.gun);
            if (this.gun != null)
            {
                if(this.gun.ClipShotsRemaining < this.gun.DefaultModule.numberOfShotsInClip)
                {
                    this.gun.ForceImmediateReload(false);
                }
                if (GameManager.Options.CurrentLanguage != this.lastLang)
                {
                    if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.RUSSIAN)
                    {
                        this.gun.SetName("Гвоздь");
                        this.gun.SetShortDescription("Верный, ненадежный");
                        this.gun.SetLongDescription("Поношенный меч из далекого королевства.\n\nТак как оружия ближнего боя запрещены в Оружелье, эта вещь далеко не надежная.");
                    }
                    else if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ENGLISH)
                    {
                        this.gun.SetName("The Nail");
                        this.gun.SetShortDescription("Trusty, Not Safe");
                        this.gun.SetLongDescription("A well worn sword from far away.\n\nAs melee weapons are rejected in Gungeon, this thing is far from being safe.");
                    }
                    this.lastLang = GameManager.Options.CurrentLanguage;
                }
            }
        }

        private StringTableManager.GungeonSupportedLanguages lastLang = StringTableManager.GungeonSupportedLanguages.ENGLISH;

        public override void PostProcessProjectile(Projectile projectile)
        {
            PlayerController playerController = this.gun.CurrentOwner as PlayerController;
            bool flag = playerController == null;
            bool flag2 = flag;
            if (flag2)
            {
                this.gun.ammo = this.gun.GetBaseMaxAmmo();
            }
            if(projectile != null)
            {
                if(KnightItemsModule.NailMode == "super")
                {
                    ProjectileModule projectileModule = null;
                    if (this.gun.Volley != null)
                    {
                        for (int i = 0; i < this.gun.Volley.projectiles.Count; i++)
                        {
                            for (int j = 0; j < this.gun.Volley.projectiles[j].projectiles.Count; j++)
                            {
                                if (projectile.name.Contains(this.gun.Volley.projectiles[j].projectiles[j].name))
                                {
                                    projectileModule = this.gun.Volley.projectiles[j];
                                    break;
                                }
                            }
                            if (projectileModule != null)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int k = 0; k < this.gun.singleModule.projectiles.Count; k++)
                        {
                            if (projectile.name.Contains(this.gun.singleModule.projectiles[k].name))
                            {
                                projectileModule = this.gun.singleModule;
                                break;
                            }
                        }
                    }
                    if (projectileModule == this.gun.DefaultModule)
                    {
                        UnityEngine.Object.Destroy(projectile.gameObject);
                    }
                    else
                    {
                        projectile.gameObject.AddComponent<NailSlashProj>();
                        projectile.hitEffects = this.gun.DefaultModule.GetCurrentProjectile().hitEffects;
                        PierceProjModifier pierceMod = projectile.gameObject.AddComponent<PierceProjModifier>();
                        pierceMod.penetratesBreakables = true;
                        pierceMod.penetration = int.MaxValue;
                    }
                }
                else if(KnightItemsModule.NailMode == "hungry")
                {
                    UnityEngine.Object.Destroy(projectile);
                }
                else if(KnightItemsModule.NailMode == "grubberfly")
                {
                    projectile.gameObject.AddComponent<NailSlashProj>();
                    projectile.hitEffects = this.gun.DefaultModule.GetCurrentProjectile().hitEffects;
                    PierceProjModifier pierceProjModifier = projectile.gameObject.AddComponent<PierceProjModifier>();
                    pierceProjModifier.penetratesBreakables = true;
                    pierceProjModifier.penetration = int.MaxValue;
                }
            }
        }

        public NailController()
        {
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            
        }

        public static ProjectileModule CopyFrom(ProjectileModule origin)
        {
            ProjectileModule mod = new ProjectileModule();
            return mod;
        }

        public float SwordCooldown = 0;
    }
}
