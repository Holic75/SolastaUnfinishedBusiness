using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class WarlockEldritchInvocationRepellingBlastBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        private const string WarlockEldritchInvocationRepellingBlastName = "ClassWarlockEldritchInvocationRepellingBlast";
        private static readonly string WarlockEldritchInvocationRepellingBlastGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, WarlockEldritchInvocationRepellingBlastName).ToString();

        protected WarlockEldritchInvocationRepellingBlastBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerFighterSecondWind, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationRepellingBlastTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationRepellingBlastDescription";

            Definition.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);
            Definition.SetFixedUsesPerRecharge(1);
            Definition.SetCostPerUse(0);
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.NoCost);

            //Create the summon effect
            var motionEffect = new EffectForm
            {
                FormType = EffectForm.EffectFormType.Motion
            };
            var motionForm = new MotionForm();
            motionForm.SetDistance(2);
            motionEffect.SetMotionForm(motionForm);

            //Add to our new effect
            var newEffectDescription = new EffectDescription();
            newEffectDescription.Copy(Definition.EffectDescription);
            newEffectDescription.EffectForms.Clear();
            newEffectDescription.EffectForms.Add(motionEffect);
            newEffectDescription.SetRangeType(RuleDefinitions.RangeType.Distance);
            newEffectDescription.SetRangeParameter(30);
            newEffectDescription.SetTargetType(RuleDefinitions.TargetType.Individuals);
            newEffectDescription.SetTargetSide(RuleDefinitions.Side.Enemy);
            newEffectDescription.SetTargetParameter(1);

            Definition.SetEffectDescription(newEffectDescription);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new WarlockEldritchInvocationRepellingBlastBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionPower WarlockEldritchInvocationRepellingBlast = CreateAndAddToDB(WarlockEldritchInvocationRepellingBlastName, WarlockEldritchInvocationRepellingBlastGuid);
    }
}
