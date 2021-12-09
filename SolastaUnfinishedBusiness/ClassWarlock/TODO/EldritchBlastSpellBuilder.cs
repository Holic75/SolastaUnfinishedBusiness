using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class EldritchBlastSpellBuilder : BaseDefinitionBuilder<SpellDefinition>
    {
        private const string EldritchBlastSpellName = "ZSEldritchBlastSpell";
        private static readonly string EldritchBlastSpellNameGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, EldritchBlastSpellName).ToString();

        protected EldritchBlastSpellBuilder(string name, string guid) : base(DatabaseHelper.SpellDefinitions.MagicMissile, name, guid)
        {
            Definition.GuiPresentation.Title = "Spell/&ZSEldritchBlastSpellTitle";
            Definition.GuiPresentation.Description = "Spell/&ZSEldritchBlastSpellDescription";
            Definition.SetSpellLevel(0);
            Definition.SetSomaticComponent(true);
            Definition.SetVerboseComponent(true);

            //D10 damage
            var damageForm = new DamageForm
            {
                DiceNumber = 1,
                DamageType = "DamageForce",
                DieType = RuleDefinitions.DieType.D10
            };

            var damageEffectForm = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Damage,
                DamageForm = damageForm
            };

            //Additional blast every 5 caster levels
            var advancement = new EffectAdvancement();
            advancement.SetEffectIncrementMethod(RuleDefinitions.EffectIncrementMethod.CasterLevelTable);
            advancement.SetIncrementMultiplier(5);
            advancement.SetAdditionalTargetsPerIncrement(1);

            var effectDescription = Definition.EffectDescription;
            effectDescription.SetRangeType(RuleDefinitions.RangeType.RangeHit);
            effectDescription.SetRangeParameter(30);
            effectDescription.SetTargetParameter(1);
            effectDescription.EffectForms.Clear();
            effectDescription.EffectForms.Add(damageEffectForm);
            effectDescription.SetEffectAdvancement(advancement);
        }

        public static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new EldritchBlastSpellBuilder(name, guid).AddToDB();
        }

        public static SpellDefinition EldritchBlastSpell = CreateAndAddToDB(EldritchBlastSpellName, EldritchBlastSpellNameGuid);
    }
}
