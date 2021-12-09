using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class CharacterInformationPanelPatcher
    {
        [HarmonyPatch(typeof(CharacterInformationPanel), "EnumerateClassBadges")]
        internal static class CharacterInformationPanelEnumerateClassBadges
        {
            internal static bool Prefix(CharacterInformationPanel __instance)
            {
                if (Models.InspectionPanelContext.IsMulticlass)
                {
                    var rulesetCharacterHero = Models.InspectionPanelContext.SelectedHero;

                    __instance.badgeDefinitions.Clear();

                    foreach (var classesAndSubclass in rulesetCharacterHero.ClassesAndSubclasses)
                    {
                        if (classesAndSubclass.Key == Models.InspectionPanelContext.SelectedClass)
                        {
                            __instance.badgeDefinitions.Add(classesAndSubclass.Value);
                        }
                    }

                    if (Models.InspectionPanelContext.RequiresDeity)
                    {
                        __instance.badgeDefinitions.Add(rulesetCharacterHero.DeityDefinition);
                    }

                    foreach (var trainedFightingStyle in Models.InspectionPanelContext.GetTrainedFightingStyles())
                    {
                        __instance.badgeDefinitions.Add(trainedFightingStyle);
                    }

                    while (__instance.classBadgesTable.childCount < __instance.badgeDefinitions.Count)
                    {
                        Gui.GetPrefabFromPool(__instance.classBadgePrefab, __instance.classBadgesTable);
                    }

                    var index = 0;

                    foreach (var badgeDefinition in __instance.badgeDefinitions)
                    {
                        var child = __instance.classBadgesTable.GetChild(index);

                        child.gameObject.SetActive(true);
                        child.GetComponent<CharacterInformationBadge>().Bind(badgeDefinition, __instance.classBadgesTable);
                        ++index;
                    }

                    for (; index < __instance.classBadgesTable.childCount; ++index)
                    {
                        var child = __instance.classBadgesTable.GetChild(index);

                        child.GetComponent<CharacterInformationBadge>().Unbind();
                        child.gameObject.SetActive(false);
                    }

                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(CharacterInformationPanel), "Refresh")]
        internal static class CharacterInformationPanelRefresh
        {
            public static string GetSelectedClassSearchTerm(string original)
            {
                return original + Models.InspectionPanelContext.SelectedClass.Name;
            }

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var containsMethod = typeof(string).GetMethod("Contains");
                var getSelectedClassSearchTermMethod = typeof(CharacterInformationPanelRefresh).GetMethod("GetSelectedClassSearchTerm");
                var found = 0;

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(containsMethod))
                    {
                        found++;
                        if (found == 2 || found == 3)
                        {
                            yield return new CodeInstruction(OpCodes.Call, getSelectedClassSearchTermMethod);
                        }

                        yield return instruction;
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }
    }
}
