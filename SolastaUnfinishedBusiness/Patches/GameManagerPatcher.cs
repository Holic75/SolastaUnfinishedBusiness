using HarmonyLib;
using SolastaUnfinishedBusiness.Builders;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class GameManagerPatcher
    {
        [HarmonyPatch(typeof(GameManager), "BindPostDatabase")]
        internal static class GameManagerBindPostDatabase
        {
            internal static void Postfix()
            {
                ClassWarlock.ClassWarlockBuilder.BuildWarlockClass();

                Models.InspectionPanelContext.Load();
                Models.RealisticModeContext.Load();
                Models.SharedSpellsContext.Load();
                Models.SpellContext.Load();

                _ = ArmorProficiencyMulticlassBuilder.BarbarianArmorProficiencyMulticlass;
                _ = ArmorProficiencyMulticlassBuilder.FighterArmorProficiencyMulticlass;
                _ = ArmorProficiencyMulticlassBuilder.PaladinArmorProficiencyMulticlass;

                _ = SkillProficiencyMulticlassBuilder.PointPoolRangerSkillPointsMulticlass;
                _ = SkillProficiencyMulticlassBuilder.PointPoolRogueSkillPointsMulticlass;

                _ = RestActivityLevelDownBuilder.RestActivityLevelDown;

                Main.Enabled = true;
            }
        }
    }
}
