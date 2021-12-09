using SolastaModApi;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class PactTouchedAutoPreparedSpellsBuilder : BaseDefinitionBuilder<FeatureDefinitionAutoPreparedSpells>
    {
        private const string PactTouchedAutoPreparedSpellsName = "ZSPactTouchedAutoPreparedSpells";
        private static readonly string PactTouchedAutoPreparedSpellsNameGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, PactTouchedAutoPreparedSpellsName).ToString();

        protected PactTouchedAutoPreparedSpellsBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAutoPreparedSpellss.AutoPreparedSpellsDomainBattle, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ZSPactTouchedAutoPreparedSpellsTitle";
            Definition.GuiPresentation.Description = "Feat/&ZSPactTouchedAutoPreparedSpellsDescription";
        }

        public static FeatureDefinitionAutoPreparedSpells CreateAndAddToDB(string name, string guid)
        {
            return new PactTouchedAutoPreparedSpellsBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionAutoPreparedSpells PactTouchedAutoPreparedSpells = CreateAndAddToDB(PactTouchedAutoPreparedSpellsName, PactTouchedAutoPreparedSpellsNameGuid);
    }
}
