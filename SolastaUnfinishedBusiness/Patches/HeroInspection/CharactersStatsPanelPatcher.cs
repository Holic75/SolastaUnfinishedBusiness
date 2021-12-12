using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class CharactersStatsPanelPatcher
    {
        [HarmonyPatch(typeof(CharacterStatsPanel), "Refresh")]
        internal static class CharacterStatsPanelRefresh
        {
            internal static void Postfix(CharacterStatsPanel __instance)
            {
                if (__instance.hitDiceBox.Activated)
                {
                    __instance.hitDiceBox.ValueLabel.Text = Models.GameUi.GetAllClassesHitDiceLabel(__instance.guiCharacter, out var dieTypeCount);
                    __instance.hitDiceBox.ValueLabel.TMP_Text.fontSize = Models.GameUi.GetFontSize(dieTypeCount);
                }
            }
        }
    }
}
