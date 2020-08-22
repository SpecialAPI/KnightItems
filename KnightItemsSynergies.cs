using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KnightItems
{
    public class TrulyVengefulSynergy : AdvancedSynergyEntry
    {
        public TrulyVengefulSynergy()
        {
            this.NameKey = "Truly Vengeful";
            this.MandatoryItemIDs = new List<int> {
                ETGMod.Databases.Items["Soul Vessel"].PickupObjectId, 
                434
            };
            this.IgnoreLichEyeBullets = true;
            this.statModifiers = new List<StatModifier>(0);
            this.bonusSynergies = new List<CustomSynergyType>();
        }
    }
    public class SoulCloakSynergy : AdvancedSynergyEntry
    {
        public SoulCloakSynergy()
        {
            this.NameKey = "Soul Cloak";
            this.MandatoryItemIDs = new List<int> {
                ETGMod.Databases.Items["Shade Cloak"].PickupObjectId,
                315
            };
            this.IgnoreLichEyeBullets = true;
            this.statModifiers = new List<StatModifier>(0);
            this.bonusSynergies = new List<CustomSynergyType>();
        }
    }
    public class VerySharpShadowSynergy : AdvancedSynergyEntry
    {
        public VerySharpShadowSynergy()
        {
            this.NameKey = "Very Sharp Shadow";
            this.MandatoryItemIDs = new List<int> {
                ETGMod.Databases.Items["Shade Cloak"].PickupObjectId,
            };
            this.OptionalItemIDs = new List<int>
            {
                457,
                414,
                312
            };
            this.IgnoreLichEyeBullets = true;
            this.statModifiers = new List<StatModifier>(0);
            this.bonusSynergies = new List<CustomSynergyType>();
        }
    }

    public class TwoBladesSynergy : AdvancedSynergyEntry
    {
        public TwoBladesSynergy()
        {
            this.NameKey = "The Two Blades";
            this.MandatoryGunIDs = new List<int> {
                ETGMod.Databases.Items["nail"].PickupObjectId,
                417
            };
            this.IgnoreLichEyeBullets = false;
            this.statModifiers = new List<StatModifier>(0);
            this.bonusSynergies = new List<CustomSynergyType>();
        }
    }
}
