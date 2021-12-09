using System;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class WarlockEldritchInvocationArmorOfShadowsPowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        private const string WarlockEldritchInvocationArmorOfShadowsPowerName = "ClassWarlockEldritchInvocationArmorOfShadowsPower";
        private static readonly string WarlockEldritchInvocationArmorOfShadowsPowerGuid = GuidHelper.Create(new Guid(Settings.GUID), WarlockEldritchInvocationArmorOfShadowsPowerName).ToString();

        protected WarlockEldritchInvocationArmorOfShadowsPowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerFighterSecondWind, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationArmorOfShadowsPowerTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationArmorOfShadowsPowerDescription";
            Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.MageArmor.GuiPresentation.SpriteReference);

            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);
            Definition.SetFixedUsesPerRecharge(1);
            Definition.SetCostPerUse(0);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.Action);

            Definition.EffectDescription.Copy(DatabaseHelper.SpellDefinitions.MageArmor.EffectDescription);
            Definition.EffectDescription.SetTargetType(RuleDefinitions.TargetType.Self);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new WarlockEldritchInvocationArmorOfShadowsPowerBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionPower WarlockEldritchInvocationArmorOfShadowsPower = CreateAndAddToDB(WarlockEldritchInvocationArmorOfShadowsPowerName, WarlockEldritchInvocationArmorOfShadowsPowerGuid);
    }
}
