using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches
{
    // use these patches to enable the Level Down button in the Heroes Pool
    internal static class CharactersPanelPatcher
    {
        [HarmonyPatch(typeof(CharactersPanel), "Refresh")]
        internal static class CharactersPanelRefresh
        {
            internal static void Postfix(CharactersPanel __instance)
            {
                var characterLevel = (__instance.selectedPlate >= 0) ? __instance.characterPlates[__instance.selectedPlate].GuiCharacter.CharacterLevel : 1;

                __instance.exportPdfButton.gameObject.SetActive(characterLevel > 1);
            }
        }

        [HarmonyPatch(typeof(CharactersPanel), "OnExportPdfCb")]
        internal static class CharactersPanelOnExportPdfCb
        {
            internal static bool Prefix(CharactersPanel __instance)
            {
                Builders.FunctorLevelDown.ConfirmAndExecute(__instance.characterPlates[__instance.selectedPlate].Filename);

                return false;
            }
        }
    }
}
