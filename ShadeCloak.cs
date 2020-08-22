using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace KnightItems
{
    public class ShadeCloak : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Shade Cloak"; 
            string resourceName = "KnightItems/Resources/hollowcape";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<ShadeCloak>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Da-a-a-a-a-a-a-a-a-sh";
            string longDesc = "Improves dodgerolling, but negates dodgeroll damage.\n\nAfter contact with pure VOID, the Mothing Cloak became this.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.DodgeRollDamage, 0, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.DodgeRollSpeedMultiplier, 1.28f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.DodgeRollDistanceMultiplier, 1.31f, StatModifier.ModifyMethod.MULTIPLICATIVE);
            item.quality = PickupObject.ItemQuality.SPECIAL;
        }

        public override void Pickup(PlayerController player)
        {
            player.rollStats.additionalInvulnerabilityFrames += 60;
            player.OnRollStarted += this.OnRollStarted;
            base.Pickup(player);
        }

        private void OnRollStarted(PlayerController player, Vector2 velocity)
        {
            if (player.HasPickupID(315))
            {
                if (this.canDodgeShoot)
                {
                    Projectile proj = ((Gun)ETGMod.Databases.Items["burning_hand"]).DefaultModule.projectiles[0];
                    GameObject obj = SpawnManager.SpawnProjectile(proj.gameObject, player.sprite.WorldCenter, Quaternion.Euler(0, 0, BraveMathCollege.Atan2Degrees(-velocity)));
                    Projectile component = obj.GetComponent<Projectile>();
                    if (component != null)
                    {
                        component.Owner = player;
                        component.Shooter = player.specRigidbody;
                        component.AppliesFire = false;
                        component.FireApplyChance = 0f;
                        component.baseData.damage = 20f;
                        component.damageTypes = CoreDamageTypes.Void;
                        component.DefaultTintColor = Color.black;
                        component.HasDefaultTint = true;
                        PierceProjModifier component2 = component.GetComponent<PierceProjModifier>();
                        if (component2 != null)
                        {
                            component2.penetratesBreakables = true;
                        }
                    }
                }
                this.canDodgeShoot = !this.canDodgeShoot;
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.rollStats.additionalInvulnerabilityFrames -= 60;
            player.OnRollStarted -= this.OnRollStarted;
            return base.Drop(player);
        }

        protected override void Update()
        {
            base.Update();
            if (GameManager.Options.CurrentLanguage != this.lastLang)
            {
                if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.RUSSIAN)
                {
                    this.SetName("Теневая Накидка");
                    this.SetShortDescription("Рыво-о-о-о-о-о-о-о-о-к");
                    this.SetLongDescription("Улучшает перекаты, но предотвращает урон от перекатов.\n\nПосле соприкосновения с чистой ПУСТОТОЙ, Накидка Мотылька превратилась в это.");
                }
                else if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ENGLISH)
                {
                    this.SetName("Shade Cloak");
                    this.SetShortDescription("Da-a-a-a-a-a-a-a-a-sh");
                    this.SetLongDescription("Improves dodgerolling, but negates dodgeroll damage.\n\nAfter contact with pure VOID, the Mothing Cloak became this.");
                }
                this.lastLang = GameManager.Options.CurrentLanguage;
            }
            if (this.m_pickedUp)
            {
                this.hasSharpShadowSynergy = this.m_owner.HasPickupID(414) || this.m_owner.HasPickupID(312) || this.m_owner.HasPickupID(457);
                if(this.hasSharpShadowSynergy != this.hasSharpShadowSynergyLast)
                {
                    if(this.hasSharpShadowSynergy == true)
                    {
                        this.RemoveStat(PlayerStats.StatType.DodgeRollDamage);
                    }
                    else
                    {
                        this.AddStat(PlayerStats.StatType.DodgeRollDamage, 0, StatModifier.ModifyMethod.MULTIPLICATIVE);
                    }
                    this.m_owner.stats.RecalculateStats(this.m_owner, true, false);
                    this.hasSharpShadowSynergyLast = this.hasSharpShadowSynergy;
                }
            }
        }

        private void AddStat(PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
        {
            StatModifier statModifier = new StatModifier();
            statModifier.amount = amount;
            statModifier.statToBoost = statType;
            statModifier.modifyType = method;
            foreach (StatModifier statModifier2 in this.passiveStatModifiers)
            {
                bool flag = statModifier2.statToBoost == statType;
                if (flag)
                {
                    return;
                }
            }
            bool flag2 = this.passiveStatModifiers == null;
            if (flag2)
            {
                this.passiveStatModifiers = new StatModifier[]
                {
                    statModifier
                };
                return;
            }
            this.passiveStatModifiers = this.passiveStatModifiers.Concat(new StatModifier[]
            {
                statModifier
            }).ToArray<StatModifier>();
        }

        private void RemoveStat(PlayerStats.StatType statType)
        {
            List<StatModifier> list = new List<StatModifier>();
            for (int i = 0; i < this.passiveStatModifiers.Length; i++)
            {
                bool flag = this.passiveStatModifiers[i].statToBoost != statType;
                if (flag)
                {
                    list.Add(this.passiveStatModifiers[i]);
                }
            }
            this.passiveStatModifiers = list.ToArray();
        }

        private StringTableManager.GungeonSupportedLanguages lastLang = StringTableManager.GungeonSupportedLanguages.ENGLISH;

        private bool canDodgeShoot = true;

        private bool hasSharpShadowSynergy = false;

        private bool hasSharpShadowSynergyLast = false;
    }
}
