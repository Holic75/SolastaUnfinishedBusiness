using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class WarlockEldritchInvocationAgnoizingBlastBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        private const string WarlockEldritchInvocationAgnoizingBlastName = "ClassWarlockEldritchInvocationAgnoizingBlast";
        private static readonly string WarlockEldritchInvocationAgnoizingBlastGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, WarlockEldritchInvocationAgnoizingBlastName).ToString();

        protected WarlockEldritchInvocationAgnoizingBlastBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerFighterSecondWind, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationAgnoizingBlastTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationAgnoizingBlastDescription";

            //A do nothing power, currently the intention is you take Agonizing blast at level 1 and this invocation since there is no way to override a spell.
            Definition.SetCostPerUse(2);
        }

        public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
        {
            return new WarlockEldritchInvocationAgnoizingBlastBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionPower WarlockEldritchInvocationAgnoizingBlast = CreateAndAddToDB(WarlockEldritchInvocationAgnoizingBlastName, WarlockEldritchInvocationAgnoizingBlastGuid);
    }
}
