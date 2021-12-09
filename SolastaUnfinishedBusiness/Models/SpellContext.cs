using System;
using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.Models
{
    internal static class SpellContext
    {
        public static readonly Dictionary<string, Dictionary<int, List<SpellDefinition>>> classSpellList = new Dictionary<string, Dictionary<int, List<SpellDefinition>>>();
        public static readonly Dictionary<string, Dictionary<int, List<SpellDefinition>>> subclassSpellList = new Dictionary<string, Dictionary<int, List<SpellDefinition>>>();

        private static int GetLowestCasterLevelFromSpellLevel(string name, int spellLevel, bool isSubclass = false)
        {
            CasterType casterType;

            if (isSubclass)
            {
                if (!Main.Settings.SubclassCasterType.ContainsKey(name))
                {
                    return 0;
                }

                casterType = Main.Settings.SubclassCasterType[name];
            }
            else
            {
                if (!Main.Settings.ClassCasterType.ContainsKey(name))
                {
                    return 0;
                }

                casterType = Main.Settings.ClassCasterType[name];
            }

            int modifier;

            switch (casterType)
            {
                case CasterType.Full:
                    modifier = 2;
                    break;

                case CasterType.Half:
                case CasterType.HalfRoundUp:
                    modifier = 4;
                    break;

                case CasterType.OneThird:
                    modifier = 6;
                    break;

                default:
                    modifier = 0;
                    break;
            }

            var classLevel = Math.Abs(((spellLevel - 1) * modifier) + (spellLevel > 1 ? 1 : modifier / 2));

            return classLevel;
        }

        private static void RegisterSpell(string name, int level, List<SpellDefinition> spellList, bool isSubclass = false)
        {
            if (spellList != null)
            {
                var record = isSubclass ? subclassSpellList : classSpellList;

                if (!record.ContainsKey(name))
                {
                    record.Add(name, new Dictionary<int, List<SpellDefinition>>());
                }

                if (!record[name].ContainsKey(level))
                {
                    record[name].Add(level, new List<SpellDefinition>());
                }

                foreach (var spell in spellList)
                {
                    if (!record[name][level].Contains(spell))
                    {
                        record[name][level].Add(spell);
                    }
                }
            }
        }

        private static void EnumerateSpells(string name, List<FeatureUnlockByLevel> featureUnlocks, bool isSubClass = false)
        {
            foreach (var featureUnlock in featureUnlocks)
            {
                var featureDefinition = featureUnlock.FeatureDefinition;

                if (featureDefinition is FeatureDefinitionCastSpell featureDefinitionCastSpell)
                {
                    var spellListDefinition = featureDefinitionCastSpell.SpellListDefinition;

                    if (spellListDefinition != null)
                    {
                        var maxLevel = spellListDefinition.MaxSpellLevel;

                        for (var i = 0; i < maxLevel; i++)
                        {
                            var level = GetLowestCasterLevelFromSpellLevel(name, spellListDefinition.SpellsByLevel[i].Level, true);
                            var spellList = spellListDefinition.SpellsByLevel[i].Spells;

                            RegisterSpell(name, level, spellList, isSubClass);
                        }
                    }
                }

                if (featureDefinition is FeatureDefinitionMagicAffinity featureDefinitionMagicAffinity)
                {
                    var spellListDefinition = featureDefinitionMagicAffinity.ExtendedSpellList;

                    if (spellListDefinition != null)
                    {
                        var maxLevel = spellListDefinition.MaxSpellLevel;

                        for (var i = 0; i < maxLevel; i++)
                        {
                            var level = GetLowestCasterLevelFromSpellLevel(name, spellListDefinition.SpellsByLevel[i].Level, true);
                            var spellList = spellListDefinition.SpellsByLevel[i].Spells;

                            RegisterSpell(name, level, spellList, isSubClass);
                        }
                    }
                }

                if (featureDefinition is FeatureDefinitionAutoPreparedSpells featureDefinitionAutoPreparedSpells)
                {
                    var autoPreparedSpellsGroups = featureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroups;

                    foreach (var autoPreparedSpellsGroup in autoPreparedSpellsGroups)
                    {
                        var level = autoPreparedSpellsGroup.ClassLevel;
                        var spellList = autoPreparedSpellsGroup.SpellsList;

                        RegisterSpell(name, level, spellList, isSubClass);
                    }
                }

                if (featureDefinition is FeatureDefinitionBonusCantrips featureDefinitionBonusCantrips)
                {
                    var level = featureUnlock.Level;
                    var spellList = featureDefinitionBonusCantrips.BonusCantrips;

                    RegisterSpell(name, level, spellList, isSubClass);
                }
            }
        }

        internal static void Load()
        {
            foreach (var characterClassDefinition in DatabaseRepository.GetDatabase<CharacterClassDefinition>())
            {
                var className = characterClassDefinition.Name;
                var featureUnlocks = characterClassDefinition?.FeatureUnlocks;

                EnumerateSpells(className, featureUnlocks, false);
            }

            foreach (var characterSubclassDefinition in DatabaseRepository.GetDatabase<CharacterSubclassDefinition>())
            {
                var subclassName = characterSubclassDefinition.Name;
                var featureUnlocks = characterSubclassDefinition?.FeatureUnlocks;

                EnumerateSpells(subclassName, featureUnlocks, true);
            }
        }

        internal static bool IsRepertoireFromSelectedClassSubclass(RulesetSpellRepertoire rulesetSpellRepertoire)
        {
            var selectedClass = LevelUpContext.SelectedClass;
            var selectedSubclass = LevelUpContext.SelectedSubclass;

            return
                rulesetSpellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Class && rulesetSpellRepertoire.SpellCastingClass == selectedClass ||
                rulesetSpellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Subclass && rulesetSpellRepertoire.SpellCastingSubclass == selectedSubclass;
        }

        internal static bool IsSpellKnownBySelectedClassSubclass(SpellDefinition spellDefinition)
        {
            var selectedHero = LevelUpContext.SelectedHero;
            var selectedClass = LevelUpContext.SelectedClass;
            var selectedSubclass = LevelUpContext.SelectedSubclass;
            var spellRepertoire = selectedHero?.SpellRepertoires.Find(sr =>
                sr.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Class && sr.SpellCastingClass == selectedClass ||
                sr.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Subclass && sr.SpellCastingSubclass == selectedSubclass);

            return spellRepertoire?.HasKnowledgeOfSpell(spellDefinition) == true;
        }

        internal static bool IsSpellOfferedBySelectedClassSubclass(SpellDefinition spellDefinition, bool onlyCurrentLevel = false)
        {
            var classLevel = LevelUpContext.GetClassLevel();
            var className = LevelUpContext.SelectedClass?.Name;
            var subClassName = LevelUpContext.SelectedSubclass?.Name;

            if (className != null && classSpellList.ContainsKey(className))
            {
                foreach (var levelSpell in classSpellList[className])
                {
                    if (levelSpell.Key <= classLevel)
                    {
                        if (levelSpell.Value.Contains(spellDefinition))
                        {
                            return true;
                        }
                        else if (onlyCurrentLevel)
                        {
                            break;
                        }
                    }
                }
            }

            if (subClassName != null && subclassSpellList.ContainsKey(subClassName))
            {
                foreach (var levelSpell in subclassSpellList[subClassName])
                {
                    if (levelSpell.Key <= classLevel)
                    {
                        if (levelSpell.Value.Contains(spellDefinition))
                        {
                            return true;
                        }
                        else if (onlyCurrentLevel)
                        {
                            break;
                        }
                    }
                }
            }

            return false;
        }
    }
}
