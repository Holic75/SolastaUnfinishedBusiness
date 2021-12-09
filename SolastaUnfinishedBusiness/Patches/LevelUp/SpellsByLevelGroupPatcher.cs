using System.Collections.Generic;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class SpellsByLevelGroupPatcher
    {
        // filters how spells are displayed during level up
        [HarmonyPatch(typeof(SpellsByLevelGroup), "CommonBind")]
        internal static class SpellsByLevelGroupCommonBind
        {
            internal static void Prefix(SpellBox.BindMode bindMode, List<SpellDefinition> allSpells)
            {
                if (Models.LevelUpContext.LevelingUp && Models.LevelUpContext.IsMulticlass)
                {
                    if (bindMode == SpellBox.BindMode.Learning && !Main.Settings.EnableDisplayAllKnownSpellsOnLevelUp)
                    {
                        allSpells.RemoveAll(s => !Models.SpellContext.IsSpellOfferedBySelectedClassSubclass(s));
                    }
                    else if (bindMode == SpellBox.BindMode.Unlearn)
                    {
                        allSpells.RemoveAll(s => !Models.SpellContext.IsSpellOfferedBySelectedClassSubclass(s) || !Models.SpellContext.IsSpellKnownBySelectedClassSubclass(s));
                    }
                }
            }
        }
    }
}
