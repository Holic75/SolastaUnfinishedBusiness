using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class EldritchBlastPowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        private const string PactMarkedFeatPowerName = "ZSPactTouchedEldritchBlastFeatPower";
        private static readonly string HellishRebukeSpellNameGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, PactMarkedFeatPowerName).ToString();

        protected EldritchBlastPowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerRangerPrimevalAwareness, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ZSPactTouchedEldritchBlastFeatPowerTitle";
            Definition.GuiPresentation.Description = "Feat/&ZSPactTouchedEldritchBlastFeatPowerDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.MagicMissile.GuiPresentation.SpriteReference);
            Definition.SetEffectDescription(EldritchBlastSpellBuilder.EldritchBlastSpell.EffectDescription);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.Action);
            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);
            Definition.SetFixedUsesPerRecharge(1);
            Definition.SetCostPerUse(0);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new EldritchBlastPowerBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionPower PactEldricthBlastPower = CreateAndAddToDB(PactMarkedFeatPowerName, HellishRebukeSpellNameGuid);
    }
}
