using System.Collections.Generic;
using System.Linq;
using static SolastaUnfinishedBusiness.Settings;

namespace SolastaUnfinishedBusiness.Models
{
    internal static class InspectionPanelContext
    {
        private static RulesetCharacterHero selectedHero;

        private static int selectedClass = 0;

        private static readonly List<string> classesWithDeity = new List<string>() { "Paladin", "Cleric" };

        internal static RulesetCharacterHero SelectedHero
        {
            get => selectedHero;
            set
            {
                selectedHero = value;
                selectedClass = 0;
            }
        }

        internal static CharacterClassDefinition SelectedClass => selectedHero?.ClassesAndLevels.Keys.ElementAt(selectedClass);

        internal static bool IsMulticlass => selectedHero?.ClassesAndLevels.Count > 1;

        internal static bool RequiresDeity => selectedHero?.DeityDefinition != null && classesWithDeity.Contains(SelectedClass.Name);

        internal static void Load()
        {
            var inputService = ServiceRepository.GetService<IInputService>();

            inputService.RegisterCommand(PLAIN_UP, 273, -1, -1, -1, -1, -1);
            inputService.RegisterCommand(PLAIN_DOWN, 274, -1, -1, -1, -1, -1);
            inputService.RegisterCommand(PLAIN_RIGHT, 275, -1, -1, -1, -1, -1);
            inputService.RegisterCommand(PLAIN_LEFT, 276, -1, -1, -1, -1, -1);
        }

        internal static List<FightingStyleDefinition> GetTrainedFightingStyles()
        {
            var fightingStyleIdx = 0;
            var classLevelFightingStyle = new Dictionary<string, FightingStyleDefinition>() { };
            var classBadges = new List<FightingStyleDefinition>() { };

            foreach (var activeFeature in selectedHero.ActiveFeatures.Where(x => x.Key.Contains(AttributeDefinitions.TagClass)))
            {
                foreach (var featureDefinition in activeFeature.Value.Where(x => x is FeatureDefinitionFightingStyleChoice featureDefinitionFightingStyleChoice))
                {
                    classLevelFightingStyle.Add(activeFeature.Key, selectedHero.TrainedFightingStyles[fightingStyleIdx++]);
                }
            }

            foreach (var tuple in classLevelFightingStyle.Where(x => x.Key.Contains(SelectedClass.Name)))
            {
                classBadges.Add(tuple.Value);
            }

            return classBadges;
        }

        internal static void PickPreviousHeroClass()
        {
            selectedClass = selectedClass > 0 ? selectedClass - 1 : selectedHero.ClassesAndLevels.Count - 1;
        }

        internal static void PickNextHeroClass()
        {
            selectedClass = selectedClass < selectedHero.ClassesAndLevels.Count - 1 ? selectedClass + 1 : 0;
        }
    }
}
