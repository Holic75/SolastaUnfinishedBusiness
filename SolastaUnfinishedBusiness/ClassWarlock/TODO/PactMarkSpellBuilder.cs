using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class PactMarkSpellBuilder : BaseDefinitionBuilder<SpellDefinition>
    {
        private const string PactMarkSpellName = "ZSPactMarkSpellBuilder";
        private static readonly string PactMarkSpellNameGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, PactMarkSpellName).ToString();

        protected PactMarkSpellBuilder(string name, string guid) : base(DatabaseHelper.SpellDefinitions.HuntersMark, name, guid)
        {
            Definition.GuiPresentation.Title = "Spell/&ZSPactMarkSpellTitle";
            Definition.GuiPresentation.Description = "Spell/&ZSPactMarkSpellDescription";
            Definition.SetSpellLevel(1);
            Definition.SetSomaticComponent(true);
            Definition.SetVerboseComponent(true);
            Definition.SetSchoolOfMagic("SchoolEnchantment");
            Definition.SetMaterialComponentType(RuleDefinitions.MaterialComponentType.Mundane);
            Definition.SetCastingTime(RuleDefinitions.ActivationTime.BonusAction);

            var markedByPactEffectForm = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Condition,
                ConditionForm = new ConditionForm
                {
                    ConditionDefinition = PactMarkMarkedByPactConditionBuilder.MarkedByPactCondition
                }
            };
            markedByPactEffectForm.SetCreatedByCharacter(true);

            var pactMarkEffectForm = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Condition,
                ConditionForm = new ConditionForm
                {
                    ConditionDefinition = PactMarkPactMarkConditionBuilder.PactMarkCondition
                }
            };
            pactMarkEffectForm.ConditionForm.SetApplyToSelf(true);
            pactMarkEffectForm.SetCreatedByCharacter(true);

            var effectDescription = Definition.EffectDescription;
            effectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            effectDescription.SetRangeParameter(24);
            effectDescription.SetTargetParameter(1);
            effectDescription.EffectForms.Clear();
            effectDescription.EffectForms.Add(markedByPactEffectForm);
            effectDescription.EffectForms.Add(pactMarkEffectForm);

            Definition.SetEffectDescription(effectDescription);
        }

        public static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new PactMarkSpellBuilder(name, guid).AddToDB();
        }

        public static SpellDefinition PactMarkSpell = CreateAndAddToDB(PactMarkSpellName, PactMarkSpellNameGuid);
    }
}
