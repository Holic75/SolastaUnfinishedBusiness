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
                if (__instance.hitDiceBox.Activated && (Models.LevelUpContext.IsMulticlass || Models.InspectionPanelContext.IsMulticlass))
                {
                    __instance.hitDiceBox.ValueLabel.Text = Models.GameUi.GetAllClassesHitDiceLabel(__instance.guiCharacter, out var dieTypeCount);

                    switch (dieTypeCount)
                    {
                        case 2:
                            __instance.hitDiceBox.ValueLabel.TMP_Text.fontSize = 16f;
                            break;

                        case 3:
                            __instance.hitDiceBox.ValueLabel.TMP_Text.fontSize = 15f;
                            break;

                        default:
                            __instance.hitDiceBox.ValueLabel.TMP_Text.fontSize = 13.5f;
                            break;
                    }
                }
            }
        }
    }
}
