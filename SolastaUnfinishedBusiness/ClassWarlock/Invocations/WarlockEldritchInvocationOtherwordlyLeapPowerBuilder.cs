using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class WarlockEldritchInvocationOtherwordlyLeapPowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        private const string WarlockEldritchInvocationOtherwordlyLeapPowerName = "ClassWarlockEldritchInvocationOtherwordlyLeapPower";
        private static readonly string WarlockEldritchInvocationOtherwordlyLeapPowerGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, WarlockEldritchInvocationOtherwordlyLeapPowerName).ToString();

        protected WarlockEldritchInvocationOtherwordlyLeapPowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerFighterSecondWind, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationOtherwordlyLeapPowerTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationOtherwordlyLeapPowerDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.Jump.GuiPresentation.SpriteReference);

            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);
            Definition.SetFixedUsesPerRecharge(1);
            Definition.SetCostPerUse(0);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.Action);

            Definition.EffectDescription.Copy(DatabaseHelper.SpellDefinitions.Jump.EffectDescription);
            Definition.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Self);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new WarlockEldritchInvocationOtherwordlyLeapPowerBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionPower WarlockEldritchInvocationOtherwordlyLeapPower = CreateAndAddToDB(WarlockEldritchInvocationOtherwordlyLeapPowerName, WarlockEldritchInvocationOtherwordlyLeapPowerGuid);
    }
}
