using HarmonyLib;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaUnfinishedBusiness.Patches
{
    internal static class RulesetImplementationManagerPatcher
    {
        internal static RulesetCharacterHero HeroWithSpellRepertoire { get; set; }
        internal static RulesetSpellRepertoire SpellRepertoire { get; set; }

        // gets hero and repertoire context to be used later on FlexibleCastingItem / ensures Wizard / Sorcerer are using caster level
        [HarmonyPatch(typeof(RulesetImplementationManager), "ApplySpellSlotsForm")]
        internal static class RulesetImplementationManagerApplySpellSlotsForm
        {
            internal static bool Prefix(EffectForm effectForm, RulesetImplementationDefinitions.ApplyFormsParams formsParams)
            {
                var spellSlotsForm = effectForm.SpellSlotsForm;

                if (spellSlotsForm.Type == SpellSlotsForm.EffectType.RecoverHalfLevelUp)
                {
                    var sourceCharacter = formsParams.sourceCharacter as RulesetCharacterHero;

                    foreach (var spellRepertoire in sourceCharacter.SpellRepertoires)
                    {
                        if (spellRepertoire.SpellCastingClass != null || spellRepertoire.SpellCastingSubclass != null)
                        {
                            var currentValue = 0;

                            switch (formsParams.activeEffect.Name)
                            {
                                case "PowerCircleLandNaturalRecovery":
                                    currentValue = sourceCharacter.ClassesAndLevels[Druid];
                                    break;

                                case "PowerWizardArcaneRecovery":
                                    currentValue = sourceCharacter.ClassesAndLevels[Wizard];
                                    break;

                                default:
                                    currentValue = sourceCharacter.GetAttribute("CharacterLevel").CurrentValue;
                                    break;
                            }

                            var slotsCapital = currentValue % 2 == 0 ? currentValue / 2 : (currentValue + 1) / 2;

                            Gui.GuiService.GetScreen<SlotRecoveryModal>().ShowSlotRecovery(sourceCharacter, formsParams.activeEffect.SourceDefinition.Name, spellRepertoire, slotsCapital, spellSlotsForm.MaxSlotLevel);
                            break;
                        }
                    }
                }
                else if (spellSlotsForm.Type == SpellSlotsForm.EffectType.CreateSpellSlot || spellSlotsForm.Type == SpellSlotsForm.EffectType.CreateSorceryPoints)
                {
                    var sourceCharacter = formsParams.sourceCharacter as RulesetCharacterHero;

#pragma warning disable S125 // Sections of code should not be commented out
                    //foreach (RulesetSpellRepertoire spellRepertoire in sourceCharacter.SpellRepertoires)
                    //{
                    //    if ((BaseDefinition)spellRepertoire.SpellCastingClass != (BaseDefinition)null || (BaseDefinition)spellRepertoire.SpellCastingSubclass != (BaseDefinition)null)
                    //    {
                    //        Gui.GuiService.GetScreen<FlexibleCastingModal>().ShowFlexibleCasting((RulesetCharacter)sourceCharacter, spellRepertoire, spellSlotsForm.Type == SpellSlotsForm.EffectType.CreateSpellSlot);
                    //        break;
                    //    }
                    //}
#pragma warning restore S125 // Sections of code should not be commented out

                    HeroWithSpellRepertoire = sourceCharacter;
                    SpellRepertoire = sourceCharacter.SpellRepertoires.Find(sr => sr.SpellCastingClass == Sorcerer);
                    Gui.GuiService.GetScreen<FlexibleCastingModal>().ShowFlexibleCasting(sourceCharacter, SpellRepertoire, spellSlotsForm.Type == SpellSlotsForm.EffectType.CreateSpellSlot);
                }
                else if (spellSlotsForm.Type == SpellSlotsForm.EffectType.GainSorceryPoints)
                {
                    (formsParams.sourceCharacter as RulesetCharacterHero).GainSorceryPoints(spellSlotsForm.SorceryPointsGain);
                }
                else
                {
                    if (spellSlotsForm.Type == SpellSlotsForm.EffectType.RecovererSorceryHalfLevelUp)
                    {
                        var sourceCharacter = formsParams.sourceCharacter as RulesetCharacterHero;

#pragma warning disable S125 // Sections of code should not be commented out
                        //int currentValue = sourceCharacter.GetAttribute("CharacterLevel").CurrentValue;
#pragma warning restore S125 // Sections of code should not be commented out
                        var currentValue = sourceCharacter.ClassesAndLevels[Sorcerer];
                        var sorceryPointsGain = currentValue % 2 == 0 ? currentValue / 2 : (currentValue + 1) / 2;
                        sourceCharacter.GainSorceryPoints(sorceryPointsGain);
                    }
                }

                return false;
            }
        }
    }
}
