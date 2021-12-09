using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class PactMarkFeatPowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        private const string PactMarkedFeatPowerName = "ZSPactMarkedFeatPower";
        private static readonly string HellishRebukeSpellNameGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, PactMarkedFeatPowerName).ToString();

        protected PactMarkFeatPowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerRangerPrimevalAwareness, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ZSPactMarkedFeatPowerTitle";
            Definition.GuiPresentation.Description = "Feat/&ZSPactMarkedFeatPowerDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.HuntersMark.GuiPresentation.SpriteReference);
            Definition.SetEffectDescription(PactMarkSpellBuilder.PactMarkSpell.EffectDescription);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.BonusAction);
            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
            Definition.SetFixedUsesPerRecharge(1);
            Definition.SetCostPerUse(1);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new PactMarkFeatPowerBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionPower PactMarkedPower = CreateAndAddToDB(PactMarkedFeatPowerName, HellishRebukeSpellNameGuid);
    }
}
