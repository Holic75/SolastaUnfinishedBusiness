
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class CharacterStageClassSelectionPanelPatcher
    {
        // flags displaying the class panel
        [HarmonyPatch(typeof(CharacterStageClassSelectionPanel), "OnBeginShow")]
        internal static class CharacterStageClassSelectionPanelOnBeginShow
        {
            internal static void Prefix(CharacterStageClassSelectionPanel __instance)
            {
                if (Models.LevelUpContext.LevelingUp)
                {
                    Models.LevelUpContext.DisplayingClassPanel = true;
                    Models.InOutRules.EnumerateHeroAllowedClassDefinitions(Models.LevelUpContext.SelectedHero, __instance.compatibleClasses, ref __instance.selectedClass);
                    __instance.CommonData.AttackModesPanel?.RefreshNow();
                    __instance.CommonData.PersonalityMapPanel?.RefreshNow();
                }
                else
                {
                    __instance.compatibleClasses.Sort((a, b) => a.FormatTitle().CompareTo(b.FormatTitle()));
                }

                __instance.classesTable.pivot = new Vector2(0.5f, 1.0f);
            }
        }

        [HarmonyPatch(typeof(CharacterStageClassSelectionPanel), "OnHigherLevelCb")]
        internal static class CharacterStageClassSelectionPanelOnHigherLevelCb
        {
            internal static bool Prefix()
            {
                if (Models.LevelUpContext.LevelingUp)
                {
                    Gui.GuiService.GetScreen<HigherLevelFeaturesModal>().Show(Models.LevelUpContext.SelectedClass.FeatureUnlocks, Models.LevelUpContext.SelectedClassLevel);

                    return false;
                }

                return true;
            }
        }

        // patches the method to get my own classLevel
        [HarmonyPatch(typeof(CharacterStageClassSelectionPanel), "FillClassFeatures")]
        internal static class CharacterStageClassSelectionPanelFillClassFeatures
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var getHeroCharacterMethod = typeof(ICharacterBuildingService).GetMethod("get_HeroCharacter");
                var getClassLevelMethod = typeof(Models.LevelUpContext).GetMethod("GetClassLevel");
                var instructionsToBypass = 0;

                foreach (var instruction in instructions)
                {
                    if (instructionsToBypass > 0)
                    {
                        instructionsToBypass--;
                    }
                    else if (instruction.Calls(getHeroCharacterMethod))
                    {
                        yield return instruction;
                        yield return new CodeInstruction(OpCodes.Call, getClassLevelMethod);
                        instructionsToBypass = 3;
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }

        // patches the method to get my own classLevel
        [HarmonyPatch(typeof(CharacterStageClassSelectionPanel), "RefreshCharacter")]
        internal static class CharacterStageClassSelectionPanelRefreshCharacter
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var getHeroCharacterMethod = typeof(ICharacterBuildingService).GetMethod("get_HeroCharacter");
                var getClassLevelMethod = typeof(Models.LevelUpContext).GetMethod("GetClassLevel");
                var instructionsToBypass = 0;

                foreach (var instruction in instructions)
                {
                    if (instructionsToBypass > 0)
                    {
                        instructionsToBypass--;
                    }
                    else if (instruction.Calls(getHeroCharacterMethod))
                    {
                        yield return instruction;
                        yield return new CodeInstruction(OpCodes.Call, getClassLevelMethod);
                        instructionsToBypass = 3;
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }

        // hides the equipment panel group on level up
        [HarmonyPatch(typeof(CharacterStageClassSelectionPanel), "Refresh")]
        internal static class CharacterStageClassSelectionPanelRefresh
        {
            public static bool SetActive()
            {
                return !(Models.LevelUpContext.LevelingUp && Models.LevelUpContext.DisplayingClassPanel);
            }

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var setActiveFound = 0;
                var setActiveMethod = typeof(GameObject).GetMethod("SetActive");
                var mySetActiveMethod = typeof(CharacterStageClassSelectionPanelRefresh).GetMethod("SetActive");

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(setActiveMethod) && ++setActiveFound == 4)
                    {
                        yield return new CodeInstruction(OpCodes.Pop);
                        yield return new CodeInstruction(OpCodes.Call, mySetActiveMethod);
                    }

                    yield return instruction;
                }
            }
        }
    }
}
