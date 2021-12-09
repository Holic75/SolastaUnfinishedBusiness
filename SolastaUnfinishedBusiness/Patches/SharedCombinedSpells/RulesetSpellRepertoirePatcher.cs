﻿using System.Collections.Generic;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class RulesetSpellRepertoirePatcher
    {
        // handles all different scenarios to determine max slots numbers
        [HarmonyPatch(typeof(RulesetSpellRepertoire), "GetMaxSlotsNumberOfAllLevels")]
        internal static class RulesetSpellRepertoireGetMaxSlotsNumberOfAllLevels
        {
            internal static bool Prefix(RulesetSpellRepertoire __instance, ref int __result)
            {
                var heroWithSpellRepertoire = Models.SharedSpellsContext.GetHero(__instance.CharacterName);

                if (heroWithSpellRepertoire == null)
                {
                    return true;
                }

                if (Models.SharedSpellsContext.IsWarlock(__instance.SpellCastingClass))
                {
                    if (Models.SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire))
                    {
                        if (Models.SharedSpellsContext.IsEnabled && Models.SharedSpellsContext.IsCombined)
                        {
                            // handles MC Warlock with combined system
                            return true;
                        }
                        else
                        {
                            // handles MC Warlock without combined system
                            __result = Models.SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);
                            return false;
                        }
                    }
                    else
                    {
                        // handles SC Warlock
                        __result = Models.SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);
                        return false;
                    }
                }
                else
                {
                    // handles SC non Warlock and MC non Warlock
                    return true;
                }
            }
        }

        // handles all different scenarios to determine remaining slots numbers
        [HarmonyPatch(typeof(RulesetSpellRepertoire), "GetRemainingSlotsNumberOfAllLevels")]
        internal static class RulesetSpellRepertoireGetRemainingSlotsNumberOfAllLevels
        {
            internal static bool Prefix(RulesetSpellRepertoire __instance, ref int __result)
            {
                var heroWithSpellRepertoire = Models.SharedSpellsContext.GetHero(__instance.CharacterName);

                if (heroWithSpellRepertoire == null)
                {
                    return true;
                }

                if (Models.SharedSpellsContext.IsWarlock(__instance.SpellCastingClass))
                {
                    if (Models.SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire))
                    {
                        if (Models.SharedSpellsContext.IsEnabled && Models.SharedSpellsContext.IsCombined)
                        {
                            // handles MC Warlock with combined system
                            return true;
                        }
                        else
                        {
                            // handles MC Warlock without combined system
                            var max = Models.SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);

                            __instance.usedSpellsSlots.TryGetValue(-1, out var used);
                            __result = max - used;
                            return false;
                        }
                    }
                    else
                    {
                        // handles SC Warlock
                        var max = Models.SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);

                        __instance.usedSpellsSlots.TryGetValue(-1, out var used);
                        __result = max - used;
                        return false;
                    }
                }
                else
                {
                    // handles SC non Warlock and MC non Warlock
                    return true;
                }
            }
        }

        // handles all different scenarios to determine slots numbers
        [HarmonyPatch(typeof(RulesetSpellRepertoire), "GetSlotsNumber")]
        internal static class RulesetSpellRepertoireGetSlotsNumber
        {
            internal static bool Prefix(RulesetSpellRepertoire __instance, int spellLevel, ref int remaining, ref int max)
            {
                var heroWithSpellRepertoire = Models.SharedSpellsContext.GetHero(__instance.CharacterName);

                if (heroWithSpellRepertoire == null)
                {
                    return true;
                }

                max = 0;
                remaining = 0;

                if (Models.SharedSpellsContext.IsWarlock(__instance.SpellCastingClass))
                {
                    if (Models.SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire))
                    {
                        if (Models.SharedSpellsContext.IsEnabled && Models.SharedSpellsContext.IsCombined)
                        {
                            // handles MC Warlock with combined system
                            __instance.spellsSlotCapacities.TryGetValue(spellLevel, out max);
                            __instance.usedSpellsSlots.TryGetValue(spellLevel, out var used);
                            remaining = max - used;
                        }
                        else
                        {
                            // handles MC Warlock without combined system
                            if (spellLevel <= __instance.MaxSpellLevelOfSpellCastingLevel)
                            {
                                max = Models.SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);
                                __instance.usedSpellsSlots.TryGetValue(-1, out var used);
                                remaining = max - used;
                            }
                        }
                    }
                    else
                    {
                        // handles SC Warlock
                        if (spellLevel <= __instance.MaxSpellLevelOfSpellCastingLevel)
                        {
                            max = Models.SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);
                            __instance.usedSpellsSlots.TryGetValue(-1, out var used);
                            remaining = max - used;
                        }
                    }
                }
                else
                {
                    // handles SC non Warlock and MC non Warlock
                    __instance.spellsSlotCapacities.TryGetValue(spellLevel, out max);
                    __instance.usedSpellsSlots.TryGetValue(spellLevel, out var used);
                    remaining = max - used;
                }

                return false;
            }
        }

        // handles all different scenarios to determine max spell level
        [HarmonyPatch(typeof(RulesetSpellRepertoire), "MaxSpellLevelOfSpellCastingLevel", MethodType.Getter)]
        internal static class RulesetSpellRepertoireMaxSpellLevelOfSpellCastingLevelGetter
        {
            internal static void Postfix(RulesetSpellRepertoire __instance, ref int __result)
            {
                var heroWithSpellRepertoire = Models.SharedSpellsContext.GetHero(__instance.CharacterName);

                if (heroWithSpellRepertoire == null)
                {
                    return;
                }

                if (Models.SharedSpellsContext.IsEnabled && Models.SharedSpellsContext.IsCombined)
                {
                    __result = Models.SharedSpellsContext.GetCombinedSpellLevel(heroWithSpellRepertoire);
                }
                else
                {
                    if (Models.SharedSpellsContext.IsWarlock(__instance.SpellCastingClass))
                    {
                        __result = Models.SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);
                    }
                    else if (Models.SharedSpellsContext.IsEnabled)
                    {
                        __result = Models.SharedSpellsContext.GetSharedSpellLevel(heroWithSpellRepertoire);
                    }
                }
            }
        }

        // handles Warlock short rest spells recovery
        [HarmonyPatch(typeof(RulesetSpellRepertoire), "RestoreAllSpellSlots")]
        internal static class RulesetSpellRepertoireRestoreAllSpellSlots
        {
            internal static bool Prefix(RulesetSpellRepertoire __instance)
            {
                var heroWithSpellRepertoire = Models.SharedSpellsContext.GetHero(__instance.CharacterName);

                if (heroWithSpellRepertoire == null)
                {
                    return true;
                }

                if (Models.SharedSpellsContext.RestType == RuleDefinitions.RestType.LongRest)
                {
                    return true;
                }

                if (!Models.SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire))
                {
                    return true;
                }

                if (!Models.SharedSpellsContext.IsEnabled)
                {
                    return true;
                }

                if (!Models.SharedSpellsContext.IsCombined)
                {
                    return true;
                }

                var warlockSpellLevel = Models.SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);

                __instance.usedSpellsSlots.TryGetValue(-1, out var slotsToRestore);

                foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires)
                {
                    if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Race)
                    {
                        continue;
                    }

                    var usedSpellsSlots = (Dictionary<int, int>)AccessTools.Field(spellRepertoire.GetType(), "usedSpellsSlots").GetValue(spellRepertoire);

                    // uses index -1 to keep a tab on short rest slots usage
                    for (var i = -1; i <= warlockSpellLevel; i++)
                    {
                        if (i == 0)
                        {
                            continue;
                        }

                        if (usedSpellsSlots.ContainsKey(i))
                        {
                            usedSpellsSlots[i] -= slotsToRestore;
                        }
                    }

                    spellRepertoire.RepertoireRefreshed?.Invoke(spellRepertoire);
                }

                return false;
            }
        }

        // handles all different scenarios of spell slots consumption (casts, smites, point buys)
        [HarmonyPatch(typeof(RulesetSpellRepertoire), "SpendSpellSlot")]
        internal static class RulesetSpellRepertoireSpendSpellSlot
        {
            internal static bool Prefix(RulesetSpellRepertoire __instance, int slotLevel)
            {
                if (slotLevel == 0)
                {
                    return true;
                }

                var heroWithSpellRepertoire = Models.SharedSpellsContext.GetHero(__instance.CharacterName);

                if (heroWithSpellRepertoire == null)
                {
                    return true;
                }

                // handles SC Warlock or MC Warlock without combined system
                if (Models.SharedSpellsContext.IsWarlock(__instance.SpellCastingClass) && (!Models.SharedSpellsContext.IsCombined || !Models.SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire)))
                {
                    SpendWarlockSlots(__instance, heroWithSpellRepertoire);
                    return false;
                }

                if (!Models.SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire))
                {
                    return true;
                }

                if (!Models.SharedSpellsContext.IsEnabled)
                {
                    return true;
                }

                var warlockSpellRepertoire = Models.SharedSpellsContext.GetWarlockSpellRepertoire(heroWithSpellRepertoire);

                // handles MC non-Warlock or MC without combined system
                if (warlockSpellRepertoire == null || !Models.SharedSpellsContext.IsCombined)
                {
                    foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires)
                    {
                        if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Race)
                        {
                            continue;
                        }

                        if (spellRepertoire.SpellCastingFeature.SlotsRecharge == RuleDefinitions.RechargeRate.LongRest)
                        {
                            var usedSpellsSlots = (Dictionary<int, int>)AccessTools.Field(spellRepertoire.GetType(), "usedSpellsSlots").GetValue(spellRepertoire);

                            if (!usedSpellsSlots.ContainsKey(slotLevel))
                            {
                                usedSpellsSlots.Add(slotLevel, 0);
                            }
                            usedSpellsSlots[slotLevel]++;
                            spellRepertoire.RepertoireRefreshed?.Invoke(spellRepertoire);
                        }
                    }
                }

                // handles MC Warlock with combined system 
                else
                {
                    var sharedSpellLevel = Models.SharedSpellsContext.GetSharedSpellLevel(heroWithSpellRepertoire);
                    var warlockSpellLevel = Models.SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);
                    var warlockMaxSlots = Models.SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);
                    var usedSpellsSlotsWarlock = (Dictionary<int, int>)AccessTools.Field(warlockSpellRepertoire.GetType(), "usedSpellsSlots").GetValue(warlockSpellRepertoire);

                    // index 0 keeps a tab of short rest slots used
                    usedSpellsSlotsWarlock.TryGetValue(-1, out var warlockUsedSlots);
                    __instance.GetSlotsNumber(slotLevel, out var sharedRemainingSlots, out var sharedMaxSlots);

                    var sharedUsedSlots = sharedMaxSlots - sharedRemainingSlots;

                    sharedMaxSlots -= warlockMaxSlots;
                    sharedUsedSlots -= warlockUsedSlots;

                    var canConsumeShortRestSlot = warlockUsedSlots < warlockMaxSlots && slotLevel <= warlockSpellLevel;
                    var canConsumeLongRestSlot = sharedUsedSlots < sharedMaxSlots && slotLevel <= sharedSpellLevel;

                    var forceLongRestSlotUI = canConsumeLongRestSlot && Models.SharedSpellsContext.ForceLongRestSlot;
                    var forceLongRestSlotSettings = canConsumeLongRestSlot && Main.Settings.EnableConsumeLongRestSlotFirst && sharedSpellLevel < warlockSpellLevel;

                    // uses short rest slots across all repertoires
                    if (canConsumeShortRestSlot && !(forceLongRestSlotUI || forceLongRestSlotSettings))
                    {
                        foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires)
                        {
                            if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Race)
                            {
                                continue;
                            }

                            SpendWarlockSlots(spellRepertoire, heroWithSpellRepertoire);
                        }
                    }

                    // otherwise uses long rest slots across all repertoires
                    else
                    {
                        foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires)
                        {
                            if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Race)
                            {
                                continue;
                            }

                            var usedSpellsSlots = (Dictionary<int, int>)AccessTools.Field(spellRepertoire.GetType(), "usedSpellsSlots").GetValue(spellRepertoire);

                            if (!usedSpellsSlots.ContainsKey(slotLevel))
                            {
                                usedSpellsSlots.Add(slotLevel, 0);
                            }
                            usedSpellsSlots[slotLevel]++;
                            spellRepertoire.RepertoireRefreshed?.Invoke(spellRepertoire);
                        }
                    }
                }

                return false;
            }

            private static void SpendWarlockSlots(RulesetSpellRepertoire rulesetSpellRepertoire, RulesetCharacterHero heroWithSpellRepertoire)
            {
                var warlockSpellLevel = Models.SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);
                var usedSpellSlots = (Dictionary<int, int>)AccessTools.Field(rulesetSpellRepertoire.GetType(), "usedSpellsSlots").GetValue(rulesetSpellRepertoire);

                // uses index -1 to keep a tab on short rest slots usage
                for (var i = -1; i <= warlockSpellLevel; i++)
                {
                    if (i == 0)
                    {
                        continue;
                    }

                    if (!usedSpellSlots.ContainsKey(i))
                    {
                        usedSpellSlots.Add(i, 0);
                    }
                    usedSpellSlots[i]++;
                }

                rulesetSpellRepertoire.RepertoireRefreshed?.Invoke(rulesetSpellRepertoire);
            }
        }
    }
}
