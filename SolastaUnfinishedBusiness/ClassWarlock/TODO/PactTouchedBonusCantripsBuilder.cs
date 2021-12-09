using SolastaModApi;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class PactTouchedBonusCantripsBuilder : BaseDefinitionBuilder<FeatureDefinitionBonusCantrips>
    {
        private const string PactTouchedBonusCantripsName = "ZSPactTouchedBonusCantrips";
        private static readonly string PactTouchedBonusCantripsGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, PactTouchedBonusCantripsName).ToString();

        protected PactTouchedBonusCantripsBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionBonusCantripss.BonusCantripsDomainSun, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ZSPactTouchedBonusCantripsTitle";
            Definition.GuiPresentation.Description = "Feat/&ZSPactTouchedBonusCantripsDescription";
            Definition.BonusCantrips.Clear();
            Definition.BonusCantrips.Add(EldritchBlastSpellBuilder.EldritchBlastSpell);
        }

        public static FeatureDefinitionBonusCantrips CreateAndAddToDB(string name, string guid)
        {
            return new PactTouchedBonusCantripsBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionBonusCantrips PactTouchCantrips = CreateAndAddToDB(PactTouchedBonusCantripsName, PactTouchedBonusCantripsGuid);
    }
}
