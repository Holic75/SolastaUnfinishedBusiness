using HarmonyLib;
using SolastaModApi;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class RulesetActorPatcher
    {
        [HarmonyPatch(typeof(RulesetActor), "RefreshAttributes")]
        internal static class RulesetActorRefreshAttributes
        {
            internal static void Postfix(RulesetActor __instance)
            {
                if (__instance is RulesetCharacterHero hero)
                {
                    // fixes the Paladin pool to use the class level instead
                    if (hero.ClassesAndLevels.ContainsKey(Paladin))
                    {
                        var healingPoolAttribute = hero.GetAttribute("HealingPool", true);

                        if (healingPoolAttribute != null)
                        {
                            foreach (var activeModifier in healingPoolAttribute.ActiveModifiers)
                            {
                                if (activeModifier.Operation != FeatureDefinitionAttributeModifier.AttributeModifierOperation.MultiplyByCharacterLevel &&
                                    activeModifier.Operation != FeatureDefinitionAttributeModifier.AttributeModifierOperation.MultiplyByClassLevel)
                                {
                                    continue;
                                }

                                activeModifier.Value = hero.ClassesAndLevels[DatabaseHelper.CharacterClassDefinitions.Paladin];
                            }

                            healingPoolAttribute.Refresh();
                        }
                    }

                    // fixes the Sorcerer pool to use the class level instead
                    if (hero.ClassesAndLevels.ContainsKey(Sorcerer))
                    {
                        var sorceryPointsAttributes = hero.GetAttribute("SorceryPoints", true);

                        if (sorceryPointsAttributes != null)
                        {
                            foreach (var activeModifier in sorceryPointsAttributes.ActiveModifiers)
                            {
                                if (activeModifier.Operation != FeatureDefinitionAttributeModifier.AttributeModifierOperation.MultiplyByCharacterLevel &&
                                    activeModifier.Operation != FeatureDefinitionAttributeModifier.AttributeModifierOperation.MultiplyByClassLevel)
                                {
                                    continue;
                                }
                                activeModifier.Value = hero.ClassesAndLevels[Sorcerer];
                            }

                            sorceryPointsAttributes.Refresh();
                        }
                    }
                }
            }
        }
    }
}
