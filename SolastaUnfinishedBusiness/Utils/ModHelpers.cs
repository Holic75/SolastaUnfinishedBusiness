using System;
using System.Linq;
using System.Reflection;

namespace SolastaUnfinishedBusiness.Utils
{
    internal static class ModHelpers
    {
        internal static bool isItemGrantedOnLvl1(CharacterClassDefinition characterClass, ItemDefinition item)
        {
            if (characterClass == null)
            {
                return false;
            }

            return characterClass.equipmentRows.Any(er => er.equipmentColumns.Any(ec => ec.equipmentOptions.Any(eo => eo.itemReference == item)));
        }

        internal static bool isFeatureIncreasesAttacksCount(FeatureDefinition feature)
        {
            var attack_modifier = feature as FeatureDefinitionAttributeModifier;
            if (attack_modifier == null)
            {
                return false;
            }

            return attack_modifier.modifiedAttribute == "AttacksNumber" 
                    && attack_modifier.modifierValue == 1
                    && attack_modifier.modifierType2 == FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive;
        }


        internal static bool shouldGrantItemOnMCLevelUp(RulesetCharacterHero hero, CharacterClassDefinition characterClass, ItemDefinition item)
        {
            if (hero == null)
            {
                return false;
            }

            return isItemGrantedOnLvl1(characterClass, item) && !hero.ClassesAndLevels.Keys.Any(k => isItemGrantedOnLvl1(k, item));
        }

        internal static Assembly GetModAssembly(string modName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.Contains(modName));
        }

        internal static Type GetModType(string modName, string typeName)
        {
            return GetModAssembly(modName)?.GetExportedTypes().FirstOrDefault(x => x.FullName.Contains(typeName));
        }

        internal static bool SetModField(string modName, string typeName, string fieldName, object value)
        {
            var type = GetModType(modName, typeName);

            if (type != null)
            {
                var fieldInfo = type.GetField(fieldName);

                if (fieldInfo != null)
                {
                    try
                    {
                        fieldInfo.SetValue(type, value);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }

                }
            }

            return false;
        }
    }
}
