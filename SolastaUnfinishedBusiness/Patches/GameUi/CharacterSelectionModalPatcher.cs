using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class CharacterSelectionModalPatcher
    {
        // ensures MC heroes are correctly offered on adventures with min/max caps on character level
        [HarmonyPatch(typeof(CharacterSelectionModal), "EnumeratePlates")]
        internal static class CharacterSelectionModalEnumeratePlates
        {
            public static int MyLevel(int[] levels)
            {
                return levels.Sum();
            }

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var bypass = 0;
                var MyLevelMethod = typeof(CharacterSelectionModalEnumeratePlates).GetMethod("MyLevel");
                var levelsField = typeof(RulesetCharacterHero.Snapshot).GetField("Levels");

                foreach (var instruction in instructions)
                {
                    if (bypass > 0)
                    {
                        bypass--;
                        continue;
                    }

                    yield return instruction;

                    if (instruction.LoadsField(levelsField))
                    {
                        yield return new CodeInstruction(OpCodes.Call, MyLevelMethod);
                        bypass = 2;
                    }
                }
            }
        }
    }
}
