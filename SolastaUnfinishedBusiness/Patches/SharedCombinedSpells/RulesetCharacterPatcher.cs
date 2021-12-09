using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class RulesetCharacterPatcherSharedSpells
    {
        // use caster level instead of character level on multiclassed heroes
        [HarmonyPatch(typeof(RulesetCharacter), "GetSpellcastingLevel")]
        internal static class RulesetCharacterGetSpellcastingLevel
        {
            internal static void Postfix(ref int __result, RulesetSpellRepertoire spellRepertoire)
            {
                var heroWithSpellRepertoire = Models.SharedSpellsContext.GetHero(spellRepertoire.CharacterName);

                if (heroWithSpellRepertoire == null)
                {
                    return;
                }

                if (!Models.SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire))
                {
                    return;
                }

                if (spellRepertoire?.SpellCastingFeature?.SpellCastingOrigin != FeatureDefinitionCastSpell.CastingOrigin.Race && spellRepertoire?.SpellCastingClass != null)
                {
                    __result = heroWithSpellRepertoire.ClassesAndLevels[spellRepertoire.SpellCastingClass];
                }
            }
        }

        // ensures ritual spells work correctly when MC
        [HarmonyPatch(typeof(RulesetCharacter), "CanCastAnyRitualSpell")]
        internal static class RulesetCharacterCanCastAnyRitualSpell
        {
            internal static bool Prefix(RulesetCharacter __instance, ref bool __result)
            {
                var canCast = false;

                __instance.EnumerateFeaturesToBrowse<FeatureDefinitionMagicAffinity>(__instance.FeaturesToBrowse);

                foreach (var featureDefinition in __instance.FeaturesToBrowse)
                {
                    if (featureDefinition is FeatureDefinitionMagicAffinity featureDefinitionMagicAffinity && featureDefinitionMagicAffinity.RitualCasting != RuleDefinitions.RitualCasting.None)
                    {
                        var ritualType = featureDefinitionMagicAffinity.RitualCasting;

                        __instance.EnumerateUsableRitualSpells(ritualType, __instance.usableSpells);

                        if (__instance.usableSpells.Count > 0)
                        {
                            canCast = true;
                            break;
                        }
                    }
                }

                __result = canCast;

                return false;
            }
        }

        // logic to correctly offer / calculate spell slots on all different scenarios
        [HarmonyPatch(typeof(RulesetCharacter), "RefreshSpellRepertoires")]
        internal static class RulesetCharacterRefreshSpellRepertoires
        {
            private static readonly Dictionary<int, int> sorcererSlots = new Dictionary<int, int>();

            public static void MyComputeSpellSlots(RulesetSpellRepertoire spellRepertoire, List<FeatureDefinition> spellCastingAffinities)
            {
                sorcererSlots.Clear();

                var mySpellCastingAffinities = new List<FeatureDefinition>();
                var heroWithSpellRepertoire = Models.SharedSpellsContext.GetHero(spellRepertoire.CharacterName);

                mySpellCastingAffinities.AddRange(spellCastingAffinities);

                if (heroWithSpellRepertoire != null && spellCastingAffinities != null && spellCastingAffinities.Count > 0)
                {
                    foreach (var featureDefinition in spellCastingAffinities)
                    {
                        if (featureDefinition is ISpellCastingAffinityProvider spellCastingAffinityProvider)
                        {
                            foreach (var additionalSlot in spellCastingAffinityProvider.AdditionalSlots)
                            {
                                if (!sorcererSlots.ContainsKey(additionalSlot.SlotLevel))
                                {
                                    sorcererSlots[additionalSlot.SlotLevel] = 0;
                                }

                                sorcererSlots[additionalSlot.SlotLevel] += additionalSlot.SlotsNumber;
                                mySpellCastingAffinities.Remove(featureDefinition);
                            }
                        }
                    }
                }

                if (Models.SharedSpellsContext.IsEnabled || spellRepertoire.SpellCastingClass?.Name.Contains("Sorcerer") != true)
                {
                    spellRepertoire.ComputeSpellSlots(mySpellCastingAffinities);
                }
                else
                {
                    spellRepertoire.ComputeSpellSlots(spellCastingAffinities);
                }
            }

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var computeSpellSlotsMethod = typeof(RulesetSpellRepertoire).GetMethod("ComputeSpellSlots");
                var myComputeSpellSlotsMethod = typeof(RulesetCharacterRefreshSpellRepertoires).GetMethod("MyComputeSpellSlots");

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(computeSpellSlotsMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Call, myComputeSpellSlotsMethod);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }

            internal static void Postfix(RulesetCharacter __instance)
            {
                if (!Models.SharedSpellsContext.IsEnabled)
                {
                    return;
                }

                if (!(__instance is RulesetCharacterHero heroWithSpellRepertoire))
                {
                    return;
                }

                var warlockRepertoire = Models.SharedSpellsContext.GetWarlockSpellRepertoire(heroWithSpellRepertoire);

                foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires)
                {
                    if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Race)
                    {
                        continue;
                    }

                    if (spellRepertoire == warlockRepertoire)
                    {
                        continue;
                    }

                    var spellsSlotCapacities = (Dictionary<int, int>)AccessTools.Field(spellRepertoire.GetType(), "spellsSlotCapacities").GetValue(spellRepertoire);

                    // replaces standard caster slots with shared slots system
                    if (Models.SharedSpellsContext.IsSharedcaster(heroWithSpellRepertoire))
                    {
                        var sharedCasterLevel = Models.SharedSpellsContext.GetSharedCasterLevel(heroWithSpellRepertoire);
                        var spellLevel = Models.SharedSpellsContext.GetSharedSpellLevel(heroWithSpellRepertoire);

                        spellsSlotCapacities.Clear();

                        // adds shared slots
                        for (var i = 1; i <= spellLevel; i++)
                        {
                            spellsSlotCapacities[i] = Models.SharedSpellsContext.FullCastingSlots[sharedCasterLevel - 1].Slots[i - 1];
                        }
                    }

                    // adds sorcerer slots from points
                    foreach (var slot in sorcererSlots)
                    {
                        if (!spellsSlotCapacities.ContainsKey(slot.Key))
                        {
                            spellsSlotCapacities[slot.Key] = 0;
                        }
                        spellsSlotCapacities[slot.Key] += slot.Value;
                    }

                    spellRepertoire.RepertoireRefreshed?.Invoke(spellRepertoire);
                }

                var anySharedRepertoire = heroWithSpellRepertoire.SpellRepertoires.Find(sr => !Models.SharedSpellsContext.IsWarlock(sr.SpellCastingClass) &&
                    (sr.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Class || sr.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Subclass));

                // combines the Shared Slot System and Warlock Pact Magic
                if (warlockRepertoire != null && anySharedRepertoire != null && Models.SharedSpellsContext.IsCombined)
                {
                    var warlockSlotsCapacities = (Dictionary<int, int>)AccessTools.Field(warlockRepertoire.GetType(), "spellsSlotCapacities").GetValue(warlockRepertoire);
                    var anySharedSlotsCapacities = (Dictionary<int, int>)AccessTools.Field(anySharedRepertoire.GetType(), "spellsSlotCapacities").GetValue(anySharedRepertoire);

                    for (var i = 1; i <= Math.Max(warlockSlotsCapacities.Count, anySharedSlotsCapacities.Count); i++)
                    {
                        if (!warlockSlotsCapacities.ContainsKey(i))
                        {
                            warlockSlotsCapacities[i] = 0;
                        }

                        if (anySharedSlotsCapacities.ContainsKey(i))
                        {
                            warlockSlotsCapacities[i] += anySharedSlotsCapacities[i];
                        }
                    }

                    foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires)
                    {
                        if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Race)
                        {
                            continue;
                        }

                        // ignores Warlock repertoire
                        if (spellRepertoire != warlockRepertoire)
                        {
                            var spellsSlotCapacities = (Dictionary<int, int>)AccessTools.Field(spellRepertoire.GetType(), "spellsSlotCapacities").GetValue(spellRepertoire);

                            spellsSlotCapacities.Clear();

                            foreach (var warlockSlotCapacities in warlockSlotsCapacities)
                            {
                                spellsSlotCapacities[warlockSlotCapacities.Key] = warlockSlotCapacities.Value;
                            }

                            spellRepertoire.RepertoireRefreshed?.Invoke(spellRepertoire);
                        }
                        else
                        {
                            warlockRepertoire.RepertoireRefreshed?.Invoke(warlockRepertoire);
                        }
                    }
                }
            }
        }
    }
}
