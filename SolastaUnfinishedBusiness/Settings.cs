using System.Collections.Generic;
using SolastaUnfinishedBusiness.Models;
using UnityModManagerNet;

namespace SolastaUnfinishedBusiness
{
    public class Core
    {

    }

    public class Settings : UnityModManager.ModSettings
    {
        public const string GUID = "38951667573b46b0970833e90c229ca2";

        public const InputCommands.Id PLAIN_LEFT = (InputCommands.Id)22220003;
        public const InputCommands.Id PLAIN_RIGHT = (InputCommands.Id)22220004;
        public const InputCommands.Id PLAIN_UP = (InputCommands.Id)22220005;
        public const InputCommands.Id PLAIN_DOWN = (InputCommands.Id)22220006;

        public const RestActivityDefinition.ActivityCondition ActivityConditionCanLevelDown = (RestActivityDefinition.ActivityCondition)(-1002);

        public int MaxAllowedClasses { get; set; } = 2;

        public bool EnableClassWarlock { get; set; } = false;

        public bool EnableMinInOutAttributes { get; set; } = true;
        public bool EnableNonStackingExtraAttacks { get; set; } = true;

        public bool EnableGrantHolySymbol { get; set; } = true;
        public bool EnableGrantComponentPouch { get; set; } = true;
        public bool EnableGrantDruidicFocus { get; set; } = true;
        public bool EnableGrantCLothesWizard { get; set; } = false;

        public bool EnableRelearnCantrips { get; set; } = false;
        public bool EnableRelearnSpells { get; set; } = false;
        public bool EnableRelearnScribedSpells { get; set; } = false;
        public bool EnableDisplayAllKnownSpellsOnLevelUp { get; set; } = false;

        public bool EnableCantripsAtCharacterLevel { get; set; } = true;
        public bool EnableFixTwinnedLogic { get; set; } = false;

        public bool EnableSharedSpellCasting { get; set; } = true;
        public bool EnableCombinedSpellCasting { get; set; } = false;
        public bool EnableConsumeLongRestSlotFirst { get; set; } = false;

        public bool RealisticMode { get; set; } = false;

        internal Dictionary<string, CasterType> ClassCasterType { get; set; } = new Dictionary<string, CasterType>()
        {
            { "Cleric", CasterType.Full },
            { "Druid", CasterType.Full },
            { "Sorcerer", CasterType.Full },
            { "Wizard", CasterType.Full },
            { "Paladin", CasterType.Half },
            { "Ranger", CasterType.Half },
            { "ClassTinkerer", CasterType.HalfRoundUp }, // ChrisJohnDigital
        };

        internal Dictionary<string, CasterType> SubclassCasterType { get; set; } = new Dictionary<string, CasterType>()
        {
            { "MartialSpellblade", CasterType.OneThird },
            { "RoguishShadowCaster", CasterType.OneThird },
            { "RoguishConArtist", CasterType.OneThird }, // ChrisJohnDigital
            { "FighterSpellShield", CasterType.OneThird }, // ChrisJohnDigital
        };
    }
}
