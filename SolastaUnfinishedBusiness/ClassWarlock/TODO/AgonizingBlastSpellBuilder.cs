using SolastaModApi;
namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class AgonizingBlastSpellBuilder : BaseDefinitionBuilder<SpellDefinition>
    {
        private const string AgonizingBlastSpellName = "ZSAgonizingBlastSpell";
        private static readonly string AgonizingBlastSpellNameGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, AgonizingBlastSpellName).ToString();

        protected AgonizingBlastSpellBuilder(string name, string guid) : base(EldritchBlastSpellBuilder.EldritchBlastSpell, name, guid)
        {
            Definition.GuiPresentation.Title = "Spell/&ZSAgonizingBlastSpellTitle";
            Definition.GuiPresentation.Description = "Spell/&ZSAgonizingBlastSpellDescription";
            Definition.EffectDescription.EffectForms.Find(ef => ef.DamageForm != null).AddBonusMode = RuleDefinitions.AddBonusMode.AbilityBonus;
        }

        public static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new AgonizingBlastSpellBuilder(name, guid).AddToDB();
        }

        public static SpellDefinition AgonizingBlastSpell = CreateAndAddToDB(AgonizingBlastSpellName, AgonizingBlastSpellNameGuid);
    }
}
