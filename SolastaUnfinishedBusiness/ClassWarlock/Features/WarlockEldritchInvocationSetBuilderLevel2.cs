using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class WarlockEldritchInvocationSetBuilderLevel2 : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
    {
        private const string WarlockEldritchInvocationSetLevel2Name = "ClassWarlockEldritchInvocationSetLevel2";
        private static readonly string WarlockEldritchInvocationSetLevel2Guid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, WarlockEldritchInvocationSetLevel2Name).ToString();

        protected WarlockEldritchInvocationSetBuilderLevel2(string name, string guid) : base(DatabaseHelper.FeatureDefinitionFeatureSets.TerrainTypeAffinityRangerNaturalExplorerChoice, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationSetLevel2Title";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationSetLevel2Description";

            Definition.FeatureSet.Clear();
            Definition.FeatureSet.Add(WarlockEldritchInvocationArmorOfShadowsPowerBuilder.WarlockEldritchInvocationArmorOfShadowsPower);
            Definition.FeatureSet.Add(WarlockEldritchInvocationEldritchSightPowerBuilder.WarlockEldritchInvocationEldritchSightPower);
            Definition.FeatureSet.Add(WarlockEldritchInvocationFiendishVigorPowerBuilder.WarlockEldritchInvocationFiendishVigorPower);
            Definition.FeatureSet.Add(WarlockEldritchInvocationDevilsSightPowerBuilder.WarlockEldritchInvocationDevilsSightPower);
            Definition.FeatureSet.Add(WarlockEldritchInvocationBeguilingInfluenceBuilder.WarlockEldritchInvocationBeguilingInfluence);
            Definition.FeatureSet.Add(WarlockEldritchInvocationRepellingBlastBuilder.WarlockEldritchInvocationRepellingBlast);
            Definition.FeatureSet.Add(WarlockEldritchInvocationAgnoizingBlastBuilder.WarlockEldritchInvocationAgnoizingBlast);
            Definition.SetUniqueChoices(false); //Seems to be a bug with unique choices where it makes the list smaller but then selects from the wrong index from the master list, using the index of the item in the smaller list.  My tests on higher levels would have RepellingBlast chosen from the master list when choosing ThirstingBlade from the smaller unique list as an example.
        }

        public static FeatureDefinitionFeatureSet CreateAndAddToDB(string name, string guid)
        {
            return new WarlockEldritchInvocationSetBuilderLevel2(name, guid).AddToDB();
        }

        public static FeatureDefinitionFeatureSet WarlockEldritchInvocationSetLevel2 = CreateAndAddToDB(WarlockEldritchInvocationSetLevel2Name, WarlockEldritchInvocationSetLevel2Guid);
    }
}
