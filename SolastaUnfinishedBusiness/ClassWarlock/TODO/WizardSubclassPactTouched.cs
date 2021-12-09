using System;
using System.Collections.Generic;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    public static class WizardSubclassPactTouched
    {
        public static Guid WizardSubclassPactTouchedGuid = new Guid("9ac9c013-eeab-4960-9548-ddf713093032");
        private const string WizardSubclassPactTouchedName = "ZSWizardSubclassPactTouched";
        private static readonly string WizardSubclassPactTouchedNameGuid = GuidHelper.Create(WizardSubclassPactTouched.WizardSubclassPactTouchedGuid, WizardSubclassPactTouchedName).ToString();

        public static CharacterSubclassDefinition Build()
        {
            var subclassGuiPresentation = new GuiPresentationBuilder(
                    "Subclass/&ZSWizardSubclassPactTouchedDescription",
                    "Subclass/&ZSWizardSubclassPactTouchedTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.RoguishDarkweaver.GuiPresentation.SpriteReference)
                    .Build();

            var definition = new CharacterSubclassDefinitionBuilder(WizardSubclassPactTouchedName, WizardSubclassPactTouchedNameGuid)
                    .SetGuiPresentation(subclassGuiPresentation)
                    .AddFeatureAtLevel(PactTouchedSubclassAgonizingBlassBonusCantripBuilder.AgonizingBlastBonusCantrip, 2)
                    .AddFeatureAtLevel(PactTouchedSubclassBuildAutoPreparedSpellsBuilder.GetOrAdd(DatabaseHelper.CharacterClassDefinitions.Wizard), 2)
                    //.AddFeatureAtLevel(PactMarkedFeatPowerBuilder.PactMarkedPower, 2)
                    .AddFeatureAtLevel(PactTouchedSummonPactWeaponPowerBuilder.SummonPactWeaponPower, 6)
                    .AddFeatureAtLevel(PactTouchedSoulTakerPowerBuilder.SoulTakerPower, 10)
                    .AddToDB();

            return definition;
        }
        internal class PactTouchedSubclassBuildAutoPreparedSpellsBuilder : BaseDefinitionBuilder<FeatureDefinitionAutoPreparedSpells>
        {
            private const string PactTouchedBuildAutoPreparedSpellsName = "ZSPactTouchedSubclassBuildAutoPreparedSpells";

            protected PactTouchedSubclassBuildAutoPreparedSpellsBuilder(CharacterClassDefinition characterClass, string name, string guid) : base(DatabaseHelper.FeatureDefinitionAutoPreparedSpellss.AutoPreparedSpellsDomainBattle, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&ZSPactTouchedBuildAutoPreparedSpellsTitle";
                Definition.GuiPresentation.Description = "Feature/&ZSPactTouchedBuildAutoPreparedSpellsDescription";
                Definition.SetSpellcastingClass(characterClass);
                Definition.AutoPreparedSpellsGroups.Clear();
                Definition.AutoPreparedSpellsGroups.Add(new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
                {
                    SpellsList = new List<SpellDefinition>
                    { AgonizingBlastSpellBuilder.AgonizingBlastSpell, PactMarkSpellBuilder.PactMarkSpell, DatabaseHelper.SpellDefinitions.Blindness, DatabaseHelper.SpellDefinitions.Fear, DatabaseHelper.SpellDefinitions.BlackTentacles, DatabaseHelper.SpellDefinitions.MindTwist},
                    ClassLevel = 0
                });
            }

            public static FeatureDefinitionAutoPreparedSpells CreateAndAddToDB(CharacterClassDefinition characterClass)
            {
                return new PactTouchedSubclassBuildAutoPreparedSpellsBuilder(characterClass, PactTouchedBuildAutoPreparedSpellsName + characterClass.Name, GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, PactTouchedBuildAutoPreparedSpellsName + characterClass.Name).ToString()).AddToDB();
            }

            public static FeatureDefinitionAutoPreparedSpells GetOrAdd(CharacterClassDefinition characterClass)
            {
                var db = DatabaseRepository.GetDatabase<FeatureDefinitionAutoPreparedSpells>();
                return db.TryGetElement(PactTouchedBuildAutoPreparedSpellsName + characterClass.Name, GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, PactTouchedBuildAutoPreparedSpellsName + characterClass.Name).ToString()) ?? CreateAndAddToDB(characterClass);
            }
        }

        internal class PactTouchedSubclassAgonizingBlassBonusCantripBuilder : BaseDefinitionBuilder<FeatureDefinitionBonusCantrips>
        {
            private const string PactTouchedBonusCantripsName = "ZSPactTouchedSubclassAgonizingBlassBonusCantrip";
            private static readonly string PactTouchedBonusCantripsGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, PactTouchedBonusCantripsName).ToString();

            protected PactTouchedSubclassAgonizingBlassBonusCantripBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionBonusCantripss.BonusCantripsDomainSun, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&ZSPactTouchedSubclassAgonizingBlassBonusCantripTitle";
                Definition.GuiPresentation.Description = "Feature/&ZSPactTouchedSubclassAgonizingBlassBonusCantripDescription";
                Definition.BonusCantrips.Clear();
                Definition.BonusCantrips.Add(AgonizingBlastSpellBuilder.AgonizingBlastSpell);
            }

            public static FeatureDefinitionBonusCantrips CreateAndAddToDB(string name, string guid)
            {
                return new PactTouchedSubclassAgonizingBlassBonusCantripBuilder(name, guid).AddToDB();
            }

            public static FeatureDefinitionBonusCantrips AgonizingBlastBonusCantrip = CreateAndAddToDB(PactTouchedBonusCantripsName, PactTouchedBonusCantripsGuid);
        }

        internal class PactTouchSubclassPactSoulTakerAdditionalDamageBuilder : BaseDefinitionBuilder<FeatureDefinitionAdditionalDamage>
        {
            private const string PactTouchSubclassPactSoulTakerAdditionalDamageBuilderName = "ZSPactTouchSubclassPactSoulTakerAdditionalDamage";
            private static readonly string PactTouchSubclassPactSoulTakerAdditionalDamageGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, PactTouchSubclassPactSoulTakerAdditionalDamageBuilderName).ToString();

            protected PactTouchSubclassPactSoulTakerAdditionalDamageBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageHuntersMark, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&ZSPactTouchSubclassPactSoulTakerAdditionalDamageTitle";
                Definition.GuiPresentation.Description = "Feature/&ZSPactTouchSubclassPactSoulTakerAdditionalDamageDescription";
                Definition.SetAttackModeOnly(false);
                Definition.SetNotificationTag("PactSoulTaker");
                Definition.SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.TargetIsWounded);
                Definition.SetDamageDieType(RuleDefinitions.DieType.D8);
            }

            public static FeatureDefinitionAdditionalDamage CreateAndAddToDB(string name, string guid)
            {
                return new PactTouchSubclassPactSoulTakerAdditionalDamageBuilder(name, guid).AddToDB();
            }

            public static FeatureDefinitionAdditionalDamage PactTouchSubclassPactSoulTakerAdditionalDamage = CreateAndAddToDB(PactTouchSubclassPactSoulTakerAdditionalDamageBuilderName, PactTouchSubclassPactSoulTakerAdditionalDamageGuid);
        }

        internal class PactTouchedSoulTakerConditionBuilder : BaseDefinitionBuilder<ConditionDefinition>
        {
            private const string PactTouchedSoulTakerConditionName = "ZSPactTouchedSoulTakerCondition";
            private static readonly string PactTouchedSoulTakerConditionGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, PactTouchedSoulTakerConditionName).ToString();

            protected PactTouchedSoulTakerConditionBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionHeraldOfBattle, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&ZSPactTouchedSoulTakerConditionTitle";
                Definition.GuiPresentation.Description = "Feature/&ZSPactTouchedSoulTakerConditionDescription";
                Definition.Features.Clear();
                Definition.Features.Add(PactTouchSubclassPactSoulTakerAdditionalDamageBuilder.PactTouchSubclassPactSoulTakerAdditionalDamage);
                Definition.SetDurationType(RuleDefinitions.DurationType.Minute);
                Definition.SetDurationParameter(1);
            }

            public static ConditionDefinition CreateAndAddToDB(string name, string guid)
            {
                return new PactTouchedSoulTakerConditionBuilder(name, guid).AddToDB();
            }

            public static ConditionDefinition SoulTakerCondition = CreateAndAddToDB(PactTouchedSoulTakerConditionName, PactTouchedSoulTakerConditionGuid);
        }

        internal class PactTouchedSoulTakerPowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
        {
            private const string PactTouchedSoulTakerPowerName = "ZSPactTouchedSoulTakerPower";
            private static readonly string PactTouchedSoulTakerPowerGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, PactTouchedSoulTakerPowerName).ToString();

            protected PactTouchedSoulTakerPowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalFireBurst, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&ZSPactTouchedSoulTakerPowerTitle";
                Definition.GuiPresentation.Description = "Feature/&ZSPactTouchedSoulTakerPowerDescription";
                Definition.SetShortTitleOverride("Feature/&ZSPactTouchedSoulTakerPowerTitle");

                Definition.SetRechargeRate(RuleDefinitions.RechargeRate.LongRest);
                Definition.SetActivationTime(RuleDefinitions.ActivationTime.BonusAction);
                Definition.SetCostPerUse(1);
                Definition.SetFixedUsesPerRecharge(1);

                //Create the power attack effect
                var soulTakerConditionEffectForm = new EffectForm
                {
                    ConditionForm = new ConditionForm(),
                    FormType = EffectForm.EffectFormType.Condition
                };
                soulTakerConditionEffectForm.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
                soulTakerConditionEffectForm.ConditionForm.ConditionDefinition = PactTouchedSoulTakerConditionBuilder.SoulTakerCondition;

                //Add to our new effect
                var newEffectDescription = new EffectDescription();
                newEffectDescription.Copy(Definition.EffectDescription);
                newEffectDescription.EffectForms.Clear();
                newEffectDescription.EffectForms.Add(soulTakerConditionEffectForm);
                newEffectDescription.HasSavingThrow = false;
                newEffectDescription.DurationType = RuleDefinitions.DurationType.Minute;
                newEffectDescription.DurationParameter = 1;
                newEffectDescription.SetTargetSide(RuleDefinitions.Side.Ally);
                newEffectDescription.SetTargetType(RuleDefinitions.TargetType.Self);
                newEffectDescription.SetCanBePlacedOnCharacter(true);

                Definition.SetEffectDescription(newEffectDescription);
            }

            public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
            {
                return new PactTouchedSoulTakerPowerBuilder(name, guid).AddToDB();
            }

            public static FeatureDefinitionPower SoulTakerPower
                = CreateAndAddToDB(PactTouchedSoulTakerPowerName, PactTouchedSoulTakerPowerGuid);
        }


        public class PactTouchedSummonPactWeaponPowerBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
        {
            private const string PactTouchedSummonPactWeaponPowerName = "ZSPactTouchedSummonPactWeaponPower";
            private static readonly string PactTouchedSummonPactWeaponPowerGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, PactTouchedSummonPactWeaponPowerName).ToString();

            protected PactTouchedSummonPactWeaponPowerBuilder(string name, string guid) : base(DatabaseHelper.FeatureDefinitionPowers.PowerTraditionShockArcanistArcaneFury, name, guid)
            {
                Definition.GuiPresentation.Title = "Feature/&ZSPactTouchedSummonPactWeaponPowerTitle";
                Definition.GuiPresentation.Description = "Feature/&ZSPactTouchedSummonPactWeaponPowerDescription";
                Definition.GuiPresentation.SetSpriteReference(DatabaseHelper.SpellDefinitions.SpiritualWeapon.GuiPresentation.SpriteReference);
                Definition.SetEffectDescription(DatabaseHelper.SpellDefinitions.SpiritualWeapon.EffectDescription);
                Definition.SetRechargeRate(RuleDefinitions.RechargeRate.ShortRest);
                Definition.SetActivationTime(RuleDefinitions.ActivationTime.NoCost);
            }

            public static FeatureDefinitionPower CreateAndAddToDB(string name, string guid)
            {
                return new PactTouchedSummonPactWeaponPowerBuilder(name, guid).AddToDB();
            }

            public static FeatureDefinitionPower SummonPactWeaponPower = CreateAndAddToDB(PactTouchedSummonPactWeaponPowerName, PactTouchedSummonPactWeaponPowerGuid);
        }

    }
}
