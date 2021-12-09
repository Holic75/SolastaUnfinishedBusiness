using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class UsablePowerBoxPatcher
    {
        // guarantees power box slots are always white
        [HarmonyPatch(typeof(UsablePowerBox), "RefreshSlotUses")]
        internal static class UsablePowerBoxRefreshSlotUses
        {
            internal static void Postfix(UsablePowerBox __instance)
            {
                for (var i = 0; i < __instance.slotStatusTable.childCount; i++)
                {
                    var child = __instance.slotStatusTable.GetChild(i);
                    var component = child.GetComponent<SlotStatus>();

                    component.Available.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                }
            }
        }
    }
}
