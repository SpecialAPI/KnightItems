using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod.RuntimeDetour;

namespace KnightItems
{
    class SoulVessel : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Soul Vessel";
            string resourceName = SoulVessel.spritePaths[3];
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<SoulVessel>();
            SoulVessel.spriteIDs = new int[SoulVessel.spritePaths.Length];
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj, true);
            SoulVessel.spriteIDs[0] = SpriteBuilder.AddSpriteToCollection(SoulVessel.spritePaths[0], item.sprite.Collection);
            SoulVessel.spriteIDs[1] = SpriteBuilder.AddSpriteToCollection(SoulVessel.spritePaths[1], item.sprite.Collection);
            SoulVessel.spriteIDs[2] = SpriteBuilder.AddSpriteToCollection(SoulVessel.spritePaths[2], item.sprite.Collection);
            SoulVessel.spriteIDs[3] = item.sprite.spriteId;
            string shortDesc = "Contains Soul";
            string longDesc = "Use to release a damaging spell.\n\nThe SOUL is a essence of life in a far away kingdom Hallownest. It kinda works in Gungeon too, so you can still use it there.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 0.5f);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalItemCapacity, 1, StatModifier.ModifyMethod.ADDITIVE);
            item.consumable = false;
            item.quality = ItemQuality.SPECIAL;
            item.CanBeDropped = false;
            Hook hook = new Hook(typeof(PlayerItem).GetMethod("Pickup"), typeof(SoulVessel).GetMethod("AntiDropPickup"));
            Hook hook2 = new Hook(typeof(ShopItemController).GetMethod("Interact"), typeof(SoulVessel).GetMethod("AntiDropPurchase"));
        }

        public override void Update()
        {
            base.Update();
            if (this.PickedUp)
            {
                base.sprite.SetSprite(SoulVessel.spriteIDs[this.usesLeft]);
            }
            if (GameManager.Options.CurrentLanguage != this.lastLang)
            {
                if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.RUSSIAN)
                {
                    this.SetName("Сосуд Души");
                    this.SetShortDescription("Содержит Душу");
                    this.SetLongDescription("Используйте, чтобы выпустить смертоносное заклинание.\n\nДУША - эссенция жизни в далеком королевстве Халлоунест. Она также как-то работает в Оружелье, так что ее можно использовать здесь.");
                }
                else if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ENGLISH)
                {
                    this.SetName("Soul Vessel");
                    this.SetShortDescription("Contains Soul");
                    this.SetLongDescription("Use to release a damaging spell.\n\nThe SOUL is a essence of life in a far away kingdom Hallownest. It kinda works in Gungeon too, so you can still use it there.");
                }
                this.lastLang = GameManager.Options.CurrentLanguage;
            }
        }

        public static void AntiDropPurchase(Action<ShopItemController, PlayerController> orig, ShopItemController self, PlayerController player)
        {
            bool flag = self.item is PlayerItem;
            bool switched = false;
            if (flag)
            {
                bool flag_ = player.CurrentItem != null && (player.CurrentItem is SoulVessel);
                bool flag2 = player.maxActiveItemsHeld > 1 && flag_;
                if (flag2)
                {
                    MethodInfo method = typeof(PlayerController).GetMethod("ChangeItem", BindingFlags.Instance | BindingFlags.NonPublic);
                    method.Invoke(player, new object[]
                    {
                    1
                    });
                    switched = true;
                }
                bool hasVessel = false;
                foreach (PlayerItem active in player.activeItems)
                {
                    if (active is SoulVessel)
                    {
                        hasVessel = true;
                    }
                }
                bool flag3 = hasVessel && !(self.item is SoulVessel) && flag_;
                if (flag3)
                {
                    bool flag4 = player.maxActiveItemsHeld <= player.activeItems.Count && !switched;
                    if (flag4)
                    {
                        return;
                    }
                }
                if (!(player.CurrentItem is SoulVessel) && hasVessel)
                {
                    switched = true;
                }
            }
            orig(self, player);
            if (switched)
            {
                MethodInfo method = typeof(PlayerController).GetMethod("ChangeItem", BindingFlags.Instance | BindingFlags.NonPublic);
                method.Invoke(player, new object[]
                {
                    1
                });
            }
        }

        public static void AntiDropPickup(Action<PlayerItem, PlayerController> orig, PlayerItem self, PlayerController player)
        {
            bool switched = false;
            bool flag = player.CurrentItem != null && (player.CurrentItem is SoulVessel);
            bool flag2 = player.maxActiveItemsHeld > 1 && flag;
            if (flag2)
            {
                MethodInfo method = typeof(PlayerController).GetMethod("ChangeItem", BindingFlags.Instance | BindingFlags.NonPublic);
                method.Invoke(player, new object[]
                {
                    1
                });
                switched = true;
            }
            bool hasVessel = false;
            foreach (PlayerItem active in player.activeItems)
            {
                if (active is SoulVessel)
                {
                    hasVessel = true;
                }
            }
            bool flag3 = hasVessel && !(self is SoulVessel) && flag;
            if (flag3)
            {
                bool flag4 = player.maxActiveItemsHeld <= player.activeItems.Count && !switched;
                if (flag4)
                {
                    return;
                }
            }
            if (!(player.CurrentItem is SoulVessel) && hasVessel)
            {
                switched = true;
            }
            orig(self, player);
            if (switched)
            {
                MethodInfo method = typeof(PlayerController).GetMethod("ChangeItem", BindingFlags.Instance | BindingFlags.NonPublic);
                method.Invoke(player, new object[]
                {
                    1
                });
            }
        }

        private StringTableManager.GungeonSupportedLanguages lastLang = StringTableManager.GungeonSupportedLanguages.ENGLISH;

        public override void Pickup(PlayerController player)
        {
            player.OnDealtDamage += this.OnDidDamage;
            player.OnReceivedDamage += this.OnRecievedDamage;
            base.Pickup(player);
        }

        private void OnRecievedDamage(PlayerController player)
        {
            if (player.HasPickupID(434))
            {
                while(this.usesLeft < SoulVessel.usesMax)
                {
                    this.usesLeft += 1;
                }
            }
        }

        protected override void OnPreDrop(PlayerController user)
        {
            user.OnDealtDamage -= this.OnDidDamage;
            base.OnPreDrop(user);
        }


        private void OnDidDamage(PlayerController player, float damage)
        {
            this.damageDid += damage;
            if(this.damageDid >= SoulVessel.damadeNeeded)
            {
                if(this.usesLeft < SoulVessel.usesMax)
                {
                    this.usesLeft += 1;
                }
                this.damageDid = 0f;
            }
        }

        protected override void DoEffect(PlayerController user)
        {
            Projectile proj = ((Gun)ETGMod.Databases.Items["burning_hand"]).DefaultModule.projectiles[0];
            BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(this.LastOwner.PlayerIDX);
            bool flag2 = instanceForPlayer == null;
            float z = 0;
            if (!flag2)
            {
                bool flag3 = instanceForPlayer.IsKeyboardAndMouse(false);
                Vector2 a;
                if (flag3)
                {
                    a = this.LastOwner.unadjustedAimPoint.XY() - base.sprite.WorldCenter;
                }
                else
                {
                    bool flag4 = instanceForPlayer.ActiveActions == null;
                    if (flag4)
                    {
                        return;
                    }
                    a = instanceForPlayer.ActiveActions.Aim.Vector;
                }
                a.Normalize();
                z = BraveMathCollege.Atan2Degrees(a);
            }
            GameObject obj = SpawnManager.SpawnProjectile(proj.gameObject, user.sprite.WorldCenter, Quaternion.Euler(0, 0, z));
            Projectile component = obj.GetComponent<Projectile>();
            if(component != null)
            {
                component.Owner = user;
                component.Shooter = user.specRigidbody;
                component.AppliesFire = false;
                component.FireApplyChance = 0f;
                component.baseData.damage = 20f;
                if (UnityEngine.Random.value < 0.26f)
                {
                    component.damageTypes = CoreDamageTypes.Void;
                    component.AdditionalScaleMultiplier *= 2f;
                    component.baseData.speed *= 2f;
                    component.DefaultTintColor = Color.black;
                    component.HasDefaultTint = true;
                }
                else
                {
                    component.damageTypes = CoreDamageTypes.None;
                    component.DefaultTintColor = Color.white;
                    component.HasDefaultTint = true;
                }
                PierceProjModifier component2 = component.GetComponent<PierceProjModifier>();
                if(component2 != null)
                {
                    component2.penetratesBreakables = true;
                }
            }
            this.usesLeft -= 1;
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return this.usesLeft > 0;
        }

        private static readonly float damadeNeeded = 80f;

        private float damageDid = 0f;

        private int usesLeft = 0;

        private static readonly int usesMax = 3;

        private static int[] spriteIDs;

        private static readonly string[] spritePaths = new string[]
        {
            "KnightItems/Resources/soulvessel_empty",
            "KnightItems/Resources/soulvessel_halfHalfEmpty",
            "KnightItems/Resources/soulvessel_halfEmpty",
            "KnightItems/Resources/soulvessel_full"
        };
    }
}
