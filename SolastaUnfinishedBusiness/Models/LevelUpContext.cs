using System.Collections.Generic;
using System.Linq;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaUnfinishedBusiness.Models
{
    internal static class LevelUpContext
    {
        private static readonly Dictionary<string, Dictionary<string, string>> featuresToReplace = new Dictionary<string, Dictionary<string, string>>
        {
            { "Barbarian", new Dictionary<string, string> {
                { "ProficiencyBarbarianArmor", "BarbarianArmorProficiencyMulticlass"} } },

            { "Fighter", new Dictionary<string, string> {
                { "ProficiencyFighterArmor", "FighterArmorProficiencyMulticlass"} } },

            { "Paladin", new Dictionary<string, string> {
                 { "ProficiencyPaladinArmor", "PaladinArmorProficiencyMulticlass"} } },

            { "Ranger", new Dictionary<string, string> {
                { "PointPoolRangerSkillPoints", "PointPoolRangerSkillPointsMulticlass"} } },

            { "Rogue", new Dictionary<string, string> {
                { "PointPoolRogueSkillPoints", "PointPoolRogueSkillPointsMulticlass"} } },
        };

        private static readonly Dictionary<string, List<string>> featuresToExclude = new Dictionary<string, List<string>>
        {
            { "Barbarian", new List<string> {
                "PointPoolBarbarianrSkillPoints",
                "ProficiencyBarbarianSavingThrow" } },

            { "Cleric", new List<string> {
                "ProficiencyClericWeapon",
                "PointPoolClericSkillPoints",
                "ProficiencyClericSavingThrow" } },

            { "Druid", new List<string> {
                "PointPoolDruidSkillPoints",
                "ProficiencyDruidSavingThrow" } },

            { "Fighter", new List<string> {
                "PointPoolFighterSkillPoints",
                "ProficiencyFighterSavingThrow" } },

            { "Paladin", new List<string> {
                "PointPoolPaladinSkillPoints",
                "ProficiencyPaladinSavingThrow" } },

            { "Ranger", new List<string> {
                "ProficiencyRangerSavingThrow" } },

            { "Rogue", new List<string> {
                "ProficiencyRogueWeapon",
                "ProficiencyRogueSavingThrow" } },

            { "Sorcerer", new List<string> {
                "ProficiencySorcererWeapon",
                "ProficiencySorcererArmor",
                "PointPoolSorcererSkillPoints",
                "ProficiencySorcererSavingThrow"} },

            { "Wizard", new List<string> {
                "ProficiencyWizardWeapon",
                "ProficiencyWizardArmor",
                "PointPoolWizardSkillPoints",
                "ProficiencyWizardSavingThrow"} },

            // CJD's classes

            { "ClassTinkerer", new List<string> {
                "ProficiencyWeaponTinkerer",
                "PointPoolTinkererSkillPoints",
                "ProficiencyTinkererSavingThrow"} },

            // Zappastuff's classes

            { "ClassWarlock", new List<string> {
                "ClassWarlockWeaponProficiency",
                "ClassWarlockSkillProficiency",
                "ClassWarlockSavingThrowProficiency" } },
        };

        private static readonly Dictionary<string, string> extraAttacksToExclude = new Dictionary<string, string>
        {
            { "Barbarian", "AttributeModifierBarbarianExtraAttack" },
            { "Fighter", "AttributeModifierFighterExtraAttack" },
            { "Paladin", "AttributeModifierPaladinExtraAttack" },
            { "Ranger", "AttributeModifierRangerExtraAttack" },
        };

        private static bool levelingUp = false;
        private static bool requiresDeity = false;
        private static bool requiresHolySymbol = false;
        private static bool requiresClothesWizard = false;
        private static bool requiresComponentPouch = false;
        private static bool requiresDruidicFocus = false;
        private static bool requiresSpellbook = false;
        private static bool hasHolySymbolGranted = false;
        private static bool hasComponentPouchGranted = false;
        private static bool hasDruidicFocusGranted = false;
        private static bool hasClothesWizardGranted = false;
        private static bool hasSpellbookGranted = false;
        private static RulesetCharacterHero selectedHero = null;
        private static CharacterClassDefinition selectedClass = null;
        private static CharacterSubclassDefinition selectedSubclass = null;
        private static readonly List<RulesetItemSpellbook> rulesetItemSpellbooks = new List<RulesetItemSpellbook>();

        internal static RulesetCharacterHero SelectedHero
        {
            get => selectedHero;
            set
            {
                selectedHero = value;
                selectedClass = null;
                selectedSubclass = null;
                levelingUp = value != null;
                requiresDeity = false;
                requiresHolySymbol = false;
                requiresClothesWizard = false;
                requiresComponentPouch = false;
                requiresDruidicFocus = false;
                requiresSpellbook = false;
                hasHolySymbolGranted = false;
                hasComponentPouchGranted = false;
                hasDruidicFocusGranted = false;
                hasClothesWizardGranted = false;
                hasSpellbookGranted = false;
                rulesetItemSpellbooks.Clear();
                selectedHero?.CharacterInventory?.BrowseAllCarriedItems<RulesetItemSpellbook>(rulesetItemSpellbooks);
            }
        }

        internal static CharacterClassDefinition SelectedClass
        {
            get => selectedClass;
            set
            {
                selectedClass = value;
                selectedSubclass = null;

                if (selectedClass != null)
                {
                    selectedHero.ClassesAndSubclasses.TryGetValue(selectedClass, out selectedSubclass);

                    DatabaseRepository.GetDatabase<CharacterClassDefinition>().TryGetElement("ClassTinkerer", out var ChrisTinkerer);

                    var classesAndLevels = selectedHero.ClassesAndLevels;
                    var hasChrisTinkerer = ChrisTinkerer != null && selectedHero.ClassesAndLevels.ContainsKey(ChrisTinkerer);

                    requiresDeity =
                        (selectedClass == Cleric && !classesAndLevels.ContainsKey(Cleric)) || (selectedClass == Paladin && selectedHero.DeityDefinition == null);

                    requiresHolySymbol =
                        !(classesAndLevels.ContainsKey(Cleric) || classesAndLevels.ContainsKey(Paladin)) && (selectedClass == Cleric || selectedClass == Paladin);

                    requiresClothesWizard =
                        !classesAndLevels.ContainsKey(Wizard) && selectedClass == Wizard;

                    requiresComponentPouch =
                        (
                            selectedClass == Ranger ||
                            selectedClass == Sorcerer ||
                            selectedClass == Wizard ||
                            selectedClass == ChrisTinkerer
                        ) &&
                        !(
                            classesAndLevels.ContainsKey(Ranger) ||
                            classesAndLevels.ContainsKey(Sorcerer) ||
                            classesAndLevels.ContainsKey(Wizard) ||
                            hasChrisTinkerer
                        );

                    requiresDruidicFocus =
                        (
                            selectedClass == Druid
                        ) &&
                        !(
                            classesAndLevels.ContainsKey(Druid)
                        );

                    requiresSpellbook =
                        !classesAndLevels.ContainsKey(Wizard) && selectedClass == Wizard;
                }
            }
        }

        internal static CharacterSubclassDefinition SelectedSubclass { get => selectedSubclass; set => selectedSubclass = value; }

        internal static int SelectedHeroLevel
        {
            get
            {
                var heroLevel = 0;

                if (selectedHero != null)
                {
                    heroLevel = selectedHero.ClassesHistory.Count;
                }

                return heroLevel;
            }
        }

        internal static int SelectedClassLevel
        {
            get
            {
                var classLevel = 1;

                if (selectedHero != null && selectedClass != null)
                {
                    selectedHero.ClassesAndLevels.TryGetValue(selectedClass, out classLevel);
                }

                return classLevel;
            }
        }

        internal static bool DisplayingClassPanel { get; set; }

        internal static bool LevelingUp => levelingUp;

        internal static bool RequiresDeity => requiresDeity;

        internal static bool IsMulticlass => selectedHero?.ClassesAndLevels?.Count > 1 || selectedHero?.ClassesAndLevels.Count > 0 && selectedHero?.ClassesAndLevels.ContainsKey(selectedClass) != true;

        internal static List<FeatureUnlockByLevel> SelectedClassFilteredFeaturesUnlocks(List<FeatureUnlockByLevel> featureUnlockByLevels)
        {
            var filteredFeatureUnlockByLevels = new List<FeatureUnlockByLevel>();

            if (LevelingUp)
            {
                var firstClassName = selectedHero.ClassesHistory[0].Name;
                var selectedClassName = selectedClass.Name;

                featuresToReplace.TryGetValue(selectedClassName, out var featureNamesToReplace);
                featuresToExclude.TryGetValue(selectedClassName, out var featureNamesToExclude);
                extraAttacksToExclude.TryGetValue(selectedClassName, out var extraAttackNameToExclude);

                foreach (var featureUnlock in featureUnlockByLevels)
                {
                    var AttacksNumberAttribute = selectedHero.GetAttribute("AttacksNumber", true);
                    var featureDefinitionName = featureUnlock.FeatureDefinition.Name;
                    var foundFeatureToExclude = false;
                    var foundFeatureToReplace = false;
                    var foundExtraAttackToExclude = false;

                    if (firstClassName != selectedClassName)
                    {
                        // replace proficiencies that need to be
                        if (featureNamesToReplace != null)
                        {
                            foreach (var featureNameToReplace in featureNamesToReplace)
                            {
                                if (featureNameToReplace.Key == featureUnlock.FeatureDefinition.Name)
                                {
                                    var newFeatureDefinition = DatabaseRepository.GetDatabase<FeatureDefinition>().GetElement(featureNameToReplace.Value);

                                    filteredFeatureUnlockByLevels.Add(new FeatureUnlockByLevel(newFeatureDefinition, featureUnlock.Level));
                                    foundFeatureToReplace = true;
                                }
                            }
                        }

                        // check if proficiencies should be excluded
                        if (featureNamesToExclude != null)
                        {
                            foundFeatureToExclude = featureNamesToExclude.Exists(x => x == featureDefinitionName);
                        }

                    }

                    // check if extra attacks should be excluded
                    if (Main.Settings.EnableNonStackingExtraAttacks)
                    {

                        foundExtraAttackToExclude = extraAttackNameToExclude == featureDefinitionName && AttacksNumberAttribute?.ActiveModifiers.Count > 0
                            && !(selectedClass.Name == "Fighter" && (SelectedClassLevel == 11 || SelectedClassLevel == 20));
                    }

                    // only add if not supposed to be excluded
                    if (!foundFeatureToReplace && !foundFeatureToExclude && !foundExtraAttackToExclude)
                    {
                        filteredFeatureUnlockByLevels.Add(featureUnlock);
                    }
                }

                return filteredFeatureUnlockByLevels;
            }
            else
            {
                return featureUnlockByLevels;
            }
        }

        //
        // need to grant some additional items depending on the new class
        //

        internal static void GrantItemsIfRequired()
        {
            if (Main.Settings.EnableGrantHolySymbol)
            {
                GrantHolySymbol();
            }

            if (Main.Settings.EnableGrantCLothesWizard)
            {
                GrantClothesWizard();
            }

            if (Main.Settings.EnableGrantComponentPouch)
            {
                GrantComponentPouch();
            }

            if (Main.Settings.EnableGrantDruidicFocus)
            {
                GrantDruidicFocus();
            }

            GrantSpellbook();
        }

        internal static void UngrantItemsIfRequired()
        {
            UngrantHolySymbol();
            UngrantClothesWizard();
            UngrantComponentPouch();
            UngrantDruidicFocus();
            UngrantSpellbook();
        }

        internal static void GrantHolySymbol()
        {
            if (requiresHolySymbol && !hasHolySymbolGranted)
            {
                var holySymbolAmulet = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.HolySymbolAmulet);

                selectedHero.GrantItem(holySymbolAmulet, true);
                hasHolySymbolGranted = true;
            }
        }

        internal static void UngrantHolySymbol()
        {
            if (selectedHero != null && hasHolySymbolGranted)
            {
                var holySymbolAmulet = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.HolySymbolAmulet);

                selectedHero.LoseItem(holySymbolAmulet);
                hasHolySymbolGranted = false;
            }
        }

        internal static void GrantClothesWizard()
        {
            if (requiresClothesWizard && !hasClothesWizardGranted)
            {
                var clothesWizard = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.ClothesWizard);

                selectedHero.GrantItem(clothesWizard, false);
                hasClothesWizardGranted = true;
            }
        }

        internal static void UngrantClothesWizard()
        {
            if (selectedHero != null && hasClothesWizardGranted)
            {
                var clothesWizard = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.ClothesWizard);

                selectedHero.LoseItem(clothesWizard);
                hasClothesWizardGranted = false;
            }
        }

        internal static void GrantComponentPouch()
        {
            if (requiresComponentPouch && !hasComponentPouchGranted)
            {
                var componentPouch = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.ComponentPouch);

                selectedHero.GrantItem(componentPouch, true);
                hasComponentPouchGranted = true;
            }
        }

        internal static void UngrantComponentPouch()
        {
            if (selectedHero != null && hasComponentPouchGranted)
            {
                var componentPouch = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.ComponentPouch);

                selectedHero.LoseItem(componentPouch);
                hasComponentPouchGranted = false;
            }
        }

        internal static void GrantDruidicFocus()
        {
            if (requiresDruidicFocus && !hasDruidicFocusGranted)
            {
                var druidicFocus = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.DruidicFocus);

                selectedHero.GrantItem(druidicFocus, true);
                hasDruidicFocusGranted = true;
            }
        }

        internal static void UngrantDruidicFocus()
        {
            if (selectedHero != null && hasDruidicFocusGranted)
            {
                var druidicFocus = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.DruidicFocus);

                selectedHero.LoseItem(druidicFocus);
                hasDruidicFocusGranted = false;
            }
        }

        internal static void GrantSpellbook()
        {
            if (requiresSpellbook && !hasSpellbookGranted)
            {
                var spellbook = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.Spellbook);

                selectedHero.GrantItem(spellbook, false);
                hasSpellbookGranted = true;
            }
        }

        internal static void UngrantSpellbook()
        {
            if (selectedHero != null && hasSpellbookGranted)
            {
                var spellbook = new RulesetItemSpellbook(SolastaModApi.DatabaseHelper.ItemDefinitions.Spellbook);

                selectedHero.LoseItem(spellbook);
                hasSpellbookGranted = false;
            }
        }

        // used on a transpiler from CharacterStageClassSelectionPanel.FillClassFeatures and CharacterStageClassSelectionPanel.RefreshCharacter
        public static int GetClassLevel(RulesetCharacterHero hero = null)
        {
            return selectedHero == null || selectedClass == null || !selectedHero.ClassesAndLevels.ContainsKey(selectedClass) ? 1 : selectedHero.ClassesAndLevels[selectedClass];
        }

        // used on a transpiler from CharacterStageLevelGainsPanel.RefreshSpellcastingFeatures
        public static List<RulesetSpellRepertoire> SpellRepertoires(RulesetCharacter rulesetCharacter)
        {
            if (levelingUp && IsMulticlass)
            {
                var result = new List<RulesetSpellRepertoire>();

                foreach (var spellRepertoire in rulesetCharacter.SpellRepertoires.Where(x => SpellContext.IsRepertoireFromSelectedClassSubclass(x)))
                {
                    result.Add(spellRepertoire);
                }

                return result;
            }

            return rulesetCharacter.SpellRepertoires;
        }

        // used on a transpiler from CharacterStageLevelGainsPanel.EnterStage
        public static void GetLastAssignedClassAndLevel(ICharacterBuildingService characterBuildingService, out CharacterClassDefinition lastClassDefinition, out int level)
        {
            if (levelingUp)
            {
                GrantItemsIfRequired();
                DisplayingClassPanel = false;
                lastClassDefinition = selectedClass;
                level = selectedHero.ClassesHistory.Count;
            }
            else
            {
                lastClassDefinition = null;
                level = 0;

                if (characterBuildingService.HeroCharacter.ClassesHistory.Count > 0)
                {
                    lastClassDefinition = characterBuildingService.HeroCharacter.ClassesHistory[characterBuildingService.HeroCharacter.ClassesHistory.Count - 1];
                    level = characterBuildingService.HeroCharacter.ClassesAndLevels[lastClassDefinition];
                }
            }
        }
    }
}
