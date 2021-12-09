using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches
{
    public static class CharacterReactionItemPatcher
    {
        // ensures reaction panel considers the original character instead of the substitute
        [HarmonyPatch(typeof(CharacterReactionItem), "Bind")]
        public static class CharacterReactionItemBind
        {
            public static RulesetCharacter MyRulesetCharacter(RulesetCharacter rulesetCharacter)
            {
                if (rulesetCharacter is RulesetCharacterMonster monster && monster.IsSubstitute)
                {
                    var name = rulesetCharacter.Name;
                    var party = ServiceRepository.GetService<IGameService>().Game.GameCampaign.Party;
                    var hero = party.CharactersList.Find(x => x.RulesetCharacter.Name == name)?.RulesetCharacter;

                    return hero ?? rulesetCharacter;
                }
                else
                {
                    return rulesetCharacter;
                }
            }

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var spellRepertoiresMethod = typeof(RulesetCharacter).GetMethod("get_SpellRepertoires");
                var myRulesetCharacterMethod = typeof(CharacterReactionItemBind).GetMethod("MyRulesetCharacter");

                foreach (var instruction in instructions)
                {
                    // at this point RulesetCharacter is on stack so I call MyRulesetCharacter to determine if it needs to be swaped with original hero
                    if (instruction.Calls(spellRepertoiresMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Call, myRulesetCharacterMethod);
                    }

                    yield return instruction;
                }
            }
        }
    }
}
