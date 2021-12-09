﻿using System;
using System.Collections.Generic;
using System.Linq;
using static FeatureDefinitionCastSpell;

namespace SolastaUnfinishedBusiness.Models
{
    public enum CasterType
    {
        None,
        Full,
        Half,
        HalfRoundUp,
        OneThird
    }

    internal class SharedSpellsContext
    {
        internal class CasterLevelContext
        {
            private readonly Dictionary<CasterType, int> levels;

            public CasterLevelContext()
            {
                levels = new Dictionary<CasterType, int>
                {
                    { CasterType.None, 0 },
                    { CasterType.Full, 0 },
                    { CasterType.Half, 0 },
                    { CasterType.HalfRoundUp, 0 },
                    { CasterType.OneThird, 0 },
                };
            }

            public void IncrementCasterLevel(CasterType casterType, int increment)
            {
                levels[casterType] += increment;
            }

            public int GetCasterLevel()
            {
                var casterLevel = 0;

                // Full Casters
                casterLevel += levels[CasterType.Full];

                // Tinkerer / ...
                if (levels[CasterType.HalfRoundUp] == 1)
                {
                    casterLevel += 1;
                }
                // Half Casters
                else
                {
                    casterLevel += (int)Math.Floor(levels[CasterType.HalfRoundUp] / 2.0);
                }

                casterLevel += (int)Math.Floor(levels[CasterType.Half] / 2.0);

                // Con Artist / ...
                casterLevel += (int)Math.Floor(levels[CasterType.OneThird] / 3.0);

                return casterLevel;
            }

            public int GetSpellLevel()
            {
                var spellLevel = 0;

                // Full Casters
                spellLevel += (int)Math.Ceiling(levels[CasterType.Full] / 2.0);

                // Tinkerer / ...
                spellLevel += (int)Math.Ceiling(levels[CasterType.HalfRoundUp] / 4.0);

                // Half Casters
                if (levels[CasterType.Half] > 1)
                {
                    spellLevel += (int)Math.Ceiling(levels[CasterType.Half] / 4.0);
                }

                // Con Artist / ...
                if (levels[CasterType.OneThird] > 2)
                {
                    spellLevel += (int)Math.Ceiling(levels[CasterType.OneThird] / 6.0);
                }

                return Math.Min(9, spellLevel);
            }
        }

        internal static readonly string[] CasterTypeNames = new string[5]
        {
            "None",
            "Full",
            "Half",
            "Tinkerer",
            "One-Third"
        };

        private static CasterType GetCasterTypeForClassOrSubclass(CharacterClassDefinition characterClassDefinition, CharacterSubclassDefinition characterSubclassDefinition)
        {
            if (characterClassDefinition != null && Main.Settings.ClassCasterType.ContainsKey(characterClassDefinition.Name) && Main.Settings.ClassCasterType[characterClassDefinition.Name] != CasterType.None)
            {
                return Main.Settings.ClassCasterType[characterClassDefinition.Name];
            }

            if (characterSubclassDefinition != null && Main.Settings.SubclassCasterType.ContainsKey(characterSubclassDefinition.Name))
            {
                return Main.Settings.SubclassCasterType[characterSubclassDefinition.Name];
            }

            return CasterType.None;
        }

        internal static bool IsEnabled => Main.Settings.EnableSharedSpellCasting;

        internal static bool IsCombined => Main.Settings.EnableCombinedSpellCasting;

        internal static bool ForceLongRestSlot { get; set; }

        internal static RuleDefinitions.RestType RestType { get; set; }

        internal static RulesetCharacterHero GetHero(string name)
        {
            var gameCampaign = Gui.GameCampaign;
            GameCampaignCharacter gameCampaignCharacter = null;

            if (gameCampaign != null)
            {
                gameCampaignCharacter = gameCampaign.Party.CharactersList.Find(x => x.RulesetCharacter.Name == name);
            }

            if (gameCampaignCharacter != null)
            {
                return gameCampaignCharacter.RulesetCharacter as RulesetCharacterHero;
            }

            var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();

            if (characterBuildingService != null && characterBuildingService.HeroCharacter != null)
            {
                return characterBuildingService.HeroCharacter;
            }

            return InspectionPanelContext.SelectedHero;
        }

        internal static bool IsMulticaster(RulesetCharacterHero rulesetCharacterHero)
        {
            var repertoires = rulesetCharacterHero?.SpellRepertoires?.FindAll(sr => (sr.SpellCastingFeature.SpellCastingOrigin == CastingOrigin.Class || sr.SpellCastingFeature.SpellCastingOrigin == CastingOrigin.Subclass));

            return repertoires?.Count > 1;
        }

        internal static bool IsSharedcaster(RulesetCharacterHero rulesetCharacterHero)
        {
            var repertoires = rulesetCharacterHero?.SpellRepertoires?.FindAll(sr => (sr.SpellCastingFeature.SpellCastingOrigin == CastingOrigin.Class || sr.SpellCastingFeature.SpellCastingOrigin == CastingOrigin.Subclass) && !IsWarlock(sr.SpellCastingClass));

            return repertoires?.Count > 1;
        }

        internal static bool IsWarlock(CharacterClassDefinition characterClassDefinition)
        {
            return characterClassDefinition?.Name.Contains("ClassWarlock") == true;
        }

        internal static int GetWarlockLevel(RulesetCharacterHero rulesetCharacterHero)
        {
            var warlockLevel = 0;
            var warlock = rulesetCharacterHero?.ClassesAndLevels.Keys.ToList().Find(x => x.Name.Contains("ClassWarlock"));

            if (warlock != null)
            {
                rulesetCharacterHero.ClassesAndLevels.TryGetValue(warlock, out warlockLevel);
            }

            return warlockLevel;
        }

        internal static int GetWarlockSpellLevel(RulesetCharacterHero rulesetCharacterHero)
        {
            var warlockLevel = GetWarlockLevel(rulesetCharacterHero);

            if (warlockLevel > 0)
            {
                return WarlockCastingSlots[warlockLevel - 1].Slots.IndexOf(0);
            }

            return 0;
        }

        internal static int GetWarlockMaxSlots(RulesetCharacterHero rulesetCharacterHero)
        {
            var warlockLevel = GetWarlockLevel(rulesetCharacterHero);

            if (warlockLevel > 0)
            {
                return WarlockCastingSlots[warlockLevel - 1].Slots[0];
            }

            return 0;
        }

        internal static RulesetSpellRepertoire GetWarlockSpellRepertoire(RulesetCharacterHero rulesetCharacterHero)
        {
            return rulesetCharacterHero?.SpellRepertoires.FirstOrDefault(x => IsWarlock(x.SpellCastingClass));
        }

        internal static int GetSharedCasterLevel(RulesetCharacterHero rulesetCharacterHero)
        {
            var casterLevelContext = new CasterLevelContext();

            if (rulesetCharacterHero?.ClassesAndLevels != null)
            {
                foreach (var classAndLevel in rulesetCharacterHero.ClassesAndLevels)
                {
                    var currentCharacterClassDefinition = classAndLevel.Key;

                    rulesetCharacterHero.ClassesAndSubclasses.TryGetValue(currentCharacterClassDefinition, out var currentCharacterSubclassDefinition);

                    var casterType = GetCasterTypeForClassOrSubclass(currentCharacterClassDefinition, currentCharacterSubclassDefinition);

                    casterLevelContext.IncrementCasterLevel(casterType, classAndLevel.Value);
                }
            }

            return casterLevelContext.GetCasterLevel();
        }

        internal static int GetSharedSpellLevel(RulesetCharacterHero rulesetCharacterHero)
        {
            if (IsSharedcaster(rulesetCharacterHero))
            {
                var sharedCasterLevel = GetSharedCasterLevel(rulesetCharacterHero);

                if (sharedCasterLevel > 0)
                {
                    return FullCastingSlots[sharedCasterLevel - 1].Slots.IndexOf(0);
                }

                return 0;
            }
            else
            {
                var otherCasterRepertoire = rulesetCharacterHero.SpellRepertoires.Find(sr =>
                    (sr.SpellCastingFeature.SpellCastingOrigin == CastingOrigin.Class || sr.SpellCastingFeature.SpellCastingOrigin == CastingOrigin.Subclass) && !IsWarlock(sr.SpellCastingClass));

                return GetClassSpellLevel(rulesetCharacterHero, otherCasterRepertoire?.SpellCastingClass, otherCasterRepertoire?.SpellCastingSubclass);
            }
        }

        internal static int GetClassCasterLevel(
            RulesetCharacterHero rulesetCharacterHero,
            CharacterClassDefinition filterCharacterClassDefinition,
            CharacterSubclassDefinition filterCharacterSublassDefinition = null)
        {
            int classCasterLevel;

            var casterLevelContext = new CasterLevelContext();

            if (rulesetCharacterHero?.ClassesAndLevels != null)
            {
                foreach (var classAndLevel in rulesetCharacterHero.ClassesAndLevels)
                {
                    var currentCharacterClassDefinition = classAndLevel.Key;

                    rulesetCharacterHero.ClassesAndSubclasses.TryGetValue(currentCharacterClassDefinition, out var currentCharacterSubclassDefinition);

                    if (filterCharacterClassDefinition == currentCharacterClassDefinition || filterCharacterSublassDefinition != null && filterCharacterSublassDefinition == currentCharacterSubclassDefinition)
                    {
                        var casterType = GetCasterTypeForClassOrSubclass(currentCharacterClassDefinition, currentCharacterSubclassDefinition);

                        casterLevelContext.IncrementCasterLevel(casterType, classAndLevel.Value);
                    }
                }
            }
            classCasterLevel = casterLevelContext.GetCasterLevel();

            return classCasterLevel;
        }

        internal static int GetClassSpellLevel(
            RulesetCharacterHero rulesetCharacterHero,
            CharacterClassDefinition filterCharacterClassDefinition,
            CharacterSubclassDefinition filterCharacterSublassDefinition = null)
        {
            int classCasterLevel;

            if (IsWarlock(filterCharacterClassDefinition))
            {
                classCasterLevel = GetWarlockSpellLevel(rulesetCharacterHero);
            }
            else
            {
                var casterLevelContext = new CasterLevelContext();

                if (rulesetCharacterHero?.ClassesAndLevels != null)
                {
                    foreach (var classAndLevel in rulesetCharacterHero.ClassesAndLevels)
                    {
                        var currentCharacterClassDefinition = classAndLevel.Key;

                        rulesetCharacterHero.ClassesAndSubclasses.TryGetValue(currentCharacterClassDefinition, out var currentCharacterSubclassDefinition);

                        if (filterCharacterClassDefinition == currentCharacterClassDefinition || filterCharacterSublassDefinition != null && filterCharacterSublassDefinition == currentCharacterSubclassDefinition)
                        {
                            var casterType = GetCasterTypeForClassOrSubclass(currentCharacterClassDefinition, currentCharacterSubclassDefinition);

                            casterLevelContext.IncrementCasterLevel(casterType, classAndLevel.Value);
                        }
                    }
                }

                classCasterLevel = casterLevelContext.GetSpellLevel();
            }

            return classCasterLevel;
        }

        internal static int GetCombinedSpellLevel(RulesetCharacterHero rulesetCharacterHero)
        {
            return Math.Max(GetWarlockSpellLevel(rulesetCharacterHero), GetSharedSpellLevel(rulesetCharacterHero));
        }

        // allows casters to use slots above their caster level if multiclassed
        internal static void Load()
        {
            var SpellListDefinition = DatabaseRepository.GetDatabase<SpellListDefinition>();

            foreach (var spellsByLevel in SpellListDefinition.Select(x => x.SpellsByLevel))
            {
                while (spellsByLevel.Count < 9)
                {
                    spellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet() { Level = spellsByLevel.Count, Spells = new List<SpellDefinition>() { } });
                }
            }
        }

        internal static readonly List<SlotsByLevelDuplet> FullCastingSlots = new List<SlotsByLevelDuplet>()
        {
            new SlotsByLevelDuplet() { Slots = new List<int> {2,0,0,0,0,0,0,0,0,0}, Level = 01 },
            new SlotsByLevelDuplet() { Slots = new List<int> {3,0,0,0,0,0,0,0,0,0}, Level = 02 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,2,0,0,0,0,0,0,0,0}, Level = 03 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,0,0,0,0,0,0,0,0}, Level = 04 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,2,0,0,0,0,0,0,0}, Level = 05 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,0,0,0,0,0,0,0}, Level = 06 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,1,0,0,0,0,0,0}, Level = 07 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,2,0,0,0,0,0,0}, Level = 08 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,1,0,0,0,0,0}, Level = 09 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,0,0,0,0,0}, Level = 10 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,0,0,0,0}, Level = 11 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,0,0,0,0}, Level = 12 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,0,0,0}, Level = 13 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,0,0,0}, Level = 14 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,1,0,0}, Level = 15 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,1,0,0}, Level = 16 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,2,1,1,1,1,0}, Level = 17 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,3,1,1,1,1,0}, Level = 18 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,3,2,1,1,1,0}, Level = 19 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,3,3,3,3,2,2,1,1,0}, Level = 20 },
        };

        internal static readonly List<SlotsByLevelDuplet> WarlockCastingSlots = new List<SlotsByLevelDuplet>()
        {
            new SlotsByLevelDuplet() { Slots = new List<int> {1,0,0,0,0,0,0,0,0,0}, Level = 01 },
            new SlotsByLevelDuplet() { Slots = new List<int> {2,0,0,0,0,0,0,0,0,0}, Level = 02 },
            new SlotsByLevelDuplet() { Slots = new List<int> {2,2,0,0,0,0,0,0,0,0}, Level = 03 },
            new SlotsByLevelDuplet() { Slots = new List<int> {2,2,0,0,0,0,0,0,0,0}, Level = 04 },
            new SlotsByLevelDuplet() { Slots = new List<int> {2,2,2,0,0,0,0,0,0,0}, Level = 05 },
            new SlotsByLevelDuplet() { Slots = new List<int> {2,2,2,0,0,0,0,0,0,0}, Level = 06 },
            new SlotsByLevelDuplet() { Slots = new List<int> {2,2,2,2,0,0,0,0,0,0}, Level = 07 },
            new SlotsByLevelDuplet() { Slots = new List<int> {2,2,2,2,0,0,0,0,0,0}, Level = 08 },
            new SlotsByLevelDuplet() { Slots = new List<int> {2,2,2,2,2,0,0,0,0,0}, Level = 09 },
            new SlotsByLevelDuplet() { Slots = new List<int> {2,2,2,2,2,0,0,0,0,0}, Level = 10 },
            new SlotsByLevelDuplet() { Slots = new List<int> {3,3,3,3,3,0,0,0,0,0}, Level = 11 },
            new SlotsByLevelDuplet() { Slots = new List<int> {3,3,3,3,3,0,0,0,0,0}, Level = 12 },
            new SlotsByLevelDuplet() { Slots = new List<int> {3,3,3,3,3,0,0,0,0,0}, Level = 13 },
            new SlotsByLevelDuplet() { Slots = new List<int> {3,3,3,3,3,0,0,0,0,0}, Level = 14 },
            new SlotsByLevelDuplet() { Slots = new List<int> {3,3,3,3,3,0,0,0,0,0}, Level = 15 },
            new SlotsByLevelDuplet() { Slots = new List<int> {3,3,3,3,3,0,0,0,0,0}, Level = 16 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,4,4,4,4,0,0,0,0,0}, Level = 17 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,4,4,4,4,0,0,0,0,0}, Level = 18 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,4,4,4,4,0,0,0,0,0}, Level = 19 },
            new SlotsByLevelDuplet() { Slots = new List<int> {4,4,4,4,4,0,0,0,0,0}, Level = 20 },
        };
    }
}
