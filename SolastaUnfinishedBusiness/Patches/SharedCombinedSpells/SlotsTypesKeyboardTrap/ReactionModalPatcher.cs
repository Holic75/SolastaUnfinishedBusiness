using HarmonyLib;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class ReactionModalPatcher
    {
        // traps if SHIFT is pressed to determine which slot type to consume
        [HarmonyPatch(typeof(ReactionModal), "OnReact")]
        internal static class ReactionModalOnReact
        {
            internal static void Prefix()
            {
                Models.SharedSpellsContext.ForceLongRestSlot = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            }
        }
    }
}