using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches
{
    public static class RulesetCharacterHeroPatcher
    {
        // use this patch to enable additional after rest actions
        [HarmonyPatch(typeof(RulesetCharacterHero), "EnumerateAfterRestActions")]
        internal static class RulesetCharacterHeroEnumerateAfterRestActions
        {
            internal static void Postfix(RulesetCharacterHero __instance)
            {
                foreach (var restActivityDefinition in DatabaseRepository.GetDatabase<RestActivityDefinition>())
                {
                    if (restActivityDefinition.Condition == Settings.ActivityConditionCanLevelDown)
                    {
                        var characterLevel = __instance.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;

                        if (characterLevel > 1)
                        {
                            __instance.afterRestActions.Add(restActivityDefinition);
                        }
                    }
                }
            }
        }
    }
}
