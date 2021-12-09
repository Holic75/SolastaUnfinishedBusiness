using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class WarlockEldritchInvocationThirstingBladeBuilder : BaseDefinitionBuilder<FeatureDefinitionAttributeModifier>
    {
        private const string WarlockEldritchInvocationThirstingBladeName = "ClassWarlockEldritchInvocationThirstingBlade";
        private static readonly string WarlockEldritchInvocationThirstingBladeGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, WarlockEldritchInvocationThirstingBladeName).ToString();

        protected WarlockEldritchInvocationThirstingBladeBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationThirstingBladeTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationThirstingBladeDescription";

            Definition.SetModifiedAttribute("AttacksNumber");
            Definition.SetModifierType2(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive);
            Definition.SetModifierValue(1);
            //Just use the extra attack from fighter
        }

        public static FeatureDefinitionAttributeModifier CreateAndAddToDB(string name, string guid)
        {
            return new WarlockEldritchInvocationThirstingBladeBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionAttributeModifier WarlockEldritchInvocationThirstingBlade = CreateAndAddToDB(WarlockEldritchInvocationThirstingBladeName, WarlockEldritchInvocationThirstingBladeGuid);
    }
}
