using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class FeatureDefinitionCastSpellPatcher
    {
        [HarmonyPatch(typeof(FeatureDefinitionCastSpell), "ComputeHighestSpellLevel")]
        internal static class FeatureDefinitionCastSpellComputeHighestSpellLevel
        {
            internal static bool Prefix(ref int __result)
            {
                if (Models.LevelUpContext.LevelingUp && Models.LevelUpContext.IsMulticlass)
                {
                    __result = Models.SharedSpellsContext.GetClassSpellLevel(Models.LevelUpContext.SelectedHero, Models.LevelUpContext.SelectedClass, Models.LevelUpContext.SelectedSubclass);

                    return false;
                }

                return true;
            }
        }
    }
}
