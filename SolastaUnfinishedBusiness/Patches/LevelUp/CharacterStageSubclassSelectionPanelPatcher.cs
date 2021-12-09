using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class CharacterStageSubclassSelectionPanelPatcher
    {
        // disables the sub class selection screen if the deity screen was enabled
        [HarmonyPatch(typeof(CharacterStageSubclassSelectionPanel), "UpdateRelevance")]
        internal static class CharacterStageSubclassSelectionPanelUpdateRelevance
        {
            internal static void Postfix(CharacterStageSubclassSelectionPanel __instance)
            {
                if (Models.LevelUpContext.LevelingUp && Models.LevelUpContext.RequiresDeity)
                {
                    __instance.isRelevant = false;
                }
            }
        }
    }
}
