using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class PactMarkAdditionalDamageBuilder : BaseDefinitionBuilder<FeatureDefinitionAdditionalDamage>
    {
        private const string PactMarkAdditionalDamageBuilderName = "ZSPactMarkAdditionalDamage";
        private static readonly string PactMarkAdditionalDamageGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, PactMarkAdditionalDamageBuilderName).ToString();

        protected PactMarkAdditionalDamageBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageHuntersMark, name, guid)
        {
            Definition.GuiPresentation.Title = "Spell/&ZSPactMarkAdditionalDamageTitle";
            Definition.GuiPresentation.Description = "Spell/&ZSPactMarkAdditionalDamageDescription";
            Definition.SetAttackModeOnly(false);
            Definition.SetRequiredTargetCondition(PactMarkMarkedByPactConditionBuilder.MarkedByPactCondition);
            Definition.SetNotificationTag("PactMarked");
        }

        public static FeatureDefinitionAdditionalDamage CreateAndAddToDB(string name, string guid)
        {
            return new PactMarkAdditionalDamageBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionAdditionalDamage PactMarkAdditionalDamage = CreateAndAddToDB(PactMarkAdditionalDamageBuilderName, PactMarkAdditionalDamageGuid);
    }
}
