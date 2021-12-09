using System;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class WarlockEldritchInvocationAscendentStepPowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        private const string WarlockEldritchInvocationAscendentStepPowerName = "ClassWarlockEldritchInvocationAscendentStepPower";
        private static readonly string WarlockEldritchInvocationAscendentStepPowerGuid = GuidHelper.Create(new Guid(Settings.GUID), WarlockEldritchInvocationAscendentStepPowerName).ToString();

        protected WarlockEldritchInvocationAscendentStepPowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerFighterSecondWind, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationAscendentStepPowerTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationAscendentStepPowerDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.Levitate.GuiPresentation.SpriteReference);

            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);
            Definition.SetFixedUsesPerRecharge(1);
            Definition.SetCostPerUse(0);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.Action);

            Definition.EffectDescription.Copy(DatabaseHelper.SpellDefinitions.Levitate.EffectDescription);
            Definition.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Self);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new WarlockEldritchInvocationAscendentStepPowerBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionPower WarlockEldritchInvocationAscendentStepPower = CreateAndAddToDB(WarlockEldritchInvocationAscendentStepPowerName, WarlockEldritchInvocationAscendentStepPowerGuid);
    }
}
