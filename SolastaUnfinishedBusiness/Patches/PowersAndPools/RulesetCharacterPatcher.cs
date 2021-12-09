using HarmonyLib;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class RulesetCharacterPatcher
    {
        // ensures that original character power pools are in sync with substitute 
        [HarmonyPatch(typeof(RulesetCharacter), "UsePower")]
        internal static class RulesetCharacterUsePower
        {
            internal static void Prefix(RulesetCharacter __instance, RulesetUsablePower usablePower)
            {
                if (__instance is RulesetCharacterMonster monster && monster.IsSubstitute && usablePower.PowerDefinition == PowerBarbarianRageStart)
                {
                    var name = __instance.Name;
                    var party = ServiceRepository.GetService<IGameService>().Game.GameCampaign.Party;
                    var hero = party.CharactersList.Find(x => x.RulesetCharacter.Name == name)?.RulesetCharacter;

                    (hero ?? monster).SpendRagePoint();
                }
            }
        }
    }
}
