using System;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class WarlockEldritchInvocationEldritchSightPowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        private const string WarlockEldritchInvocationEldritchSightPowerName = "ClassWarlockEldritchInvocationEldritchSightPower";
        private static readonly string WarlockEldritchInvocationEldritchSightPowerGuid = GuidHelper.Create(new Guid(Settings.GUID), WarlockEldritchInvocationEldritchSightPowerName).ToString();

        protected WarlockEldritchInvocationEldritchSightPowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerFighterSecondWind, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationEldritchSightPowerTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationEldritchSightPowerDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.DetectMagic.GuiPresentation.SpriteReference);

            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);
            Definition.SetFixedUsesPerRecharge(1);
            Definition.SetCostPerUse(0);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.Action);

            Definition.EffectDescription.Copy(DatabaseHelper.SpellDefinitions.DetectMagic.EffectDescription);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new WarlockEldritchInvocationEldritchSightPowerBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionPower WarlockEldritchInvocationEldritchSightPower = CreateAndAddToDB(WarlockEldritchInvocationEldritchSightPowerName, WarlockEldritchInvocationEldritchSightPowerGuid);
    }
}
