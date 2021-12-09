using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class CharacterReactionSubitemPatcher
    {
        // creates different slots colors and pop up messages depending on slot types
        [HarmonyPatch(typeof(CharacterReactionSubitem), "Bind")]
        internal static class CharacterReactionSubitemBind
        {
            public static bool Prefix(
                CharacterReactionSubitem __instance,
                RulesetSpellRepertoire spellRepertoire,
                int slotLevel,
                string text,
                bool interactable,
                CharacterReactionSubitem.SubitemSelectedHandler subitemSelected)
            {
                __instance.label.Text = text;
                __instance.toggle.interactable = interactable;
                __instance.canvasGroup.interactable = interactable;
                __instance.SubitemSelected = subitemSelected;
                spellRepertoire.GetSlotsNumber(slotLevel, out var totalSlotsRemainingCount, out var totalSlotsCount);
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

                while (__instance.slotStatusTable.childCount < totalSlotsCount)
                {
                    Gui.GetPrefabFromPool(__instance.slotStatusPrefab, __instance.slotStatusTable);
                }

                for (var index = 0; index < totalSlotsCount; ++index)
                {
                    var child = __instance.slotStatusTable.GetChild(index);
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
                        shortRestSlotsUsedCount = Math.Min(shortRestSlotsUsedCount, shortRestSlotsCount);
                    }

                    var shortRestSlotsRemainingCount = shortRestSlotsCount - shortRestSlotsUsedCount;

                    if (Models.SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire) && Models.SharedSpellsContext.IsEnabled && Models.SharedSpellsContext.IsCombined && slotLevel <= warlockSpellLevel)
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

                    if (Models.SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire) && Models.SharedSpellsContext.IsEnabled)
                    {
                        if (Models.SharedSpellsContext.IsCombined)
                        {
                            if (index >= longRestSlotsCount && slotLevel <= warlockSpellLevel)
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
                for (var index = totalSlotsCount; index < __instance.slotStatusTable.childCount; ++index)
                {
                    __instance.slotStatusTable.GetChild(index).gameObject.SetActive(false);
                }

                __instance.slotStatusTable.GetComponent<GuiTooltip>().Content = str;

                return false;
            }
        }
    }
}
