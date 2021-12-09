using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class FlexibleCastingItemPatcher
    {
        // creates different slots colors and pop up messages depending on slot types
        [HarmonyPatch(typeof(FlexibleCastingItem), "Bind")]
        internal static class FlexibleCastingItemBind
        {
            internal static bool Prefix(
                FlexibleCastingItem __instance,
                int slotLevel,
                int remainingSlots,
                int maxSlots,
                int availableSorceryPoints,
                int sorceryAmount,
                bool available,
                bool createSlot,
                string failure,
                FlexibleCastingItem.SlotSelectedHandler slotSelected)
            {
                __instance.refreshing = true;
                __instance.SlotLevel = slotLevel;
                __instance.spellLevelLabel.Text = Gui.ToRoman(slotLevel);
                __instance.SlotSelected = slotSelected;
                __instance.toggle.isOn = false;
                __instance.toggle.interactable = available;
                __instance.sorceryPointsValue.Text = sorceryAmount.ToString();
                __instance.convertTooltip.Content = available ? string.Empty : Gui.FormatFailure(string.Empty, failure, false);
                while (__instance.slotStatusTable.childCount < maxSlots)
                {
                    Gui.GetPrefabFromPool(__instance.slotStatusPrefab, __instance.slotStatusTable);
                }

                for (var index = 0; index < maxSlots; ++index)
                {
                    var component = __instance.slotStatusTable.GetChild(index).GetComponent<SlotStatus>();
                    if (index < remainingSlots)
                    {
                        component.Available.gameObject.SetActive(true);
                        component.Used.gameObject.SetActive(false);
                    }
                    else
                    {
                        component.Available.gameObject.SetActive(false);
                        component.Used.gameObject.SetActive(true);
                    }

                    // PATCH
                    var heroWithSpellRepertoire = RulesetImplementationManagerPatcher.HeroWithSpellRepertoire;
                    var spellRepertoire = RulesetImplementationManagerPatcher.SpellRepertoire;
                    var shortRestSlotsCount = Models.SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);
                    var longRestSlotsCount = maxSlots - shortRestSlotsCount;
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
                            component.Used.gameObject.SetActive(index >= remainingSlots - shortRestSlotsRemainingCount);
                            component.Available.gameObject.SetActive(index < remainingSlots - shortRestSlotsRemainingCount);
                        }
                        else
                        {
                            component.Used.gameObject.SetActive(index - longRestSlotsCount >= shortRestSlotsRemainingCount);
                            component.Available.gameObject.SetActive(index - longRestSlotsCount < shortRestSlotsRemainingCount);
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

                    component.ScaleModifier.ResetModifier(true);
                }
                if (createSlot)
                {
                    __instance.sorceryImage.material = null;
                    __instance.slotConversionGroup.SetSiblingIndex(2);
                    __instance.sorceryConversionGroup.SetSiblingIndex(0);
                }
                else
                {
                    __instance.sorceryImage.material = sorceryAmount > availableSorceryPoints ? __instance.disableMaterial : null;
                    __instance.slotConversionGroup.SetSiblingIndex(0);
                    __instance.sorceryConversionGroup.SetSiblingIndex(2);
                }
                __instance.slotsModifier.ResetModifier(true);
                __instance.refreshing = false;
                return false;
            }
        }
    }
}
