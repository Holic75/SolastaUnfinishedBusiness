using HarmonyLib;
using UnityEngine.UI;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class SpellRepertoirePanelPatcher
    {
        // filters how spells and slots are displayed on inspection
        [HarmonyPatch(typeof(SpellRepertoirePanel), "Bind")]
        internal static class SpellRepertoirePanelBind
        {
            internal static void Postfix(SpellRepertoirePanel __instance)
            {
                var heroWithSpellRepertoire = __instance.GuiCharacter.RulesetCharacterHero;
                var characterClassDefinition = __instance.SpellRepertoire.SpellCastingClass;
                var characterSubClassDefinition = __instance.SpellRepertoire.SpellCastingSubclass;

                // determines the display context
                int slotLevel;
                var classSpellLevel = Models.SharedSpellsContext.GetClassSpellLevel(heroWithSpellRepertoire, characterClassDefinition, characterSubClassDefinition);

                if (Models.SharedSpellsContext.IsEnabled)
                {
                    if (Models.SharedSpellsContext.IsCombined)
                    {
                        slotLevel = Models.SharedSpellsContext.GetCombinedSpellLevel(heroWithSpellRepertoire);
                    }
                    else if (Models.SharedSpellsContext.IsWarlock(characterClassDefinition))
                    {
                        slotLevel = Models.SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);
                    }
                    else
                    {
                        slotLevel = Models.SharedSpellsContext.GetSharedSpellLevel(heroWithSpellRepertoire);
                    }
                }
                else
                {
                    slotLevel = classSpellLevel;
                }

                // patches the spell level buttons to be hidden if no spells available at that level
                for (var i = 1; i < __instance.levelButtonsTable.childCount; i++)
                {
                    var child = __instance.levelButtonsTable.GetChild(i);

                    child.gameObject.SetActive(i <= classSpellLevel);
                }

                // patches the panel to display higher level spell slots from shared slots table but hide the spell panels if class level not there yet
                for (var i = 1; i < __instance.spellsByLevelTable.childCount; i++)
                {
                    var spellsByLevel = __instance.spellsByLevelTable.GetChild(i);

                    for (var j = 0; j < spellsByLevel.childCount; j++)
                    {
                        var transform = spellsByLevel.GetChild(j);

                        if (transform.TryGetComponent(typeof(SlotStatusTable), out var _))
                        {
                            transform.gameObject.SetActive(i <= slotLevel);
                        }
                        else
                        {
                            transform.gameObject.SetActive(i <= classSpellLevel);
                        }
                    }
                }

                // determines if the sorcery points UI needs to be hidden
                var hasSorceryPoints = characterClassDefinition == Sorcerer && heroWithSpellRepertoire.ClassesAndLevels.ContainsKey(Sorcerer) && heroWithSpellRepertoire.ClassesAndLevels[Sorcerer] >= 2;

                __instance.sorceryPointsBox.gameObject.SetActive(hasSorceryPoints);
                __instance.sorceryPointsLabel.gameObject.SetActive(hasSorceryPoints);

                LayoutRebuilder.ForceRebuildLayoutImmediate(__instance.spellsByLevelTable);
            }
        }
    }
}
