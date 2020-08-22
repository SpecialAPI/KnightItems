using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Gungeon;

namespace KnightItems
{
    public class SoulVesselGifter : PassiveItem
    {
        public static void Init()
        {
            string itemName = "sv_give";
            string resourceName = "KnightItems/Resources/soulvessel_empty";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<SoulVesselGifter>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "";
            string longDesc = "";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = PickupObject.ItemQuality.EXCLUDED;
        }

        public override void Pickup(PlayerController player)
        {
            this.m_pickedUpThisRun = true;
            base.Pickup(player);
            GameManager.Instance.StartCoroutine(this.Give(player));
        }

        public IEnumerator Give(PlayerController player)
        {
            yield return new WaitForSeconds(0.1f);
            EncounterTrackable.SuppressNextNotification = true;
            LootEngine.GivePrefabToPlayer((ETGMod.Databases.Items["Soul Vessel"] as PlayerItem).gameObject, player);
            player.RemovePassiveItem(this.PickupObjectId);
            yield break;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            return base.Drop(player);
        }
    }
}
