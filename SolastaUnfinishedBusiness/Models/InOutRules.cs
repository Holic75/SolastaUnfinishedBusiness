using System.Collections.Generic;

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
            var strength = hero.GetAttribute(AttributeDefinitions.Strength).BaseValue;
            var dexterity = hero.GetAttribute(AttributeDefinitions.Dexterity).BaseValue;
            var intelligence = hero.GetAttribute(AttributeDefinitions.Intelligence).BaseValue;
            var wisdom = hero.GetAttribute(AttributeDefinitions.Wisdom).BaseValue;
            var charisma = hero.GetAttribute(AttributeDefinitions.Charisma).BaseValue;

            if (classDefinition.GuiPresentation.Hidden)
            {
                return false;
            }

            switch (classDefinition.Name)
            {
                case "Barbarian":
                    return strength >= 13;

                //case "Bard":
                //case "Warlock":
                case "Sorcerer":
                case "ClassWarlock": // Zappastuff's Warlock
                    return charisma >= 13;

                case "Cleric":
                case "Druid":
                    return wisdom >= 13;

                case "Fighter":
                    return strength >= 13 || dexterity >= 13;

                //case "Monk":
                case "Ranger":
                    return dexterity >= 13 && wisdom >= 13;

                case "Paladin":
                    return strength >= 13 && charisma >= 13;

                case "Rogue":
                    return dexterity >= 13;

                case "ClassTinkerer": // CJD's Tinkerer
                case "Wizard":
                    return intelligence >= 13;

                default:
                    return false;
            }
        }

        internal static bool IsSupported(CharacterClassDefinition classDefinition)
        {
            switch (classDefinition.Name)
            {
                case "Barbarian":
                //case "Bard":
                case "Sorcerer":
                //case "Warlock":
                case "ClassWarlock": // Zappastuff's Warlock
                case "Cleric":
                case "Druid":
                case "Fighter":
                //case "Monk":
                case "Ranger":
                case "Paladin":
                case "Rogue":
                case "ClassTinkerer": // CJD's Tinkerer
                case "Wizard":
                    return true;

                default:
                    return false;
            }
        }
    }
}
