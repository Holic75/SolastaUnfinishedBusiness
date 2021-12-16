using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches.LevelUp
{
    class CharacterSubclassDefinitionPatcher
    {
        // returns a filtered features unlocks list depending on level up context
        [HarmonyPatch(typeof(CharacterSubclassDefinition), "FeatureUnlocks", MethodType.Getter)]
        internal static class CharacterSubclassDefinitionFeatureUnlocks
        {
            internal static void Postfix(CharacterSubclassDefinition __instance, ref List<FeatureUnlockByLevel> __result)
            {
                __result = Models.LevelUpContext.SelectedClassFilteredFeaturesUnlocks(__result, __instance);
            }
        }
    }
}
