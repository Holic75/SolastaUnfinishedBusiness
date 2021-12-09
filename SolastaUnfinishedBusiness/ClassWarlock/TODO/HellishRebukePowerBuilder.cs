using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class HellishRebukePowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        private const string HellishRebukeSpellName = "ZSHellishRebukePowerSpell";
        private static readonly string HellishRebukeSpellNameGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, HellishRebukeSpellName).ToString();

        protected HellishRebukePowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerRangerPrimevalAwareness, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ZSHellishRebukeSpellTitle";
            Definition.GuiPresentation.Description = "Feat/&ZSHellishRebukeSpellDescription";
            Definition.SetSpellcastingFeature(DatabaseHelper.FeatureDefinitionCastSpells.CastSpellWizard);
            Definition.SetEffectDescription(HellishRebukeSpellBuilder.HellishRebukeSpell.EffectDescription);
            Definition.SetReactionContext(RuleDefinitions.ReactionTriggerContext.HitByMelee);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.Reaction);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new HellishRebukePowerBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionPower HellishRebukeSpell = CreateAndAddToDB(HellishRebukeSpellName, HellishRebukeSpellNameGuid);
    }
}
