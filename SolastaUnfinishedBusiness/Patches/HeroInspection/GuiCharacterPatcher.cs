using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class GuiCharacterPatcher
    {
        [HarmonyPatch(typeof(GuiCharacter), "MainClassDefinition", MethodType.Getter)]
        internal static class GuiCharacterMainClassDefinition
        {
            internal static void Postfix(ref CharacterClassDefinition __result)
            {
                __result = Models.InspectionPanelContext.SelectedClass ?? __result; // this is also called on level up reason why coalescing to default __result
            }
        }

        [HarmonyPatch(typeof(GuiCharacter), "LevelAndClassAndSubclass", MethodType.Getter)]
        internal static class GuiCharacterLevelAndClassAndSubclass
        {
            internal static void Postfix(GuiCharacter __instance, ref string __result)
            {
                __result = Models.GameUi.GetAllClassesLabel(__instance, " - ") ?? __result;
            }
        }

        [HarmonyPatch(typeof(GuiCharacter), "ClassAndLevel", MethodType.Getter)]
        internal static class GuiCharacterClassAndLevel
        {
            internal static void Postfix(GuiCharacter __instance, ref string __result)
            {
                __result = Models.GameUi.GetAllClassesLabel(__instance, " - ") ?? __result;
            }
        }

        [HarmonyPatch(typeof(GuiCharacter), "LevelAndExperienceTooltip", MethodType.Getter)]
        internal static class GuiCharacterLevelAndExperienceTooltip
        {
            internal static bool Prefix(GuiCharacter __instance, ref string __result)
            {
                __result = Models.GameUi.GetLevelAndExperienceTooltip(__instance);

                return false;
            }
        }
    }
}
