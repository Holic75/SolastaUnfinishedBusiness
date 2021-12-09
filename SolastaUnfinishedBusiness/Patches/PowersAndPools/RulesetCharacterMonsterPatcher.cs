using HarmonyLib;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class RulesetCharacterMonsterPatcher
    {
        // ensures that wildshapes get all original character pools and current powers states
        [HarmonyPatch(typeof(RulesetCharacterMonster), "FinalizeMonster")]
        internal static class RulesetCharacterMonsterRefreshAttributes
        {
            // remaining pools must be added beforehand to avoid a null pointer exception
            internal static void Prefix(RulesetCharacterMonster __instance)
            {
                if (__instance?.IsSubstitute == true)
                {
                    var party = ServiceRepository.GetService<IGameService>().Game.GameCampaign.Party;
                    var name = __instance.Name;

                    if (party.CharactersList.Find(x => x.RulesetCharacter.Name == name)?.RulesetCharacter is RulesetCharacterHero hero)
                    {
                        foreach (var attribute in hero.Attributes)
                        {
                            // transfers over all missing attributes to substitute
                            if (!__instance.Attributes.ContainsKey(attribute.Key))
                            {
                                __instance.Attributes.Add(attribute.Key, attribute.Value);
                            }
                        }
                    }
                }
            }

            // usable powers must be added afterhand to overwrite default values from game
            internal static void Postfix(RulesetCharacterMonster __instance)
            {
                if (__instance?.IsSubstitute == true)
                {
                    var party = ServiceRepository.GetService<IGameService>().Game.GameCampaign.Party;
                    var name = __instance.Name;

                    if (party.CharactersList.Find(x => x.RulesetCharacter.Name == name)?.RulesetCharacter is RulesetCharacterHero hero)
                    {
                        __instance.UsablePowers.Clear();

                        foreach (var usablePower in hero.UsablePowers)
                        {
                            __instance.UsablePowers.Add(usablePower);

                            if (usablePower.PowerDefinition == PowerBarbarianRageStart)
                            {
                                var count = hero.UsedRagePoints;

                                while (count-- > 0)
                                {
                                    __instance.SpendRagePoint();
                                }
                            }

                            __instance.RefreshUsablePower(usablePower);
                        }

                        // adds additional AC provided by Barbarian Unarmored Defense
                        if (hero.ClassesAndLevels.ContainsKey(Barbarian))
                        {
                            var conModifier = AttributeDefinitions.ComputeAbilityScoreModifier(__instance.GetAttribute("Constitution").CurrentValue);

                            __instance.GetAttribute("ArmorClass").BaseValue += conModifier;
                        }
                    }
                }
            }
        }
    }
}
