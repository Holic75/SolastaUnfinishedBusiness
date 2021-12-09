using HarmonyLib;
using static SolastaUnfinishedBusiness.Settings;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class CharacterInspectionScreenPatcher
    {
        [HarmonyPatch(typeof(CharacterInspectionScreen), "Bind")]
        internal static class CharacterInspectionScreenBind
        {
            internal static void Prefix(CharacterInspectionScreen __instance, RulesetCharacterHero heroCharacter)
            {
                var classesCount = heroCharacter.ClassesAndLevels.Count;

                __instance.characterPlate.classLabel.TMP_Text.fontSize = Models.GameUi.GetFontSize(classesCount);
                __instance.toggleGroup.transform.position = new UnityEngine.Vector3(__instance.characterPlate.transform.position.x / 2f, __instance.toggleGroup.transform.position.y, 0);

                Models.InspectionPanelContext.SelectedHero = heroCharacter;

                CharacterInspectionScreenHandleInput.CharacterTabActive = true;
                CharacterInspectionScreenHandleInput.DisplayClassesLabel = true;

            }
        }

        [HarmonyPatch(typeof(CharacterInspectionScreen), "Unbind")]
        internal static class CharacterInspectionScreenUnbind
        {
            internal static void Postfix()
            {
                Models.InspectionPanelContext.SelectedHero = null;
            }
        }

        [HarmonyPatch(typeof(CharacterInspectionScreen), "HandleInput")]
        internal static class CharacterInspectionScreenHandleInput
        {
            internal static bool CharacterTabActive { get; set; }

            internal static bool DisplayClassesLabel { get; set; }

            internal static void Postfix(CharacterInspectionScreen __instance, InputCommands.Id command)
            {
                switch (command)
                {
                    case PLAIN_UP:
                        if (!__instance.characterInformationPanel.gameObject.activeSelf)
                        {
                            CharacterTabActive = !CharacterTabActive;
                            __instance.toggleGroup.transform.GetChild(1).gameObject.SetActive(CharacterTabActive);
                        }

                        break;

                    case PLAIN_DOWN:
                        if (Models.InspectionPanelContext.IsMulticlass)
                        {
                            if (DisplayClassesLabel)
                            {
                                __instance.characterPlate.classLabel.Text = Models.GameUi.GetAllSubclassesLabel(__instance.InspectedCharacter, __instance.characterPlate.classLabel.Text);
                            }
                            else
                            {
                                __instance.characterPlate.classLabel.Text = Models.GameUi.GetAllClassesLabel(__instance.InspectedCharacter, __instance.characterPlate.classLabel.Text);
                            }

                            DisplayClassesLabel = !DisplayClassesLabel;
                        }

                        break;

                    case PLAIN_LEFT:
                        if (__instance.characterInformationPanel.gameObject.activeSelf)
                        {
                            __instance.characterInformationPanel.RefreshNow();
                        }

                        Models.InspectionPanelContext.PickPreviousHeroClass();
                        break;

                    case PLAIN_RIGHT:
                        if (__instance.characterInformationPanel.gameObject.activeSelf)
                        {
                            __instance.characterInformationPanel.RefreshNow();
                        }

                        Models.InspectionPanelContext.PickNextHeroClass();
                        break;
                }
            }
        }
    }
}
