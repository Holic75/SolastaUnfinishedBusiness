using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class SlotStatusTablePatcher
    {
        // creates different slots colors and pop up messages depending on slot types
        [HarmonyPatch(typeof(SlotStatusTable), "Bind")]
        internal static class SlotStatusTableBind
        {
            public static bool Prefix(
                SlotStatusTable __instance,
                RulesetSpellRepertoire spellRepertoire,
                int spellLevel,
                List<SpellDefinition> spells,
                bool compactIfNeeded = true)
            {
                __instance.infinitySymbol.gameObject.SetActive(false);
                __instance.table.gameObject.SetActive(false);
                __instance.slotsText.gameObject.SetActive(false);

                if (spells != null && spells.Count == 1 && spellLevel == 0)
                {
                    __instance.levelLabel.gameObject.SetActive(false);
                }
                else if (__instance.cantripLabel != null)
                {
                    __instance.levelLabel.gameObject.SetActive(spellLevel > 0);
                    __instance.cantripLabel.gameObject.SetActive(spellLevel == 0);

                    if (spellLevel > 0)
                    {
                        __instance.levelLabel.Text = Gui.LocalizeSpellLevel(spellLevel);
                    }
                }
                else
                {
                    __instance.levelLabel.Text = Gui.LocalizeSpellLevel(spellLevel);
                }

                if (spellRepertoire != null)
                {
                    if (__instance.slotsTitle != null)
                    {
                        __instance.slotsTitle.Text = spellLevel == 0 ? "Screen/&SpellsGroupCantripsTitle" : "Screen/&SpellsGroupSlotsTitle";
                    }

                    if (spellLevel == 0)
                    {
                        __instance.infinitySymbol.gameObject.SetActive(true);
                    }
                    else
                    {
                        spellRepertoire.GetSlotsNumber(spellLevel, out var totalSlotsRemainingCount, out var totalSlotsCount);

                        string str;

                        if (totalSlotsRemainingCount == 0)
                        {
                            str = "Screen/&SpellSlotsUsedAllDescription";
                        }
                        else if (totalSlotsRemainingCount == totalSlotsCount)
                        {
                            str = "Screen/&SpellSlotsUsedNoneDescription";
                        }
                        else
                        {
                            str = Gui.Format("Screen/&SpellSlotsUsedSomeDescription", (totalSlotsCount - totalSlotsRemainingCount).ToString());
                        }

                        if (spells?.Count > 1 || totalSlotsCount == 1 || !compactIfNeeded)
                        {
                            __instance.table.gameObject.SetActive(true);

                            while (__instance.table.childCount < totalSlotsCount)
                            {
                                Gui.GetPrefabFromPool(__instance.slotStatusPrefab, __instance.table);
                            }

                            for (var index = 0; index < totalSlotsCount; ++index)
                            {
                                var child = __instance.table.GetChild(index);

                                child.gameObject.SetActive(true);

                                var component = child.GetComponent<SlotStatus>();

                                component.Used.gameObject.SetActive(index >= totalSlotsRemainingCount);
                                component.Available.gameObject.SetActive(index < totalSlotsRemainingCount);

                                // PATCH
                                if (spellRepertoire?.CharacterName == null)
                                {
                                    continue;
                                }

                                var heroWithSpellRepertoire = Models.SharedSpellsContext.GetHero(spellRepertoire?.CharacterName);
                                var shortRestSlotsCount = Models.SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);
                                var longRestSlotsCount = totalSlotsCount - shortRestSlotsCount;
                                var shortRestSlotsUsedCount = 0;
                                var warlockSpellRepertoire = Models.SharedSpellsContext.GetWarlockSpellRepertoire(heroWithSpellRepertoire);
                                var warlockSpellLevel = Models.SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);

                                if (warlockSpellRepertoire != null)
                                {
                                    var usedSpellsSlots = (Dictionary<int, int>)AccessTools.Field(typeof(RulesetSpellRepertoire), "usedSpellsSlots").GetValue(warlockSpellRepertoire);

                                    usedSpellsSlots.TryGetValue(-1, out shortRestSlotsUsedCount);
                                }

                                var shortRestSlotsRemainingCount = shortRestSlotsCount - shortRestSlotsUsedCount;
                                var longRestSlotsRemainingCount = totalSlotsRemainingCount - shortRestSlotsRemainingCount;
                                var longRestSlotsUsedCount = longRestSlotsCount - longRestSlotsRemainingCount;

                                if (Models.SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire) && Models.SharedSpellsContext.IsEnabled && Models.SharedSpellsContext.IsCombined)
                                {
                                    if (totalSlotsRemainingCount == 0)
                                    {
                                        str = "Screen/&SpellSlotsUsedAllDescription";
                                    }
                                    else if (totalSlotsRemainingCount == totalSlotsCount)
                                    {
                                        str = "Screen/&SpellSlotsUsedNoneDescription";
                                    }
                                    else if (shortRestSlotsRemainingCount == shortRestSlotsCount)
                                    {
                                        str = Gui.Format("Screen/&SpellSlotsUsedLongDescription", longRestSlotsUsedCount.ToString());
                                    }
                                    else if (longRestSlotsRemainingCount == longRestSlotsCount)
                                    {
                                        str = Gui.Format("Screen/&SpellSlotsUsedShortDescription", shortRestSlotsUsedCount.ToString());
                                    }
                                    else
                                    {
                                        str = Gui.Format("Screen/&SpellSlotsUsedShortLongDescription", shortRestSlotsUsedCount.ToString(), longRestSlotsUsedCount.ToString());
                                    }

                                    if (spellLevel <= warlockSpellLevel)
                                    {
                                        if (index < longRestSlotsCount)
                                        {
                                            component.Used.gameObject.SetActive(index >= totalSlotsRemainingCount - shortRestSlotsRemainingCount);
                                            component.Available.gameObject.SetActive(index < totalSlotsRemainingCount - shortRestSlotsRemainingCount);
                                        }
                                        else
                                        {
                                            component.Used.gameObject.SetActive(index >= longRestSlotsCount + shortRestSlotsRemainingCount);
                                            component.Available.gameObject.SetActive(index < longRestSlotsCount + shortRestSlotsRemainingCount);
                                        }
                                    }
                                }

                                if (Models.SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire) && Models.SharedSpellsContext.IsEnabled)
                                {
                                    if (Models.SharedSpellsContext.IsCombined)
                                    {
                                        if (index >= longRestSlotsCount && spellLevel <= warlockSpellLevel)
                                        {
                                            component.Available.GetComponent<Image>().color = new Color(0f, 1f, 0f, 1f);
                                        }
                                        else
                                        {
                                            component.Available.GetComponent<Image>().color = new Color(0f, 0.5f, 0f, 1f);
                                        }
                                    }
                                    else
                                    {
                                        if (Models.SharedSpellsContext.IsWarlock(spellRepertoire?.SpellCastingClass))
                                        {
                                            component.Available.GetComponent<Image>().color = new Color(0f, 1f, 0f, 1f);
                                        }
                                        else
                                        {
                                            component.Available.GetComponent<Image>().color = new Color(0f, 0.5f, 0f, 1f);
                                        }
                                    }
                                }
                                else
                                {
                                    component.Available.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                                }
                                // END PATCH
                            }
                            for (var index = totalSlotsCount; index < __instance.table.childCount; ++index)
                            {
                                __instance.table.GetChild(index).gameObject.SetActive(false);
                            }

                            __instance.table.GetComponent<GuiTooltip>().Content = str;
                        }
                        else
                        {
                            if (spells?.Count == 1)
                            {
                                __instance.slotsText.gameObject.SetActive(true);
                                __instance.slotsText.Text = Gui.FormatCurrentOverMax(totalSlotsRemainingCount, totalSlotsCount);
                            }
                        }
                    }
                }
                else
                {
                    __instance.infinitySymbol.gameObject.SetActive(false);
                    __instance.table.gameObject.SetActive(false);
                    __instance.slotsTitle?.gameObject.SetActive(false);
                }

                return false;
            }
        }
    }
}
