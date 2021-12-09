using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class PactTouchedMagicAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionMagicAffinity>
    {
        private const string PactTouchedSpellListName = "ZSPactTouchedSpellList";
        private static readonly string PactTouchedSpellListGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, PactTouchedSpellListName).ToString();

        protected PactTouchedMagicAffinityBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityGreenmageGreenMagicList, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ZSPactTouchedMagicAffinityTitle";
            Definition.GuiPresentation.Description = "Feat/&ZSPactTouchedMagicAffinityDescription";
            Definition.SetExtendedSpellList(PactTouchedSpellListBuilder.PactTouchedSpellList);
        }

        public static FeatureDefinitionMagicAffinity CreateAndAddToDB(string name, string guid)
        {
            return new PactTouchedMagicAffinityBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionMagicAffinity PactTouchedSpellList = CreateAndAddToDB(PactTouchedSpellListName, PactTouchedSpellListGuid);
    }
}
