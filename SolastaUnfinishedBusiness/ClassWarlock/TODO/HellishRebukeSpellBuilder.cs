using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class HellishRebukeSpellBuilder : BaseDefinitionBuilder<SpellDefinition>
    {
        private const string HellishRebukeSpellName = "ZSHellishRebukeSpell";
        private static readonly string HellishRebukeSpellNameGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, HellishRebukeSpellName).ToString();

        protected HellishRebukeSpellBuilder(string name, string guid) : base(DatabaseHelper.SpellDefinitions.SacredFlame, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ZSHellishRebukeSpellTitle";
            Definition.GuiPresentation.Description = "Feat/&ZSHellishRebukeSpellDescription";
            Definition.SetSpellLevel(1);
            Definition.SetSomaticComponent(true);
            Definition.SetVerboseComponent(true);
            Definition.SetCastingTime(RuleDefinitions.ActivationTime.Reaction);

            //D10 damage
            var damageForm = new DamageForm
            {
                DiceNumber = 2,
                DamageType = "DamageFire",
                DieType = RuleDefinitions.DieType.D10
            };

            var damageEffectForm = new EffectForm
            {
                HasSavingThrow = true,
                FormType = EffectForm.EffectFormType.Damage,
                SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage,
                SaveOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn,
                DamageForm = damageForm
            };


            //Additional die per spell level
            var advancement = new EffectAdvancement();
            advancement.SetEffectIncrementMethod(RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel);
            advancement.SetAdditionalDicePerIncrement(1);

            var effectDescription = Definition.EffectDescription;
            effectDescription.SetRangeParameter(12);
            effectDescription.EffectForms.Clear();
            effectDescription.SetTargetParameter(1);
            damageEffectForm.HasSavingThrow = true;
            effectDescription.SavingThrowAbility = "Dexterity";
            effectDescription.SetEffectAdvancement(advancement);
            effectDescription.EffectForms.Add(damageEffectForm);

            Definition.SetEffectDescription(effectDescription);
        }

        public static SpellDefinition CreateAndAddToDB(string name, string guid)
        {
            return new HellishRebukeSpellBuilder(name, guid).AddToDB();
        }

        public static SpellDefinition HellishRebukeSpell = CreateAndAddToDB(HellishRebukeSpellName, HellishRebukeSpellNameGuid);
    }
}
