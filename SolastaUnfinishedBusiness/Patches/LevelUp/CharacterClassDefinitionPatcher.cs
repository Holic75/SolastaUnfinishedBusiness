using System.Collections.Generic;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class CharacterClassDefinitionPatcher
    {
        // returns a filtered features unlocks list depending on level up context
        [HarmonyPatch(typeof(CharacterClassDefinition), "FeatureUnlocks", MethodType.Getter)]
        internal static class CharacterClassDefinitionFeatureUnlocks
        {
            internal static void Postfix(ref List<FeatureUnlockByLevel> __result)
            {
                __result = Models.LevelUpContext.SelectedClassFilteredFeaturesUnlocks(__result);
            }
        }
    }
}