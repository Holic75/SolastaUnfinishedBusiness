using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    //Unfortunately implementing the see through magic darkness is going to be very hard/impossible
    internal class WarlockEldritchInvocationDevilsSightPowerBuilder : BaseDefinitionBuilder<FeatureDefinitionSense>
    {
        private const string WarlockEldritchInvocationDevilsSightPowerName = "ClassWarlockEldritchInvocationDevilsSightPower";
        private static readonly string WarlockEldritchInvocationDevilsSightPowerGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, WarlockEldritchInvocationDevilsSightPowerName).ToString();

        protected WarlockEldritchInvocationDevilsSightPowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision12, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationDevilsSightPowerTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationDevilsSightPowerDescription";

            Definition.SetSenseRange(24);
            //Could potentially give blindsight as well to see through magical darkness but that makes this even more OP
        }

        public static FeatureDefinitionSense CreateAndAddToDB(string name, string guid)
        {
            return new WarlockEldritchInvocationDevilsSightPowerBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionSense WarlockEldritchInvocationDevilsSightPower = CreateAndAddToDB(WarlockEldritchInvocationDevilsSightPowerName, WarlockEldritchInvocationDevilsSightPowerGuid);
    }
}
