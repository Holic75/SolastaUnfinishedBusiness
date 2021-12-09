using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class CharacterBuildingManagerPatcher
    {
        // captures the desired class and ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "AssignClassLevel")]
        internal static class CharacterBuildingManagerAssignClassLevel
        {
            internal static bool Prefix(CharacterClassDefinition classDefinition)
            {
                if (Models.LevelUpContext.LevelingUp && Models.LevelUpContext.DisplayingClassPanel)
                {
                    Models.LevelUpContext.SelectedClass = classDefinition;
                }

                return !(Models.LevelUpContext.LevelingUp && Models.LevelUpContext.DisplayingClassPanel);
            }
        }

        // ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "AssignEquipment")]
        internal static class CharacterBuildingManagerAssignEquipment
        {
            internal static bool Prefix()
            {
                return !(Models.LevelUpContext.LevelingUp && Models.LevelUpContext.DisplayingClassPanel);
            }
        }

        // captures the desired sub class
        [HarmonyPatch(typeof(CharacterBuildingManager), "AssignSubclass")]
        internal static class CharacterBuildingManagerAssignSubclass
        {
            internal static void Prefix(CharacterSubclassDefinition subclassDefinition)
            {
                Models.LevelUpContext.SelectedSubclass = subclassDefinition;
            }
        }

        // ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "BuildWieldedConfigurations")]
        internal static class CharacterBuildingManagerBuildWieldedConfigurations
        {
            internal static bool Prefix()
            {
                return !(Models.LevelUpContext.LevelingUp && Models.LevelUpContext.DisplayingClassPanel);
            }
        }

        // ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "ClearWieldedConfigurations")]
        internal static class CharacterBuildingManagerClearWieldedConfigurations
        {
            internal static bool Prefix()
            {
                return !(Models.LevelUpContext.LevelingUp && Models.LevelUpContext.DisplayingClassPanel);
            }
        }

        // ensures the level up process only presents / offers spells based on all different mod settings
        [HarmonyPatch(typeof(CharacterBuildingManager), "EnumerateKnownAndAcquiredSpells")]
        internal static class CharacterBuildingManagerEnumerateKnownAndAcquiredSpells
        {
            internal static bool Prefix(CharacterBuildingManager __instance, string tagToIgnore, ref List<SpellDefinition> __result)
            {
                if (Models.LevelUpContext.LevelingUp && Models.LevelUpContext.IsMulticlass)
                {
                    var spellDefinitionList = new List<SpellDefinition>();

                    __instance.matchingFeatures.Clear();

                    foreach (var spellRepertoire in __instance.HeroCharacter.SpellRepertoires)
                    {
                        var isRepertoireFromSelectedClassSubclass = Models.SpellContext.IsRepertoireFromSelectedClassSubclass(spellRepertoire);

                        // PATCH: don't allow cantrips to be re-learned
                        foreach (var spell in spellRepertoire.KnownCantrips)
                        {
                            if (!spellDefinitionList.Contains(spell) &&
                                (
                                    isRepertoireFromSelectedClassSubclass ||
                                    !Main.Settings.EnableRelearnCantrips && Models.SpellContext.IsSpellOfferedBySelectedClassSubclass(spell)
                                ))
                            {
                                spellDefinitionList.Add(spell);
                            }
                        }

                        // PATCH: don't allow spells to be re-learned
                        if (spellRepertoire.SpellCastingFeature.SpellKnowledge == RuleDefinitions.SpellKnowledge.WholeList)
                        {
                            var classSpellLevel = Models.SharedSpellsContext.GetClassSpellLevel(Models.LevelUpContext.SelectedHero, spellRepertoire.SpellCastingClass, spellRepertoire.SpellCastingSubclass);

                            for (var spellLevel = 1; spellLevel <= classSpellLevel; spellLevel++)
                            {
                                foreach (var spell in spellRepertoire.SpellCastingFeature.SpellListDefinition.GetSpellsOfLevel(spellLevel))
                                {
                                    if (!spellDefinitionList.Contains(spell) &&
                                        (
                                            isRepertoireFromSelectedClassSubclass ||
                                            !Main.Settings.EnableRelearnSpells && Models.SpellContext.IsSpellOfferedBySelectedClassSubclass(spell)
                                        ))
                                    {
                                        spellDefinitionList.Add(spell);
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var spell in spellRepertoire.KnownSpells)
                            {
                                if (!spellDefinitionList.Contains(spell) &&
                                    (
                                        isRepertoireFromSelectedClassSubclass ||
                                        !Main.Settings.EnableRelearnSpells && Models.SpellContext.IsSpellOfferedBySelectedClassSubclass(spell)
                                    ))
                                {
                                    spellDefinitionList.Add(spell);
                                }
                            }
                            foreach (var spell in spellRepertoire.PreparedSpells)
                            {
                                if (!spellDefinitionList.Contains(spell) &&
                                    (
                                        isRepertoireFromSelectedClassSubclass ||
                                        !Main.Settings.EnableRelearnSpells && Models.SpellContext.IsSpellOfferedBySelectedClassSubclass(spell)
                                    ))
                                {
                                    spellDefinitionList.Add(spell);
                                }
                            }
                        }
                    }

                    // PATCH: don't allow scribed spells to be re-learned
                    var foundSpellbooks = new List<RulesetItemSpellbook>();

                    __instance.HeroCharacter.CharacterInventory.BrowseAllCarriedItems<RulesetItemSpellbook>(foundSpellbooks);
                    foreach (var foundSpellbook in foundSpellbooks)
                    {
                        foreach (var spell in foundSpellbook.ScribedSpells)
                        {
                            if (!spellDefinitionList.Contains(spell) &&
                                (
                                    Models.LevelUpContext.SelectedClass.Name == "Wizard" ||
                                    !Main.Settings.EnableRelearnScribedSpells && Models.SpellContext.IsSpellOfferedBySelectedClassSubclass(spell)
                                ))
                            {
                                spellDefinitionList.Add(spell);
                            }
                        }
                    }

                    // GAME CODE FROM HERE
                    foreach (var bonusCantrip in __instance.bonusCantrips)
                    {
                        if (bonusCantrip.Key != tagToIgnore)
                        {
                            foreach (var spellDefinition in bonusCantrip.Value)
                            {
                                if (!spellDefinitionList.Contains(spellDefinition))
                                {
                                    spellDefinitionList.Add(spellDefinition);
                                }
                            }
                        }
                    }

                    foreach (var acquiredCantrip in __instance.acquiredCantrips)
                    {
                        if (acquiredCantrip.Key != tagToIgnore)
                        {
                            foreach (var spellDefinition in acquiredCantrip.Value)
                            {
                                if (!spellDefinitionList.Contains(spellDefinition))
                                {
                                    spellDefinitionList.Add(spellDefinition);
                                }
                            }
                        }
                    }

                    foreach (var acquiredSpell in __instance.acquiredSpells)
                    {
                        if (acquiredSpell.Key != tagToIgnore)
                        {
                            foreach (var spellDefinition in acquiredSpell.Value)
                            {
                                if (!spellDefinitionList.Contains(spellDefinition))
                                {
                                    spellDefinitionList.Add(spellDefinition);
                                }
                            }
                        }
                    }

                    __result = spellDefinitionList;
                }

                return !(Models.LevelUpContext.LevelingUp && Models.LevelUpContext.IsMulticlass);
            }
        }

        // removes any levels from the tag otherwise it'll have a hard time finding it if multiclassed
        [HarmonyPatch(typeof(CharacterBuildingManager), "GetSpellFeature")]
        internal static class CharacterBuildingManagerGetSpellFeature
        {
            internal static bool Prefix(string tag, ref FeatureDefinitionCastSpell __result)
            {
                if (Models.LevelUpContext.LevelingUp && Models.LevelUpContext.IsMulticlass)
                {
                    var localTag = tag;

                    __result = null;

                    if (localTag.StartsWith("03Class"))
                    {
                        localTag = "03Class" + Models.LevelUpContext.SelectedClass.Name;
                    }
                    else if (localTag.StartsWith("06Subclass"))
                    {
                        localTag = "06Subclass" + Models.LevelUpContext.SelectedClass.Name;
                    }

                    // PATCH
                    foreach (var activeFeature in Models.LevelUpContext.SelectedHero.ActiveFeatures.Where(x => x.Key.StartsWith(localTag)))
                    {
                        foreach (var featureDefinition in activeFeature.Value)
                        {
                            if (featureDefinition is FeatureDefinitionCastSpell)
                            {
                                __result = featureDefinition as FeatureDefinitionCastSpell;
                                return false;
                            }
                        }
                    }

                    if (localTag.StartsWith("06Subclass"))
                    {
                        localTag = "03Class" + Models.LevelUpContext.SelectedClass.Name;

                        // PATCH
                        foreach (var activeFeature in Models.LevelUpContext.SelectedHero.ActiveFeatures.Where(x => x.Key.StartsWith(localTag)))
                        {
                            foreach (var featureDefinition in activeFeature.Value)
                            {
                                if (featureDefinition is FeatureDefinitionCastSpell)
                                {
                                    __result = featureDefinition as FeatureDefinitionCastSpell;
                                    return false;
                                }
                            }
                        }
                    }
                }

                return !(Models.LevelUpContext.LevelingUp && Models.LevelUpContext.IsMulticlass);
            }
        }

        // ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "GrantBaseEquipment")]
        internal static class CharacterBuildingManagerGrantBaseEquipment
        {
            internal static bool Prefix()
            {
                return !(Models.LevelUpContext.LevelingUp && Models.LevelUpContext.DisplayingClassPanel);
            }
        }

        // ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "GrantFeatures")]
        internal static class CharacterBuildingManagerGrantFeatures
        {
            internal static bool Prefix()
            {
                return !(Models.LevelUpContext.LevelingUp && Models.LevelUpContext.DisplayingClassPanel);
            }
        }

        // ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "RemoveBaseEquipment")]
        internal static class CharacterBuildingManagerRemoveBaseEquipment
        {
            internal static bool Prefix()
            {
                return !(Models.LevelUpContext.LevelingUp && Models.LevelUpContext.DisplayingClassPanel);
            }
        }

       // ensures this doesn't get executed in the class panel level up screen
       [HarmonyPatch(typeof(CharacterBuildingManager), "TransferOrSpawnWieldedItem")]
        internal static class CharacterBuildingManagerTransferOrSpawnWieldedItem
        {
            internal static bool Prefix()
            {
                return !(Models.LevelUpContext.LevelingUp && Models.LevelUpContext.DisplayingClassPanel);
            }
        }

        // ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "UnassignEquipment")]
        internal static class CharacterBuildingManagerUnassignEquipment
        {
            internal static bool Prefix()
            {
                return !(Models.LevelUpContext.LevelingUp && Models.LevelUpContext.DisplayingClassPanel);
            }
        }

        // ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "UnassignLastClassLevel")]
        internal static class CharacterBuildingManagerUnassignLastClassLevel
        {
            internal static bool Prefix()
            {
                return !(Models.LevelUpContext.LevelingUp && Models.LevelUpContext.DisplayingClassPanel);
            }
        }

        // ensures the level up process only offers spells from the leveling up class
        [HarmonyPatch(typeof(CharacterBuildingManager), "UpgradeSpellPointPools")]
        internal static class CharacterBuildingManagerUpgradeSpellPointPools
        {
            internal static bool Prefix(CharacterBuildingManager __instance)
            {
                if (Models.LevelUpContext.LevelingUp && Models.LevelUpContext.IsMulticlass)
                {
                    foreach (var spellRepertoire in __instance.HeroCharacter.SpellRepertoires)
                    {
                        var poolName = string.Empty;
                        var maxPoints = 0;

                        if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Class)
                        {
                            // PATCH: short circuit if the feature is for another class (change from native code)
                            if (spellRepertoire.SpellCastingClass != Models.LevelUpContext.SelectedClass)
                            {
                                continue;
                            }

                            poolName = AttributeDefinitions.GetClassTag(Models.LevelUpContext.SelectedClass, Models.LevelUpContext.SelectedClassLevel); // SelectedClassLevel ???
                        }
                        else if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Subclass)
                        {
                            // PATCH: short circuit if the feature is for another subclass (change from native code)
                            if (spellRepertoire.SpellCastingSubclass != Models.LevelUpContext.SelectedSubclass)
                            {
                                continue;
                            }

                            poolName = AttributeDefinitions.GetSubclassTag(Models.LevelUpContext.SelectedClass, Models.LevelUpContext.SelectedClassLevel, Models.LevelUpContext.SelectedSubclass); // SelectedClassLevel ???
                        }
                        else if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Race)
                        {
                            poolName = "02Race";
                        }

                        __instance.tempAcquiredCantripsNumber = 0;
                        __instance.tempAcquiredSpellsNumber = 0;
                        __instance.tempUnlearnedSpellsNumber = 0;

                        __instance.ApplyFeatureCastSpell(spellRepertoire.SpellCastingFeature);

                        if (__instance.HasAnyActivePoolOfType(HeroDefinitions.PointsPoolType.Cantrip) && __instance.PointPoolStacks[HeroDefinitions.PointsPoolType.Cantrip].ActivePools.ContainsKey(poolName))
                        {
                            maxPoints = __instance.PointPoolStacks[HeroDefinitions.PointsPoolType.Cantrip].ActivePools[poolName].MaxPoints;
                        }

                        __instance.SetPointPool(HeroDefinitions.PointsPoolType.Cantrip, poolName, __instance.tempAcquiredCantripsNumber + maxPoints);
                        __instance.SetPointPool(HeroDefinitions.PointsPoolType.Spell, poolName, __instance.tempAcquiredSpellsNumber);
                        __instance.SetPointPool(HeroDefinitions.PointsPoolType.SpellUnlearn, poolName, __instance.tempUnlearnedSpellsNumber);
                    }
                }

                return !(Models.LevelUpContext.LevelingUp && Models.LevelUpContext.IsMulticlass);
            }
        }
    }
}
