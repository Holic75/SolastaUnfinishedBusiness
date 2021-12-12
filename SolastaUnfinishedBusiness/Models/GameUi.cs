using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Models
{
    internal static class GameUi
    {
        private static readonly float[] fontSizes = new float[] { 17f, 17f, 16f, 15f, 12.5f };

        internal static float GetFontSize(int classesCount)
        {
            return fontSizes[classesCount % 5];
        }

        internal static string GetAllSubclassesLabel(GuiCharacter character)
        {
            var builder = new StringBuilder();
            var hero = character.RulesetCharacterHero;

            foreach (var characterClassDefinition in hero.ClassesAndLevels.Keys)
            {
                if (hero.ClassesAndSubclasses.ContainsKey(characterClassDefinition) == true)
                {
                    builder.Append(hero.ClassesAndSubclasses[characterClassDefinition].FormatTitle());
                }
                else
                {
                    builder.Append(characterClassDefinition.FormatTitle());
                    builder.Append("/");
                    builder.Append(hero.ClassesAndLevels[characterClassDefinition]);
                }

                builder.Append("\n");
            }

            return builder.ToString();
        }

        internal static string GetAllClassesLabel(GuiCharacter character, string separator = "\n")
        {
            var builder = new StringBuilder();
            var snapshot = character?.Snapshot;
            var hero = character?.RulesetCharacterHero;

            if (snapshot != null)
            {
                foreach (var className in snapshot.Classes)
                {
                    var classTitle = DatabaseRepository.GetDatabase<CharacterClassDefinition>().GetElement(className).FormatTitle();

                    builder.Append(classTitle);
                    builder.Append(separator);
                }
            }
            else if (hero != null && hero.ClassesAndLevels.Count > 1)
            {
                foreach (var characterClassDefinition in hero.ClassesAndLevels.Keys)
                {
                    builder.Append(characterClassDefinition.FormatTitle());
                    builder.Append("/");
                    builder.Append(hero.ClassesAndLevels[characterClassDefinition]);
                    builder.Append(separator);
                }
            }
            else
            {
                return null;
            }

            return builder.ToString().Remove(builder.Length - separator.Length, separator.Length);
        }

        internal static string GetAllClassesHitDiceLabel(GuiCharacter character, out int dieTypeCount)
        {
            var builder = new StringBuilder();
            var hero = character?.RulesetCharacterHero;
            var dieTypesCount = new Dictionary<RuleDefinitions.DieType, int>() { };
            var separator = " ";

            if (hero != null)
            {
                foreach (var characterClassDefinition in hero.ClassesAndLevels.Keys)
                {
                    if (!dieTypesCount.ContainsKey(characterClassDefinition.HitDice))
                    {
                        dieTypesCount.Add(characterClassDefinition.HitDice, 0);
                    }

                    dieTypesCount[characterClassDefinition.HitDice] += hero.ClassesAndLevels[characterClassDefinition];
                }

                foreach (var dieType in dieTypesCount.Keys)
                {
                    builder.Append(dieTypesCount[dieType].ToString());
                    builder.Append(Gui.GetDieSymbol(dieType));
                    builder.Append(separator);
                }
            }

            dieTypeCount = dieTypesCount.Count;

            return builder.Remove(builder.Length - separator.Length, separator.Length).ToString();
        }

        internal static string GetLevelAndExperienceTooltip(GuiCharacter character)
        {
            var builder = new StringBuilder();
            var hero = character.RulesetCharacterHero;

            if (hero == null)
            {
                return string.Empty;
            }
            else
            {
                var characterLevelAttribute = hero.GetAttribute("CharacterLevel");
                var characterLevel = characterLevelAttribute.CurrentValue;
                var experience = hero.GetAttribute("Experience").CurrentValue;

                if (characterLevel == characterLevelAttribute.MaxValue)
                {
                    builder.Append(Gui.Format("Format/&LevelAndExperienceMaxedFormat", characterLevel.ToString("N0"), experience.ToString("N0")));
                }
                else
                {
                    var num = Mathf.Max(0.0f, (float)RuleDefinitions.ExperienceThresholds[characterLevel] - experience);

                    builder.Append(Gui.Format("Format/&LevelAndExperienceFormat", characterLevel.ToString("N0"), experience.ToString("N0"), num.ToString("N0"), (characterLevel + 1).ToString("N0")));
                }

                if (InspectionPanelContext.IsMulticlass)
                {
                    builder.Append("\n");

                    for (var i = 0; i < hero.ClassesHistory.Count; i++)
                    {
                        var characterClassDefinition = hero.ClassesHistory[i];

                        hero.ClassesAndSubclasses.TryGetValue(characterClassDefinition, out var characterSubclassDefinition);

                        builder.Append($"\n{i + 1:00} - {characterClassDefinition.FormatTitle()} {characterSubclassDefinition?.FormatTitle()}");
                    }
                }

                return builder.ToString();
            }
        }
    }
}
