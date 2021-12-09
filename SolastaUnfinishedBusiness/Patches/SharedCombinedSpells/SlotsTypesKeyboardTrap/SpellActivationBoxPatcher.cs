using HarmonyLib;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class SpellActivationBoxPatcher
    {
        // traps if SHIFT is pressed to determine which slot type to consume
        [HarmonyPatch(typeof(SpellActivationBox), "OnActivateCb")]
        internal static class SpellActivationBoxOnActivateCb
        {
            internal static void Prefix()
            {
                Models.SharedSpellsContext.ForceLongRestSlot = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            }
        }
    }
}