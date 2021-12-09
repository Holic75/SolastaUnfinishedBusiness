using System;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class WarlockEldritchInvocationFiendishVigorPowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        private const string WarlockEldritchInvocationFiendishVigorPowerName = "ClassWarlockEldritchInvocationFiendishVigorPower";
        private static readonly string WarlockEldritchInvocationFiendishVigorPowerGuid = GuidHelper.Create(new Guid(Settings.GUID), WarlockEldritchInvocationFiendishVigorPowerName).ToString();

        protected WarlockEldritchInvocationFiendishVigorPowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerFighterSecondWind, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationFiendishVigorPowerTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationFiendishVigorPowerDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.FalseLife.GuiPresentation.SpriteReference);

            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);
            Definition.SetFixedUsesPerRecharge(1);
            Definition.SetCostPerUse(0);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.Action);

            Definition.EffectDescription.Copy(DatabaseHelper.SpellDefinitions.FalseLife.EffectDescription);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new WarlockEldritchInvocationFiendishVigorPowerBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionPower WarlockEldritchInvocationFiendishVigorPower = CreateAndAddToDB(WarlockEldritchInvocationFiendishVigorPowerName, WarlockEldritchInvocationFiendishVigorPowerGuid);
    }
}
