using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KnightItems
{
    class SynergyTranslationUpdateBehaviour : MonoBehaviour
    {
        public void Update()
        {
            if (GameManager.Options.CurrentLanguage != this.lastLang)
            {
                if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.RUSSIAN)
                {
                    KnightItemsModule.trulyVengefulSynergy.NameKey = "Воистину Мстительный";
                    KnightItemsModule.soulCloakSynergy.NameKey = "Накидка Души";
                    KnightItemsModule.verySharpShadowSynergy.NameKey = "Очень Острая Тень";
                    KnightItemsModule.twoBladesSynergy.NameKey = "Два Клинка";
                }
                else if (GameManager.Options.CurrentLanguage == StringTableManager.GungeonSupportedLanguages.ENGLISH)
                {
                    KnightItemsModule.trulyVengefulSynergy.NameKey = "Truly Vengeful";
                    KnightItemsModule.soulCloakSynergy.NameKey = "Soul Cloak";
                    KnightItemsModule.verySharpShadowSynergy.NameKey = "Very Sharp Shadow";
                    KnightItemsModule.twoBladesSynergy.NameKey = "The Two Blades";
                }
                this.lastLang = GameManager.Options.CurrentLanguage;
            }
        }

        private StringTableManager.GungeonSupportedLanguages lastLang = StringTableManager.GungeonSupportedLanguages.ENGLISH;
    }
}
