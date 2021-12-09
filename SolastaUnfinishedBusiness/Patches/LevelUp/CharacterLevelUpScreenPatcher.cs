using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class CharacterLevelUpScreenPatcher
    {
        // add the class selection stage panel to the level up screen
        [HarmonyPatch(typeof(CharacterLevelUpScreen), "LoadStagePanels")]
        internal static class CharacterLevelUpScreenLoadStagePanels
        {
            internal static void Postfix(CharacterLevelUpScreen __instance)
            {
                var screen = Gui.GuiService.GetScreen<CharacterCreationScreen>();
                var stagePanelPrefabs = (GameObject[])AccessTools.Field(screen.GetType(), "stagePanelPrefabs").GetValue(screen);
                var classSelectionPanel = Gui.GetPrefabFromPool(stagePanelPrefabs[1], __instance.StagesPanelContainer).GetComponent<CharacterStagePanel>();
                var deitySelectionPanel = Gui.GetPrefabFromPool(stagePanelPrefabs[2], __instance.StagesPanelContainer).GetComponent<CharacterStagePanel>();
                var stagePanelsByName = new Dictionary<string, CharacterStagePanel>
                {
                    { "ClassSelection", classSelectionPanel },
                    { "LevelGains", __instance.stagePanelsByName["LevelGains"] },
                    { "DeitySelection", deitySelectionPanel },
                    { "SubclassSelection", __instance.stagePanelsByName["SubclassSelection"] },
                    { "AbilityScores", __instance.stagePanelsByName["AbilityScores"] },
                    { "FightingStyleSelection", __instance.stagePanelsByName["FightingStyleSelection"] },
                    { "ProficiencySelection", __instance.stagePanelsByName["ProficiencySelection"] },
                    { "", __instance.stagePanelsByName[""] }
                };

                __instance.stagePanelsByName = stagePanelsByName;
            }
        }

        // binds the hero
        [HarmonyPatch(typeof(CharacterLevelUpScreen), "OnBeginShow")]
        internal static class CharacterLevelUpScreenOnBeginShow
        {
            internal static void Postfix(CharacterLevelUpScreen __instance)
            {
                Models.LevelUpContext.SelectedHero = __instance.CharacterBuildingService.HeroCharacter;
            }
        }

        // unbinds the hero
        [HarmonyPatch(typeof(CharacterLevelUpScreen), "OnBeginHide")]
        internal static class CharacterLevelUpScreenOnBeginHide
        {
            internal static void Postfix(CharacterLevelUpScreen __instance)
            {
                Models.LevelUpContext.SelectedHero = null;
            }
        }

        // removes the wizard spell book in case it was granted
        [HarmonyPatch(typeof(CharacterLevelUpScreen), "DoAbort")]
        internal static class CharacterLevelUpScreenDoAbort
        {
            internal static void Prefix(CharacterLevelUpScreen __instance)
            {
                Models.LevelUpContext.UngrantItemsIfRequired();
            }
        }
    }
}
