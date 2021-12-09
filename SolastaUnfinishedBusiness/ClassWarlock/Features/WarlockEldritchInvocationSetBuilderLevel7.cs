using System;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class WarlockEldritchInvocationSetBuilderLevel7 : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
    {
        private const string WarlockEldritchInvocationSetLevel7Name = "ClassWarlockEldritchInvocationSetLevel7";
        private static readonly string WarlockEldritchInvocationSetLevel7Guid = GuidHelper.Create(new Guid(Settings.GUID), WarlockEldritchInvocationSetLevel7Name).ToString();

        protected WarlockEldritchInvocationSetBuilderLevel7(string name, string guid) : base(WarlockEldritchInvocationSetBuilderLevel5.WarlockEldritchInvocationSetLevel5, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationSetLevel7Title";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationSetLevel7Description";

            Definition.SetUniqueChoices(false);
        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new WarlockEldritchInvocationSetBuilderLevel7(name, guid).AddToDB();
        }

        public static FeatureDefinitionFeatureSet WarlockEldritchInvocationSetLevel7 = CreateAndAddToDB(WarlockEldritchInvocationSetLevel7Name, WarlockEldritchInvocationSetLevel7Guid);
    }
}
