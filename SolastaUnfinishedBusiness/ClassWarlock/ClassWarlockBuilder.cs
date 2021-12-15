using System;
using System.Collections.Generic;
using SolastaModApi;
using UnityEngine;
using static CharacterClassDefinition;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    public static class ClassWarlockBuilder
    {
        public static CharacterClassDefinition ClassWarlock { get; private set; }

        public static FeatureDefinitionProficiency FeatureDefinitionProficiencyArmor { get; private set; }

        public static FeatureDefinitionProficiency FeatureDefinitionProficiencyWeapon { get; private set; }

        public static FeatureDefinitionProficiency FeatureDefinitionProficiencyTool { get; private set; }

        public static FeatureDefinitionProficiency FeatureDefinitionProficiencySavingThrow { get; private set; }

        public static FeatureDefinitionPointPool FeatureDefinitionSkillPoints { get; private set; }

        public static FeatureDefinitionCastSpell FeatureDefinitionClassWarlockCastSpell { get; private set; }

        private static void BuildEquipment(CharacterClassDefinitionBuilder classWarlockBuilder)
        {
            classWarlockBuilder.AddEquipmentRow(
                new List<HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.LightCrossbow, EquipmentDefinitions.OptionWeapon, 1),
                    EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Bolt, EquipmentDefinitions.OptionAmmoPack, 1),
                },
                new List<HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.LightCrossbow, EquipmentDefinitions.OptionWeaponSimpleChoice, 1),
                });

            classWarlockBuilder.AddEquipmentRow(
                new List<HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.ScholarPack, EquipmentDefinitions.OptionStarterPack, 1),
                },
                new List<HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.DungeoneerPack, EquipmentDefinitions.OptionStarterPack, 1),
                });

            classWarlockBuilder.AddEquipmentRow(
                new List<HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Leather, EquipmentDefinitions.OptionArmor, 1),
                    EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.ComponentPouch, EquipmentDefinitions.OptionFocus, 1),
                    EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Dagger, EquipmentDefinitions.OptionWeapon, 2),
                    EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Dagger, EquipmentDefinitions.OptionWeaponSimpleChoice, 1),
                });
        }

        private static void BuildProficiencies()
        {
            FeatureDefinitionProficiencyArmor = FeatureDefinitionProficiencyBuilder.Build(
                RuleDefinitions.ProficiencyType.Armor,
                new List<string>() { EquipmentDefinitions.LightArmorCategory },
                "ClassWarlockArmorProficiency",
                new GuiPresentationBuilder(
                    "Feature/&ClassWarlockArmorProficiencyDescription",
                    "Feature/&ClassWarlockArmorProficiencyTitle").Build());

            FeatureDefinitionProficiencyWeapon = FeatureDefinitionProficiencyBuilder.Build(
                RuleDefinitions.ProficiencyType.Weapon,
                new List<string>() { EquipmentDefinitions.SimpleWeaponCategory },
                "ClassWarlockWeaponProficiency",
                new GuiPresentationBuilder(
                    "Feature/&ClassWarlockWeaponProficiencyDescription",
                    "Feature/&ClassWarlockWeaponProficiencyTitle").Build());

            FeatureDefinitionProficiencyTool = FeatureDefinitionProficiencyBuilder.Build(
                RuleDefinitions.ProficiencyType.Tool,
                new List<string>
                {
                    DatabaseHelper.ToolTypeDefinitions.EnchantingToolType.Name,
                    DatabaseHelper.ToolTypeDefinitions.HerbalismKitType.Name
                },
                "ClassWarlockToolsProficiency",
                new GuiPresentationBuilder(
                    "Feature/&ClassWarlockToolsProficiencyDescription",
                    "Feature/&ClassWarlockToolsProficiencyTitle").Build());

            FeatureDefinitionProficiencySavingThrow = FeatureDefinitionProficiencyBuilder.Build(
                RuleDefinitions.ProficiencyType.SavingThrow,
                new List<string>() { AttributeDefinitions.Charisma, AttributeDefinitions.Wisdom },
                "ClassWarlockSavingThrowProficiency",
                new GuiPresentationBuilder(
                    "Feature/&ClassWarlockSavingThrowProficiencyDescription",
                    "Feature/&ClassWarlockSavingThrowProficiencyTitle").Build());

            FeatureDefinitionSkillPoints = FeatureDefinitionPointPoolBuilder.Build(HeroDefinitions.PointsPoolType.Skill, 2,
                new List<string>
                {
                    SkillDefinitions.Arcana,
                    SkillDefinitions.Deception,
                    SkillDefinitions.History,
                    SkillDefinitions.Intimidation,
                    SkillDefinitions.Investigation,
                    SkillDefinitions.Nature,
                    SkillDefinitions.Religion
                },
                "ClassWarlockSkillProficiency",
                new GuiPresentationBuilder(
                    "Feature/&ClassWarlockSkillProficiencyDescription",
                    "Feature/&ClassWarlockSkillProficiencyTitle").Build());
        }

        private static void BuildSpells()
        {
            var castSpellName = "ClassWarlockCastSpell";
            var castSpellGuid = GuidHelper.Create(new Guid(Settings.GUID), castSpellName).ToString();
            var classWarlockCastSpell = new CastSpellBuilder(castSpellName, castSpellGuid);
            var classWarlockSpellList = ClassWarlockSpellListBuilder.Build();

            classWarlockCastSpell.SetGuiPresentation(new GuiPresentationBuilder("Feature/&ClassWarlockSpellcastingDescription", "Feature/&ClassWarlockSpellcastingTitle").Build());
            classWarlockCastSpell.SetKnownCantrips(new List<int>
            {
                2, 2, 2, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4
            });
            classWarlockCastSpell.SetKnownSpells(new List<int>
            {
                2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 11, 11, 12, 12, 13, 13, 14, 14, 15, 15
            });
            classWarlockCastSpell.SetSlotsPerLevel(Models.SharedSpellsContext.WarlockCastingSlots);
            classWarlockCastSpell.SetSlotsRecharge(RuleDefinitions.RechargeRate.ShortRest);
            classWarlockCastSpell.SetSpellCastingAbility(AttributeDefinitions.Charisma);
            classWarlockCastSpell.SetSpellCastingLevel(5);
            classWarlockCastSpell.SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Class);
            classWarlockCastSpell.SetSpellList(classWarlockSpellList);
            classWarlockCastSpell.SetSpellKnowledge(RuleDefinitions.SpellKnowledge.Selection);
            classWarlockCastSpell.SetSpellPreparationCount(RuleDefinitions.SpellPreparationCount.AbilityBonusPlusLevel);
            classWarlockCastSpell.SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown);

            FeatureDefinitionClassWarlockCastSpell = classWarlockCastSpell.AddToDB();
        }

        private static void BuildSubclasses(CharacterClassDefinitionBuilder classWarlockBuilder)
        {
            var subClassChoiceName = "ClassWarlockSubclassChoice";
            var subClassChoiceGuid = GuidHelper.Create(new Guid(Settings.GUID), subClassChoiceName).ToString();
            var classWarlockPatronPresentationBuilder = new GuiPresentationBuilder("Subclass/&ClassWarlockPatronDescription", "Subclass/&ClassWarlockPatronTitle");
            var subclassChoices = classWarlockBuilder.BuildSubclassChoice(1, "Patron", false, subClassChoiceName, classWarlockPatronPresentationBuilder.Build(), subClassChoiceGuid);

            SubclassFiendPatronBuilder.Build();
            subclassChoices.Subclasses.Add(SubclassFiendPatronBuilder.ClassWarlockSubclassFiendPatronName);
        }

        private static void BuildProgression(CharacterClassDefinitionBuilder classWarlockBuilder)
        {
            classWarlockBuilder.AddFeatureAtLevel(FeatureDefinitionProficiencySavingThrow, 1);
            classWarlockBuilder.AddFeatureAtLevel(FeatureDefinitionProficiencyArmor, 1);
            classWarlockBuilder.AddFeatureAtLevel(FeatureDefinitionProficiencyWeapon, 1);
            classWarlockBuilder.AddFeatureAtLevel(FeatureDefinitionProficiencyTool, 1);
            classWarlockBuilder.AddFeatureAtLevel(FeatureDefinitionSkillPoints, 1);
            classWarlockBuilder.AddFeatureAtLevel(FeatureDefinitionClassWarlockCastSpell, 1);
            //classWarlockBuilder.AddFeatureAtLevel(WarlockEldritchInvocationSetBuilderLevel2.WarlockEldritchInvocationSetLevel2, 2);
            classWarlockBuilder.AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 4);
            //classWarlockBuilder.AddFeatureAtLevel(WarlockEldritchInvocationSetBuilderLevel5.WarlockEldritchInvocationSetLevel5, 5);
            //classWarlockBuilder.AddFeatureAtLevel(WarlockEldritchInvocationSetBuilderLevel7.WarlockEldritchInvocationSetLevel7, 7);
            classWarlockBuilder.AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 8);
            //classWarlockBuilder.AddFeatureAtLevel(WarlockEldritchInvocationSetBuilderLevel9.WarlockEldritchInvocationSetLevel9, 9);
            classWarlockBuilder.AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 12);
            classWarlockBuilder.AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 16);
            classWarlockBuilder.AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 19);
        }

        internal static void BuildWarlockClass()
        {
            var className = "ClassWarlock";
            var classGuid = GuidHelper.Create(new Guid(Settings.GUID), className).ToString();
            var classWarlockBuilder = new CharacterClassDefinitionBuilder(className, classGuid);
            var classWarlockGuiPresentationBuilder = new GuiPresentationBuilder("Class/&ClassWarlockDescription", "Class/&ClassWarlockTitle");

            classWarlockGuiPresentationBuilder.SetColor(new Color(1.0f, 1.0f, 1.0f, 1.0f));
            classWarlockGuiPresentationBuilder.SetHidden(!Main.Settings.EnableClassWarlock);
            classWarlockGuiPresentationBuilder.SetSortOrder(1);
            classWarlockGuiPresentationBuilder.SetSpriteReference(Cleric.GuiPresentation.SpriteReference);

            classWarlockBuilder.AddFeatPreference(DatabaseHelper.FeatDefinitions.PowerfulCantrip);
            classWarlockBuilder.AddFeatPreference(DatabaseHelper.FeatDefinitions.FlawlessConcentration);
            classWarlockBuilder.AddFeatPreference(DatabaseHelper.FeatDefinitions.Robust);

            classWarlockBuilder.AddPersonality(DatabaseHelper.PersonalityFlagDefinitions.Violence, 3);
            classWarlockBuilder.AddPersonality(DatabaseHelper.PersonalityFlagDefinitions.Self_Preservation, 3);
            classWarlockBuilder.AddPersonality(DatabaseHelper.PersonalityFlagDefinitions.Normal, 3);
            classWarlockBuilder.AddPersonality(DatabaseHelper.PersonalityFlagDefinitions.GpSpellcaster, 5);
            classWarlockBuilder.AddPersonality(DatabaseHelper.PersonalityFlagDefinitions.GpExplorer, 1);

            classWarlockBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Deception);
            classWarlockBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Intimidation);
            classWarlockBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Arcana);
            classWarlockBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.History);
            classWarlockBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Investigation);
            classWarlockBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Religion);
            classWarlockBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Nature);
            classWarlockBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Persuasion);

            classWarlockBuilder.AddToolPreference(DatabaseHelper.ToolTypeDefinitions.EnchantingToolType);
            classWarlockBuilder.AddToolPreference(DatabaseHelper.ToolTypeDefinitions.HerbalismKitType);

            classWarlockBuilder.SetAbilityScorePriorities(
                AttributeDefinitions.Charisma,
                AttributeDefinitions.Strength,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Intelligence);

            classWarlockBuilder.SetAnimationId(AnimationDefinitions.ClassAnimationId.Wizard);
            classWarlockBuilder.SetBattleAI(DatabaseHelper.DecisionPackageDefinitions.DefaultSupportCasterWithBackupAttacksDecisions);
            classWarlockBuilder.SetGuiPresentation(classWarlockGuiPresentationBuilder.Build());
            classWarlockBuilder.SetHitDice(RuleDefinitions.DieType.D8);
            classWarlockBuilder.SetIngredientGatheringOdds(Sorcerer.IngredientGatheringOdds);
            classWarlockBuilder.SetPictogram(Wizard.ClassPictogramReference);

            BuildEquipment(classWarlockBuilder);
            BuildProficiencies();
            BuildSpells();
            BuildProgression(classWarlockBuilder);
            BuildSubclasses(classWarlockBuilder);

            ClassWarlock = classWarlockBuilder.AddToDB();
        }
    }
}
