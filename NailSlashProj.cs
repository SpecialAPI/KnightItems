using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KnightItems
{
    internal class NailSlashProj : MonoBehaviour
    {
        public void Start()
        {
            this.projectile = base.GetComponent<Projectile>();
            Projectile projectile = this.projectile;
            projectile.baseData.damage = 10;
            projectile.baseData.speed = 20;
            projectile.baseData.range = int.MaxValue;
            projectile.sprite.spriteId = projectile.sprite.GetSpriteIdByName("grubberfly_strike_001");
            projectile.baseData.force = 0;
        }

        private Projectile projectile;
    }
}
