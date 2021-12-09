using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using TMPro;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class FeatureDescriptionItemPatcher
    {
        [HarmonyPatch(typeof(FeatureDescriptionItem), "Bind")]
        internal static class FeatureDescriptionItemBind
        {
            public static void DisableDropdownIfMulticlass(FeatureDescriptionItem featureDescriptionItem)
            {
                var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();
                var hero = characterBuildingService.HeroCharacter;

                if (Models.LevelUpContext.LevelingUp && Models.LevelUpContext.DisplayingClassPanel && hero.ClassesAndLevels.ContainsKey(Models.LevelUpContext.SelectedClass))
                {
                    var featureDefinitionFeatureSet = featureDescriptionItem.Feature as FeatureDefinitionFeatureSet;
                    var featureDefinitions = new List<FeatureDefinition>();

                    foreach (var activeFeature in characterBuildingService.HeroCharacter.ActiveFeatures)
                    {
                        if (activeFeature.Key.StartsWith("03Class"))
                        {
                            featureDefinitions.AddRange(activeFeature.Value);
                        }
                    }

                    for (var index = 0; index < featureDefinitionFeatureSet.FeatureSet.Count; ++index)
                    {
                        if (featureDefinitions.Contains(featureDefinitionFeatureSet.FeatureSet[index]))
                        {
                            featureDescriptionItem.choiceDropdown.value = index;
                        }
                    }

                    featureDescriptionItem.choiceDropdown.interactable = false;
                }
                else
                {
                    featureDescriptionItem.choiceDropdown.interactable = true;
                }
            }

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var setValueMethod = typeof(TMP_Dropdown).GetMethod("set_value");
                var getLastAssignedClassAndLevelCustomMethod = typeof(FeatureDescriptionItemBind).GetMethod("DisableDropdownIfMulticlass");

                foreach (var instruction in instructions)
                {
                    yield return instruction;

                    if (instruction.Calls(setValueMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Call, getLastAssignedClassAndLevelCustomMethod);
                    }
                }
            }
        }
    }
}
