using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.RestActivityDefinitions;

namespace SolastaUnfinishedBusiness.Builders
{
    public class RestActivityLevelDownBuilder : BaseDefinitionBuilder<RestActivityDefinition>
    {
        private const string LevelDownName = "ZSLevelDown";
        private const string LevelDownGuid = "fdb4d86eaef942d1a22dbf1fb5a7299f";

        protected RestActivityLevelDownBuilder(string name, string guid) : base(LevelUp, name, guid)
        {
            Definition.GuiPresentation.Title = "RestActivity/&ZSLevelDownTitle";
            Definition.GuiPresentation.Description = "RestActivity/&ZSLevelDownDescription";
            Definition.SetCondition<RestActivityDefinition>(Settings.ActivityConditionCanLevelDown);
            Definition.SetFunctor<RestActivityDefinition>(LevelDownName);
            ServiceRepository.GetService<IFunctorService>().RegisterFunctor(LevelDownName, new FunctorLevelDown());
        }

        private static RestActivityDefinition CreateAndAddToDB(string name, string guid)
        {
            return new RestActivityLevelDownBuilder(name, guid).AddToDB();
        }

        public static readonly RestActivityDefinition RestActivityLevelDown
            = CreateAndAddToDB(LevelDownName, LevelDownGuid);
    }

    internal class FunctorLevelDown : Functor
    {
        public override IEnumerator Execute(
          FunctorParametersDescription functorParameters,
          Functor.FunctorExecutionContext context)
        {
            yield return ConfirmationModal("Message/&ZSLevelDownConfirmationTitle", "Message/&ZSLevelDownConfirmationDescription", functorParameters.RestingHero);
        }

        internal static void ConfirmAndExecute(string filename)
        {
            Gui.GuiService.ShowMessage(
                MessageModal.Severity.Attention2,
                "Message/&ZSLevelDownConfirmationTitle", "Message/&ZSLevelDownConfirmationDescription",
                "Message/&MessageYesTitle", "Message/&MessageNoTitle",
                () =>
                {
                    var service = ServiceRepository.GetService<ICharacterPoolService>();

                    if (service != null)
                    {
                        service.LoadCharacter(filename, out var rulesetCharacterHero, out _);
                        LevelDown(rulesetCharacterHero);
                    }
                },
                null);
        }

        internal static IEnumerator ConfirmationModal(string confirmationTitle, string confirmationDescription, RulesetCharacterHero rulesetCharacterHero)
        {
            var state = -1;
            Gui.GuiService.ShowMessage(
                MessageModal.Severity.Attention2,
                confirmationTitle, confirmationDescription,
                "Message/&MessageYesTitle", "Message/&MessageNoTitle",
                new MessageModal.MessageValidatedHandler(() => { state = 1; }),
                new MessageModal.MessageCancelledHandler(() => { state = 0; }));

            while (state < 0)
            {
                yield return null;
            }

            if (state > 0)
            {
                LevelDown(rulesetCharacterHero);
            }
        }

        internal static void LevelDown(RulesetCharacterHero hero)
        {
            var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();
            var characterClassDefinition = hero.ClassesHistory[hero.ClassesHistory.Count - 1];
            var classesAndLevel = hero.ClassesAndLevels[characterClassDefinition];
            var classTag = AttributeDefinitions.GetClassTag(characterClassDefinition, classesAndLevel);
            var subclassTag = string.Empty;

            hero.ClassesAndSubclasses.TryGetValue(characterClassDefinition, out var characterSubclassDefinition);

            if ((new List<string> { "AlchemistClass", "BardClass", "MonkClass", "WarlockClass" }).Contains(characterClassDefinition.Name))
            {
                Gui.GuiService.ShowMessage(MessageModal.Severity.Informative1, "Level Down", $"Class {characterClassDefinition.FormatTitle()} is unsupported.", "Message/&MessageOkTitle", string.Empty, null, null);

                return;
            }

            if (characterSubclassDefinition != null)
            {
                subclassTag = AttributeDefinitions.GetSubclassTag(characterClassDefinition, classesAndLevel, characterSubclassDefinition);
            }

            // adds context
            AccessTools.Field(characterBuildingService.GetType(), "heroCharacter").SetValue(characterBuildingService, hero);
            Models.LevelUpContext.SelectedHero = hero;
            Models.LevelUpContext.SelectedClass = characterClassDefinition;
            Models.LevelUpContext.SelectedSubclass = characterSubclassDefinition;

            // collects current class spell repertoire
            var heroRepertoire = hero.SpellRepertoires.FirstOrDefault(x => Models.SpellContext.IsRepertoireFromSelectedClassSubclass(x));

            // removes feature from stack based on pool type
            void RemoveFeatureDefinitionPointPool(FeatureDefinitionPointPool featureDefinitionPointPool)
            {
                var poolAmount = featureDefinitionPointPool.PoolAmount;

                switch (featureDefinitionPointPool.PoolType)
                {
                    case HeroDefinitions.PointsPoolType.Cantrip:
                        for (var i = poolAmount; i > 0; i--)
                        {
                            heroRepertoire.KnownCantrips.RemoveAt(heroRepertoire.KnownCantrips.Count - 1);
                        }

                        break;

                    case HeroDefinitions.PointsPoolType.Expertise:
                        for (var i = poolAmount; i > 0; i--)
                        {
                            hero.TrainedExpertises.RemoveAt(hero.TrainedExpertises.Count - 1);
                        }

                        break;

                    case HeroDefinitions.PointsPoolType.Feat:
                        for (var i = poolAmount; i > 0; i--)
                        {
                            hero.TrainedFeats.RemoveAt(hero.TrainedFeats.Count - 1);
                        }

                        break;

                    case HeroDefinitions.PointsPoolType.Language:
                        for (var i = poolAmount; i > 0; i--)
                        {
                            hero.TrainedLanguages.RemoveAt(hero.TrainedLanguages.Count - 1);
                        }

                        break;

                    case HeroDefinitions.PointsPoolType.Skill:
                        for (var i = poolAmount; i > 0; i--)
                        {
                            hero.TrainedSkills.RemoveAt(hero.TrainedSkills.Count - 1);
                        }

                        break;

                    case HeroDefinitions.PointsPoolType.Metamagic:
                        for (var i = poolAmount; i > 0; i--)
                        {
                            hero.TrainedMetamagicOptions.RemoveAt(hero.TrainedMetamagicOptions.Count - 1);
                        }

                        break;

                    case HeroDefinitions.PointsPoolType.Spell:
                        for (var i = poolAmount; i > 0; i--)
                        {
                            heroRepertoire.KnownSpells.RemoveAt(heroRepertoire.KnownSpells.Count - 1);
                        }

                        break;

                    case HeroDefinitions.PointsPoolType.Tool:
                        for (var i = poolAmount; i > 0; i--)
                        {
                            hero.TrainedToolTypes.RemoveAt(hero.TrainedToolTypes.Count - 1);
                        }

                        break;
                }
            }

            // removes features
            void RemoveFeatures(string tag)
            {
                if (hero.ActiveFeatures.ContainsKey(tag))
                {
                    foreach (var activeFeature in hero.ActiveFeatures[tag])
                    {
                        if (activeFeature is FeatureDefinitionCastSpell)
                        {
                            if (heroRepertoire != null)
                            {
                                hero.SpellRepertoires.Remove(heroRepertoire);
                            }
                        }
                        else if (activeFeature is FeatureDefinitionFightingStyleChoice)
                        {
                            hero.TrainedFightingStyles.RemoveAt(hero.TrainedFightingStyles.Count - 1);
                        }
                        else if (activeFeature is FeatureDefinitionSubclassChoice)
                        {
                            hero.ClassesAndSubclasses.Remove(characterClassDefinition);
                        }
                        else if (activeFeature is FeatureDefinitionFeatureSet featureDefinitionFeatureSet)
                        {
                            if (featureDefinitionFeatureSet.Mode == FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                            {
                                foreach (var featureDefinition in featureDefinitionFeatureSet.FeatureSet)
                                {
                                    if (featureDefinition is FeatureDefinitionPointPool featureDefinitionPointPool)
                                    {
                                        RemoveFeatureDefinitionPointPool(featureDefinitionPointPool);
                                    }
                                }
                            }
                        }
                        else if (activeFeature is FeatureDefinitionPointPool featureDefinitionPointPool)
                        {
                            RemoveFeatureDefinitionPointPool(featureDefinitionPointPool);
                        }
                    }
                }
            }

            //
            // MAIN
            //

            RemoveFeatures(classTag);
            hero.ActiveFeatures.Remove(classTag);
            hero.ClearFeatureModifiers(classTag);

            if (subclassTag != string.Empty)
            {
                RemoveFeatures(subclassTag);
                hero.ActiveFeatures.Remove(subclassTag);
                hero.ClearFeatureModifiers(subclassTag);
            }

            hero.RemoveClassLevel();
            hero.RefreshActiveFightingStyles();
            hero.RefreshActiveItemFeatures();
            hero.RefreshArmorClass();
            hero.RefreshAttackModes();
            hero.RefreshAttributeModifiersFromConditions();
            hero.RefreshAttributeModifiersFromFeats();
            hero.RefreshAttributes();
            hero.RefreshClimbRules();
            hero.RefreshConditionFlags();
            hero.RefreshEncumberance();
            hero.RefreshJumpRules();
            hero.RefreshMoveModes();
            hero.RefreshPersonalityFlags();
            hero.RefreshPowers();
            hero.RefreshProficiencies();
            hero.RefreshSpellRepertoires();
            hero.RefreshTags();
            hero.RefreshUsableDeviceFunctions();
            hero.ComputeHitPoints(true);

            // removes context
            AccessTools.Field(characterBuildingService.GetType(), "heroCharacter").SetValue(characterBuildingService, null);
            Models.LevelUpContext.SelectedHero = null;

            // saves hero if not in game
            if (Gui.Game == null)
            {
                ServiceRepository.GetService<ICharacterPoolService>().SaveCharacter(hero, true);
            }
        }
    }
}
