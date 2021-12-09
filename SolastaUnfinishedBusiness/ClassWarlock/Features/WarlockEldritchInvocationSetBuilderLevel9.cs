using System;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class WarlockEldritchInvocationSetBuilderLevel9 : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
    {
        private const string WarlockEldritchInvocationSetLevel9Name = "ClassWarlockEldritchInvocationSetLevel9";
        private static readonly string WarlockEldritchInvocationSetLevel9Guid = GuidHelper.Create(new Guid(Settings.GUID), WarlockEldritchInvocationSetLevel9Name).ToString();

        protected WarlockEldritchInvocationSetBuilderLevel9(string name, string guid) : base(WarlockEldritchInvocationSetBuilderLevel5.WarlockEldritchInvocationSetLevel5, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationSetLevel9Title";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationSetLevel9Description";

            Definition.FeatureSet.Add(WarlockEldritchInvocationAscendentStepPowerBuilder.WarlockEldritchInvocationAscendentStepPower);
            Definition.FeatureSet.Add(WarlockEldritchInvocationOtherwordlyLeapPowerBuilder.WarlockEldritchInvocationOtherwordlyLeapPower);

            Definition.SetUniqueChoices(false);
        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new WarlockEldritchInvocationSetBuilderLevel9(name, guid).AddToDB();
        }

        public static FeatureDefinitionFeatureSet WarlockEldritchInvocationSetLevel9 = CreateAndAddToDB(WarlockEldritchInvocationSetLevel9Name, WarlockEldritchInvocationSetLevel9Guid);
    }
}
