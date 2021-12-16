using System.Collections.Generic;
using System.Linq;

namespace SolastaUnfinishedBusiness.Models
{
    internal static class InOutRules
    {
        internal static void EnumerateHeroAllowedClassDefinitions(RulesetCharacterHero hero, List<CharacterClassDefinition> allowedClasses, ref int selectedClass)
        {
            var currentClass = hero.ClassesHistory[hero.ClassesHistory.Count - 1];

            allowedClasses.Clear();

            // only allows to leave a class if it is a supported one with required In/Out attributes
            if (!IsSupported(currentClass) || Main.Settings.EnableMinInOutAttributes && !ApproveMultiClassInOut(hero, currentClass))
            {
                allowedClasses.Add(currentClass);
            }

            // only allows existing classes with required In/Out attributes
            else if (hero.ClassesAndLevels.Count >= Main.Settings.MaxAllowedClasses)
            {
                foreach (var characterClassDefinition in hero.ClassesAndLevels.Keys)
                {
                    if (!Main.Settings.EnableMinInOutAttributes || ApproveMultiClassInOut(hero, characterClassDefinition))
                    {
                        allowedClasses.Add(characterClassDefinition);
                    }
                }
            }

            // only allows supported classes with required In/Out attributes
            else
            {
                foreach (var classDefinition in DatabaseRepository.GetDatabase<CharacterClassDefinition>())
                {
                    if (IsSupported(classDefinition) && (!Main.Settings.EnableMinInOutAttributes || ApproveMultiClassInOut(hero, classDefinition)))
                    {
                        allowedClasses.Add(classDefinition);
                    }
                }
            }

            allowedClasses.Sort((a, b) => a.FormatTitle().CompareTo(b.FormatTitle()));
            selectedClass = allowedClasses.IndexOf(hero.ClassesHistory[hero.ClassesHistory.Count - 1]);
        }

        internal static bool ApproveMultiClassInOut(RulesetCharacterHero hero, CharacterClassDefinition classDefinition)
        {
            if (classDefinition.GuiPresentation.Hidden)
            {
                return false;
            }

            if (!LevelUpContext.inOutPrerequisites.ContainsKey(classDefinition.Name))
            {
                return false;
            }

            var prerequisites = LevelUpContext.inOutPrerequisites[classDefinition.Name];
            foreach (var p in prerequisites)
            {
                if (p.All(kv => hero.GetAttribute(kv.Key).CurrentValue >= kv.Value))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool IsSupported(CharacterClassDefinition classDefinition)
        {
            return LevelUpContext.inOutPrerequisites.ContainsKey(classDefinition.Name);
        }
    }
}
