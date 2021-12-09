using System;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class WarlockEldritchInvocationSetBuilderLevel5 : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
    {
        private const string WarlockEldritchInvocationSetLevel5Name = "ClassWarlockEldritchInvocationSetLevel5";
        private static readonly string WarlockEldritchInvocationSetLevel5Guid = GuidHelper.Create(new Guid(Settings.GUID), WarlockEldritchInvocationSetLevel5Name).ToString();

        protected WarlockEldritchInvocationSetBuilderLevel5(string name, string guid) : base(WarlockEldritchInvocationSetBuilderLevel2.WarlockEldritchInvocationSetLevel2, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationSetLevel5Title";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationSetLevel5Description";

            Definition.FeatureSet.Add(WarlockEldritchInvocationThirstingBladeBuilder.WarlockEldritchInvocationThirstingBlade);
            Definition.SetUniqueChoices(false);
        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new WarlockEldritchInvocationSetBuilderLevel5(name, guid).AddToDB();
        }

        public static FeatureDefinitionFeatureSet WarlockEldritchInvocationSetLevel5 = CreateAndAddToDB(WarlockEldritchInvocationSetLevel5Name, WarlockEldritchInvocationSetLevel5Guid);
    }
}
