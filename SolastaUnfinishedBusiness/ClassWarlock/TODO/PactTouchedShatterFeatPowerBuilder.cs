using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class PactTouchedShatterFeatPowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        private const string PactMarkedFeatPowerName = "ZSPactTouchedShatterFeatPower";
        private static readonly string HellishRebukeSpellNameGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, PactMarkedFeatPowerName).ToString();

        protected PactTouchedShatterFeatPowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerRangerPrimevalAwareness, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ZSPactTouchedShatterFeatPowerTitle";
            Definition.GuiPresentation.Description = "Feat/&ZSPactTouchedShatterFeatPowerDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.Shatter.GuiPresentation.SpriteReference);
            Definition.SetEffectDescription(DatabaseHelper.SpellDefinitions.Shatter.EffectDescription);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.Action);
            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
            Definition.SetFixedUsesPerRecharge(1);
            Definition.SetCostPerUse(1);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new PactTouchedShatterFeatPowerBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionPower PactShatter = CreateAndAddToDB(PactMarkedFeatPowerName, HellishRebukeSpellNameGuid);
    }
}
