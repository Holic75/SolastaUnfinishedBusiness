using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches
{
    // these patches enforces cantrips to be cast at character level
    internal static class RulesetEffectSpellPatcher
    {
        internal static int CasterLevel { get; set; }

        [HarmonyPatch(typeof(RulesetEffectSpell), "ComputeTargetParameter")]
        internal static class RulesetEffectSpellComputeTargetParameter
        {
            internal static void Prefix(RulesetEffectSpell __instance)
            {
                if (__instance != null)
                {
                    var rulesetCharacter = __instance.Caster;
                    var rulesetSpellRepertoire = __instance.SpellRepertoire;
                    var spellDefinition = __instance.SpellDefinition;

                    if (Main.Settings.EnableCantripsAtCharacterLevel &&
                        rulesetCharacter != null && rulesetCharacter is RulesetCharacterHero rulesetCharacterHero &&
                        rulesetSpellRepertoire != null && spellDefinition != null && rulesetSpellRepertoire.KnownCantrips.Contains(spellDefinition))
                    {
                        CasterLevel = rulesetCharacterHero.ClassesHistory.Count;
                    }
                    else
                    {
                        CasterLevel = 0;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(RulesetEffectSpell), "GetClassLevel")]
        internal static class RulesetEffectSpellGetClassLevel
        {
            internal static void Postfix(ref int __result)
            {
                if (CasterLevel > 0)
                {
                    __result = CasterLevel;
                }
            }
        }
    }
}
