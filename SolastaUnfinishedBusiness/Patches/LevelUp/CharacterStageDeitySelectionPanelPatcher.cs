using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class CharacterStageDeitySelectionPanelPatcher
    {
        // disables the deity selection screen if any classes multiclass into a Cleric or if any classes except Cleric multiclasses into a Paladin
        [HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), "UpdateRelevance")]
        internal static class CharacterStageDeitySelectionPanelUpdateRelevance
        {
            internal static void Postfix(CharacterStageDeitySelectionPanel __instance)
            {
                if (Models.LevelUpContext.LevelingUp)
                {
                    __instance.isRelevant = Models.LevelUpContext.RequiresDeity;
                }
            }
        }
    }
}
