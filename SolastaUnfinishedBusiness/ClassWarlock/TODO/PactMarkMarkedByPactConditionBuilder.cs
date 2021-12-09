using SolastaModApi;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class PactMarkMarkedByPactConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
    {
        private const string PactMarkMarkedByPactConditionName = "ZSPactMarkMarkedByPactCondition";
        private static readonly string PactMarkMarkedByPactConditionGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, PactMarkMarkedByPactConditionName).ToString();

        protected PactMarkMarkedByPactConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionMarkedByHunter, name, guid)
        {
            Definition.GuiPresentation.Title = "Spell/&ZSPactMarkMarkedByPactConditionTitle";
            Definition.GuiPresentation.Description = "Spell/&ZSPactMarkMarkedByPactConditionDescription";
        }

        public static ConditionDefinition CreateAndAddToDB(string name, string guid)
        {
            return new PactMarkMarkedByPactConditionBuilder(name, guid).AddToDB();
        }

        public static ConditionDefinition MarkedByPactCondition = CreateAndAddToDB(PactMarkMarkedByPactConditionName, PactMarkMarkedByPactConditionGuid);
    }
}
