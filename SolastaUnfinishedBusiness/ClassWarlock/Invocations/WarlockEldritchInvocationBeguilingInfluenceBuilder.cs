using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class WarlockEldritchInvocationBeguilingInfluenceBuilder : BaseDefinitionBuilder<FeatureDefinitionProficiency>
    {
        private const string WarlockEldritchInvocationBeguilingInfluenceName = "ClassWarlockEldritchInvocationBeguilingInfluence";
        private static readonly string WarlockEldritchInvocationBeguilingInfluenceGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, WarlockEldritchInvocationBeguilingInfluenceName).ToString();

        protected WarlockEldritchInvocationBeguilingInfluenceBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionProficiencys.ProficiencySpySkills, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ClassWarlockEldritchInvocationBeguilingInfluenceTitle";
            Definition.GuiPresentation.Description = "Feature/&ClassWarlockEldritchInvocationBeguilingInfluenceDescription";

            Definition.SetProficiencyType(RuleDefinitions.ProficiencyType.Skill);
            Definition.Proficiencies.Clear();
            Definition.Proficiencies.AddRange(new string[] { "Deception", "Persuasion" });
        }

        public static FeatureDefinitionProficiency CreateAndAddToDB(string name, string guid)
        {
            return new WarlockEldritchInvocationBeguilingInfluenceBuilder(name, guid).AddToDB();
        }

        public static FeatureDefinitionProficiency WarlockEldritchInvocationBeguilingInfluence = CreateAndAddToDB(WarlockEldritchInvocationBeguilingInfluenceName, WarlockEldritchInvocationBeguilingInfluenceGuid);
    }
}
