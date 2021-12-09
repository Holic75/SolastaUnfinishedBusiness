using System.Collections.Generic;
using SolastaModApi;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionProficiencys;

namespace SolastaUnfinishedBusiness.Builders
{
    internal class ArmorProficiencyMulticlassBuilder : BaseDefinitionBuilder<FeatureDefinitionProficiency>
    {
        private const string BarbarianArmorProficiencyMulticlassName = "BarbarianArmorProficiencyMulticlass";
        private const string BarbarianArmorProficiencyMulticlassGuid = "5dffec907a424fccbfec103344421b51";
        private const string FighterArmorProficiencyMulticlassName = "FighterArmorProficiencyMulticlass";
        private const string FighterArmorProficiencyMulticlassGuid = "5df5ec907a424fccbfec103344421b51";
        private const string PaladinArmorProficiencyMulticlassName = "PaladinArmorProficiencyMulticlass";
        private const string PaladinArmorProficiencyMulticlassGuid = "69b18e44aabd4acca702c05f9d6c7fcb";

        protected ArmorProficiencyMulticlassBuilder(string name, string guid, string title, List<string> proficiencysToReplace) : base(ProficiencyFighterArmor, name, guid)
        {
            Definition.Proficiencies.Clear();
            Definition.Proficiencies.AddRange(proficiencysToReplace);
            Definition.GuiPresentation.Title = title;
        }

        private static FeatureDefinitionProficiency CreateAndAddToDB(string name, string guid, string title, List<string> proficiencysToReplace)
        {
            return new ArmorProficiencyMulticlassBuilder(name, guid, title, proficiencysToReplace).AddToDB();
        }

        public static readonly FeatureDefinitionProficiency BarbarianArmorProficiencyMulticlass =
            CreateAndAddToDB(BarbarianArmorProficiencyMulticlassName, BarbarianArmorProficiencyMulticlassGuid, "Feature/&BarbarianArmorProficiencyTitle", new List<string> {
                "ShieldCategory"
            });

        public static readonly FeatureDefinitionProficiency FighterArmorProficiencyMulticlass =
            CreateAndAddToDB(FighterArmorProficiencyMulticlassName, FighterArmorProficiencyMulticlassGuid, "Feature/&FighterArmorProficiencyTitle", new List<string> {
                "LightArmorCategory",
                "MediumArmorCategory",
                "ShieldCategory"
            });

        public static readonly FeatureDefinitionProficiency PaladinArmorProficiencyMulticlass =
            CreateAndAddToDB(PaladinArmorProficiencyMulticlassName, PaladinArmorProficiencyMulticlassGuid, "Feature/&PaladinArmorProficiencyTitle", new List<string> {
                "LightArmorCategory",
                "MediumArmorCategory",
                "ShieldCategory"
            });
    }
}