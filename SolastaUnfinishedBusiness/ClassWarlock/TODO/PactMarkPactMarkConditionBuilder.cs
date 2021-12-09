using SolastaModApi;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class PactMarkPactMarkConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
    {
        private const string PactMarkPactMarkConditionName = "ZSPactMarkPactMarkCondition";
        private static readonly string PactMarkPactMarkConditionGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, PactMarkPactMarkConditionName).ToString();

        protected PactMarkPactMarkConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionHuntersMark, name, guid)
        {
            Definition.GuiPresentation.Title = "Spell/&ZSPactMarkPactMarkConditionTitle";
            Definition.GuiPresentation.Description = "Spell/&ZSPactMarkPactMarkConditionDescription";
            Definition.Features.Clear();
            Definition.Features.Add(PactMarkAdditionalDamageBuilder.PactMarkAdditionalDamage);
        }

        public static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new PactMarkPactMarkConditionBuilder(name, guid).AddToDB();
        }

        public static ConditionDefinition PactMarkCondition = CreateAndAddToDB(PactMarkPactMarkConditionName, PactMarkPactMarkConditionGuid);
    }
}
