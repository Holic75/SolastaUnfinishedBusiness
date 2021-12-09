using System.Collections.Generic;
using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPointPools;

namespace SolastaUnfinishedBusiness.Builders
{
    internal class SkillProficiencyMulticlassBuilder : BaseDefinitionBuilder<FeatureDefinitionPointPool>
    {
        private const string BardClassSkillProficiencyMulticlassName = "BardClassSkillProficiencyMulticlass";
        private const string BardClassSkillProficiencyMulticlassGuid = "a69b2527569b4893abe57ad1f80e97ed";

        private const string PointPoolRangerSkillPointsMulticlassName = "PointPoolRangerSkillPointsMulticlass";
        private const string PointPoolRangerSkillPointsMulticlassGuid = "096e4e01b52b490e807cf8d458845aa5";
        private const string PointPoolRogueSkillPointsMulticlassName = "PointPoolRogueSkillPointsMulticlass";
        private const string PointPoolRogueSkillPointsMulticlassGuid = "451259da8c5c41f4b1b363f00b01be4e";

        protected SkillProficiencyMulticlassBuilder(string name, string guid, string title, List<string> restrictedChoices) : base(PointPoolRangerSkillPoints, name, guid)
        {
            Definition.SetPoolAmount(1);
            Definition.RestrictedChoices.Clear();
            Definition.RestrictedChoices.AddRange(restrictedChoices);
            Definition.GuiPresentation.Title = title;
            Definition.GuiPresentation.Description = "Feature/&SkillGainChoicesPluralDescription";
        }

        private static FeatureDefinitionPointPool CreateAndAddToDB(string name, string guid, string title, List<string> proficiencysToReplace)
        {
            return new SkillProficiencyMulticlassBuilder(name, guid, title, proficiencysToReplace).AddToDB();
        }

        public static readonly FeatureDefinitionPointPool BardClassSkillProficiencyMulticlass =
            CreateAndAddToDB(BardClassSkillProficiencyMulticlassName, BardClassSkillProficiencyMulticlassGuid, "Feature/&BardClassSkillPointPoolTitle", new List<string> {
                "Acrobatics",
                "AnimalHandling",
                "Arcana",
                "Athletics",
                "Deception",
                "History",
                "Insight",
                "Intimidation",
                "Investigation",
                "Medecine",
                "Nature",
                "Perception",
                "Performance",
                "Persuasion",
                "Religion",
                "SleightOfHand",
                "Stealth",
                "Survival"
            });

        public static readonly FeatureDefinitionPointPool PointPoolRangerSkillPointsMulticlass =
            CreateAndAddToDB(PointPoolRangerSkillPointsMulticlassName, PointPoolRangerSkillPointsMulticlassGuid, "Feature/&RangerSkillsTitle", new List<string>
            {
                "AnimalHandling",
                "Athletics",
                "Insight",
                "Investigation",
                "Nature",
                "Perception",
                "Survival",
                "Stealth"
            });

        public static readonly FeatureDefinitionPointPool PointPoolRogueSkillPointsMulticlass =
            CreateAndAddToDB(PointPoolRogueSkillPointsMulticlassName, PointPoolRogueSkillPointsMulticlassGuid, "Feature/&RogueSkillPointsTitle", new List<string>
            {
                "Acrobatics",
                "Athletics",
                "Deception",
                "Insight",
                "Intimidation",
                "Investigation",
                "Perception",
                "Performance",
                "Persuasion",
                "SleightOfHand",
                "Stealth"
            });
    }
}