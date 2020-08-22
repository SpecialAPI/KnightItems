using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace KnightItems
{
    public class EmptySoul : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Void Heart";
            string resourceName = "KnightItems/Resources/empty_soul";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<EmptySoul>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "...";
            string longDesc = "An emptiness that was hidden within, now unconstrained. Unifies the void under the bearer's will.\n\nThis charm is a part of its bearer and can not be unequipped.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Health, 2, StatModifier.ModifyMethod.ADDITIVE);
            item.CanBeDropped = false;
            item.quality = PickupObject.ItemQuality.SPECIAL;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            if(player.healthHaver != null)
            {
                player.healthHaver.ModifyDamage += this.DoubleDamage;
                player.healthHaver.ModifyHealing += this.HealIfNotFull;
            }
        }

        private void DoubleDamage(HealthHaver hh, HealthHaver.ModifyDamageEventArgs args)
        {
            if(args == EventArgs.Empty)
            {
                return;
            }
            args.ModifiedDamage *= 2;
        }

        private void HealIfNotFull(HealthHaver hh, HealthHaver.ModifyHealingEventArgs args)
        {
            if (args == EventArgs.Empty)
            {
                return;
            }
            //if(args.ModifiedHealing % 1 != 0)
            //{
            //    args.ModifiedHealing += 0.5f;
            //}
            args.ModifiedHealing *= 2;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            if(player.healthHaver != null)
            {
                player.healthHaver.ModifyDamage -= this.DoubleDamage;
                player.healthHaver.ModifyHealing -= this.HealIfNotFull;
            }
            return base.Drop(player);
        }

        protected override void Update()
        {
            base.Update();
            if (GameManager.Options.CurrentLanguage != this.lastLang)
            {
                if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.RUSSIAN)
                {
                    this.SetName("Сердце Пустоты");
                    this.SetShortDescription("...");
                    this.SetLongDescription("Пустота, что была спрятана внутри, ныне свободна. Объединяет Пустоту волею носителя.\n\nЭтот амулет является частью носителя и не может быть снят.");
                }
                else if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ENGLISH)
                {
                    this.SetName("Void Heart");
                    this.SetShortDescription("...");
                    this.SetLongDescription("An emptiness that was hidden within, now unconstrained. Unifies the void under the bearer's will.\n\nThis charm is a part of its bearer and can not be unequipped.");
                }
                this.lastLang = GameManager.Options.CurrentLanguage;
            }
        }

        private StringTableManager.GungeonSupportedLanguages lastLang = StringTableManager.GungeonSupportedLanguages.ENGLISH;
    }
}
