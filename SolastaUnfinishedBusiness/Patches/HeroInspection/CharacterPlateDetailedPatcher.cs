using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class CharacterPlateDetailedPatcher
    {
        [HarmonyPatch(typeof(CharacterPlateDetailed), "OnPortraitShowed")]
        internal static class CharacterPlateDetailedOnPortraitShowed
        {
            internal static void Postfix(CharacterPlateDetailed __instance)
            {
                int classesCount;
                string separator;

                if (__instance.GuiCharacter.Snapshot == null)
                {
                    separator = "\n";
                    classesCount = __instance.GuiCharacter.RulesetCharacterHero.ClassesAndLevels.Count;
                }
                else
                {
                    separator = "\\";
                    classesCount = __instance.GuiCharacter.Snapshot.Classes.Length;
                }

                __instance.classLabel.TMP_Text.fontSize = Models.GameUi.GetFontSize(classesCount);
                __instance.classLabel.Text = Models.GameUi.GetAllClassesLabel(__instance.GuiCharacter, __instance.classLabel.Text, separator);
            }
        }
    }
}
