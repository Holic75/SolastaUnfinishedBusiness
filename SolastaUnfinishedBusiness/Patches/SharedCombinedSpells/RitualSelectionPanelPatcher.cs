using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class RitualSelectionPanelPatcher
    {
        // ensures ritual spells from all spell repertoires are made available
        [HarmonyPatch(typeof(RitualSelectionPanel), "Bind")]
        internal static class RitualSelectionPanelBind
        {
            internal static bool Prefix(
                RitualSelectionPanel __instance,
                GameLocationCharacter caster,
                RitualSelectionPanel.RitualCastCancelledHandler ritualCastCancelled,
                RitualBox.RitualCastEngagedHandler ritualCastEngaged)
            {
                var rulesetCharacter = caster.RulesetCharacter;
                var ritualType = RuleDefinitions.RitualCasting.None;

                __instance.Caster = caster;
                __instance.RitualCastCancelled = ritualCastCancelled;
                rulesetCharacter.EnumerateFeaturesToBrowse<FeatureDefinitionMagicAffinity>(rulesetCharacter.FeaturesToBrowse);

                // BEGIN PATCH
                var ritualSpells = new List<SpellDefinition>();

                __instance.ritualSpells.Clear();

                foreach (var featureDefinition in rulesetCharacter.FeaturesToBrowse)
                {
                    if (featureDefinition is FeatureDefinitionMagicAffinity featureDefinitionMagicAffinity && featureDefinitionMagicAffinity.RitualCasting != RuleDefinitions.RitualCasting.None)
                    {
                        ritualType = featureDefinitionMagicAffinity.RitualCasting;
                        __instance.Caster.RulesetCharacter.EnumerateUsableRitualSpells(ritualType, ritualSpells);
                        foreach (var ritualSpell in ritualSpells)
                        {
                            if (!__instance.ritualSpells.Contains(ritualSpell))
                            {
                                __instance.ritualSpells.Add(ritualSpell);
                            }
                        }
                    }
                }
                // END PATCH

                while (__instance.ritualBoxesTable.childCount < __instance.ritualSpells.Count)
                {
                    Gui.GetPrefabFromPool(__instance.ritualBoxPrefab, __instance.ritualBoxesTable);
                }

                for (var index = 0; index < __instance.ritualBoxesTable.childCount; ++index)
                {
                    var child = __instance.ritualBoxesTable.GetChild(index);
                    if (index < __instance.ritualSpells.Count)
                    {
                        if (!child.gameObject.activeSelf)
                        {
                            child.gameObject.SetActive(true);
                        }

                        child.GetComponent<RitualBox>().Bind(rulesetCharacter, __instance.ritualSpells[index], ritualCastEngaged);
                    }
                    else if (child.gameObject.activeSelf)
                    {
                        child.GetComponent<RitualBox>().Unbind();
                        child.gameObject.SetActive(false);
                    }
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(__instance.labelTransform);

                var a = __instance.labelTransform.rect.width + 2f * __instance.labelTransform.anchoredPosition.x;

                LayoutRebuilder.ForceRebuildLayoutImmediate(__instance.ritualBoxesTable);

                __instance.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Max(a, __instance.ritualBoxesTable.rect.width));

                return false;
            }
        }
    }
}
