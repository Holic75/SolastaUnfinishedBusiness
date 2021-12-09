using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches
{
    public static class GameLocationBattleManagerPatcher
    {
        // fixes rage damage calculation under wildshape
        [HarmonyPatch(typeof(GameLocationBattleManager), "ComputeAndNotifyAdditionalDamage")]
        public static class GameLocationBattleManagerComputeAndNotifyAdditionalDamage
        {
            public static bool IsRulesetCharacterHeroOrSubstitute(RulesetCharacter rulesetCharacter)
            {
                return rulesetCharacter is RulesetCharacterHero || rulesetCharacter.IsSubstitute;
            }

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var found = 0;
                var isRulesetCharacterHeroOrSubstituteMethod = typeof(GameLocationBattleManagerComputeAndNotifyAdditionalDamage).GetMethod("IsRulesetCharacterHeroOrSubstitute");

                foreach (var instruction in instructions)
                {
                    // at this point RulesetCharacter is on stack so I call IsRulesetCharacterHeroOrSubstitute to determine if it is a hero or a substitute
                    if (instruction.opcode == OpCodes.Isinst && instruction.operand.ToString() == "RulesetCharacterHero" && ++found == 2)
                    {
                        yield return new CodeInstruction(OpCodes.Call, isRulesetCharacterHeroOrSubstituteMethod);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }

        // allows class feature damages to be correctly calculated under wildshape
        [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterAttackDamage")]
        internal static class GameLocationBattleManagerHandleCharacterAttackDamage
        {
            internal static IEnumerator<object> Postfix(
                IEnumerator<object> values,
                GameLocationBattleManager __instance,
                GameLocationCharacter attacker,
                GameLocationCharacter defender,
                ActionModifier attackModifier,
                RulesetAttackMode attackMode,
                bool rangedAttack,
                RuleDefinitions.AdvantageType advantageType,
                List<EffectForm> actualEffectForms,
                RulesetEffect rulesetEffect,
                bool criticalHit,
                bool firstTarget)
            {
                var isRelevant = attacker.RulesetCharacter is RulesetCharacterMonster monster && monster.IsSubstitute;

                if (!isRelevant)
                {
                    while (values.MoveNext())
                    {
                        yield return values.Current;
                    }

                    yield break;
                }

                if (defender != null && defender.RulesetActor != null && (defender.RulesetActor is RulesetCharacterMonster || defender.RulesetActor is RulesetCharacterHero))
                {
                    attacker.RulesetCharacter.EnumerateFeaturesToBrowse<IAdditionalDamageProvider>(__instance.featuresToBrowseReaction);
                    if (attacker.RulesetCharacter.CharacterInventory != null && attackMode != null && (attackMode.SourceObject is RulesetItem sourceObject2))
                    {
                        sourceObject2.EnumerateFeaturesToBrowse<IAdditionalDamageProvider>(__instance.featuresToBrowseItem);
                        __instance.featuresToBrowseReaction.AddRange(__instance.featuresToBrowseItem);
                        __instance.featuresToBrowseItem.Clear();
                    }
                    foreach (var featureDefinition in __instance.featuresToBrowseReaction)
                    {
                        var provider = featureDefinition as IAdditionalDamageProvider;
                        if (!provider.AttackModeOnly || attackMode != null)
                        {
                            var flag1 = false;
                            var flag2 = true;
                            if (provider.LimitedUsage != RuleDefinitions.FeatureLimitedUsage.None)
                            {
                                if (provider.LimitedUsage == RuleDefinitions.FeatureLimitedUsage.OnceInMyturn && (attacker.UsedSpecialFeatures.ContainsKey(featureDefinition.Name) || __instance.Battle != null && __instance.Battle.ActiveContender != attacker))
                                {
                                    flag2 = false;
                                }
                                else if (provider.LimitedUsage == RuleDefinitions.FeatureLimitedUsage.OncePerTurn && attacker.UsedSpecialFeatures.ContainsKey(featureDefinition.Name))
                                {
                                    flag2 = false;
                                }
                            }
                            CharacterActionParams reactionParams = null;
                            if (flag2)
                            {
                                if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly && attackMode != null)
                                {
                                    if (advantageType == RuleDefinitions.AdvantageType.Advantage || advantageType != RuleDefinitions.AdvantageType.Disadvantage && __instance.IsConsciousCharacterOfSideNextToCharacter(defender, attacker.Side, attacker))
                                    {
                                        flag1 = true;
                                    }
                                }
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.SpendSpellSlot && attackModifier != null && attackModifier.Proximity == RuleDefinitions.AttackProximity.Melee)
                                {
                                    // BEGIN PATCH
                                    RulesetCharacterHero rulesetCharacter = null;

                                    if (attacker.RulesetCharacter is RulesetCharacterMonster rulesetCharacterMonster && rulesetCharacterMonster.IsSubstitute)
                                    {
                                        var party = ServiceRepository.GetService<IGameService>().Game.GameCampaign.Party;
                                        var name = rulesetCharacterMonster.Name;

                                        rulesetCharacter = party.CharactersList.Find(x => x.RulesetCharacter.Name == name)?.RulesetCharacter as RulesetCharacterHero;
                                    }
                                    else
                                    {
                                        rulesetCharacter = attacker.RulesetCharacter as RulesetCharacterHero;
                                    }
                                    // END PATCH
                                    var classDefinition = rulesetCharacter.FindClassHoldingFeature(featureDefinition);
                                    RulesetSpellRepertoire selectedSpellRepertoire = null;
                                    foreach (var spellRepertoire in rulesetCharacter.SpellRepertoires)
                                    {
                                        if (spellRepertoire.SpellCastingClass == classDefinition)
                                        {
                                            var flag3 = false;
                                            for (var spellLevel = 1; spellLevel <= spellRepertoire.MaxSpellLevelOfSpellCastingLevel; ++spellLevel)
                                            {
                                                spellRepertoire.GetSlotsNumber(spellLevel, out var remaining, out var max);
                                                if (remaining > 0)
                                                {
                                                    selectedSpellRepertoire = spellRepertoire;
                                                    flag3 = true;
                                                    break;
                                                }
                                            }
                                            if (flag3)
                                            {
                                                reactionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendSpellSlot)
                                                {
                                                    IntParameter = 1,
                                                    StringParameter = provider.NotificationTag,
                                                    SpellRepertoire = selectedSpellRepertoire
                                                };
                                                var service = ServiceRepository.GetService<IGameLocationActionService>();
                                                var count = service.PendingReactionRequestGroups.Count;
                                                service.ReactToSpendSpellSlot(reactionParams);
                                                yield return __instance.WaitForReactions(attacker, service, count);
                                                flag1 = reactionParams.ReactionValidated;
                                            }
                                        }
                                    }
                                }
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasConditionCreatedByMe)
                                {
                                    if (defender.RulesetActor.HasConditionOfTypeAndSource(provider.RequiredTargetCondition, attacker.Guid))
                                    {
                                        flag1 = true;
                                    }
                                }
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasCondition)
                                {
                                    if (defender.RulesetActor.HasConditionOfType(provider.RequiredTargetCondition.Name))
                                    {
                                        flag1 = true;
                                    }
                                }
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.TargetDoesNotHaveCondition)
                                {
                                    if (!defender.RulesetActor.HasConditionOfType(provider.RequiredTargetCondition.Name))
                                    {
                                        flag1 = true;
                                    }
                                }
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.TargetIsWounded)
                                {
                                    if (defender.RulesetCharacter != null && defender.RulesetCharacter.CurrentHitPoints < defender.RulesetCharacter.GetAttribute("HitPoints").CurrentValue)
                                    {
                                        flag1 = true;
                                    }
                                }
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasSenseType)
                                {
                                    if (defender.RulesetCharacter != null && defender.RulesetCharacter.HasSenseType(provider.RequiredTargetSenseType))
                                    {
                                        flag1 = true;
                                    }
                                }
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.TargetHasCreatureTag)
                                {
                                    if (defender.RulesetCharacter != null && defender.RulesetCharacter.HasTag(provider.RequiredTargetCreatureTag))
                                    {
                                        flag1 = true;
                                    }
                                }
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.RangeAttackFromHigherGround && attackMode != null)
                                {
                                    if (attacker.LocationPosition.y > defender.LocationPosition.y)
                                    {
                                        var element = DatabaseRepository.GetDatabase<ItemDefinition>().GetElement(attackMode.SourceDefinition.Name, true);
                                        if (element != null && element.IsWeapon && DatabaseRepository.GetDatabase<WeaponTypeDefinition>().GetElement(element.WeaponDescription.WeaponType).WeaponProximity == RuleDefinitions.AttackProximity.Range)
                                        {
                                            flag1 = true;
                                        }
                                    }
                                }
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.SpecificCharacterFamily)
                                {
                                    if (defender.RulesetCharacter != null && defender.RulesetCharacter.CharacterFamily == provider.RequiredCharacterFamily.Name)
                                    {
                                        flag1 = true;
                                    }
                                }
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.CriticalHit)
                                {
                                    flag1 = criticalHit;
                                }
#pragma warning disable S2178 // Short-circuit logic should be used in boolean contexts
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.EvocationSpellDamage & firstTarget && rulesetEffect is RulesetEffectSpell && (rulesetEffect as RulesetEffectSpell).SpellDefinition.SchoolOfMagic == "SchoolEvocation")
#pragma warning restore S2178 // Short-circuit logic should be used in boolean contexts
                                {
                                    flag1 = true;
                                }
#pragma warning disable S2178 // Short-circuit logic should be used in boolean contexts
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.SpellDamageMatchesSourceAncestry & firstTarget && rulesetEffect is RulesetEffectSpell && attacker.RulesetCharacter.HasAncestryMatchingDamageType(actualEffectForms))
#pragma warning restore S2178 // Short-circuit logic should be used in boolean contexts
                                {
                                    flag1 = true;
                                }
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive)
                                {
                                    flag1 = true;
                                }
                                else if (provider.TriggerCondition == RuleDefinitions.AdditionalDamageTriggerCondition.RagingAndTargetIsSpellcaster && defender.RulesetCharacter != null && (attacker.RulesetCharacter.HasConditionOfType("ConditionRaging") && defender.RulesetCharacter.SpellRepertoires.Count > 0))
                                {
                                    flag1 = true;
                                }
                            }
                            var flag4 = true;
                            if (flag1 && provider.RequiredProperty != RuleDefinitions.AdditionalDamageRequiredProperty.None && attackMode != null)
                            {
                                var flag3 = false;
                                var flag5 = false;
                                var flag6 = false;
                                var element = DatabaseRepository.GetDatabase<ItemDefinition>().GetElement(attackMode.SourceDefinition.Name, true);
                                if (element != null && element.IsWeapon)
                                {
                                    if (DatabaseRepository.GetDatabase<WeaponTypeDefinition>().GetElement(element.WeaponDescription.WeaponType).WeaponProximity == RuleDefinitions.AttackProximity.Melee && !rangedAttack)
                                    {
                                        flag5 = true;
                                        if (element.WeaponDescription.WeaponTags.Contains("Finesse"))
                                        {
                                            flag3 = true;
                                        }
                                    }
                                    else
                                    {
                                        flag6 = true;
                                    }
                                }
                                // BEGIN PATCH
                                else if (attacker.RulesetCharacter is RulesetCharacterMonster rulesetCharacterMonster && rulesetCharacterMonster.IsSubstitute)
                                {
                                    flag5 = true;
                                }
                                // END PATCH
                                if (provider.RequiredProperty == RuleDefinitions.AdditionalDamageRequiredProperty.FinesseOrRangeWeapon)
                                {
                                    if (!flag3 && !flag6)
                                    {
                                        flag4 = false;
                                    }
                                }
                                else if (provider.RequiredProperty == RuleDefinitions.AdditionalDamageRequiredProperty.RangeWeapon)
                                {
                                    if (!flag6)
                                    {
                                        flag4 = false;
                                    }
                                }
                                else if (provider.RequiredProperty == RuleDefinitions.AdditionalDamageRequiredProperty.MeleeWeapon)
                                {
                                    if (!flag5)
                                    {
                                        flag4 = false;
                                    }
                                }
                                else if (provider.RequiredProperty == RuleDefinitions.AdditionalDamageRequiredProperty.MeleeStrengthWeapon)
                                {
                                    if (!flag5 || attackMode.AbilityScore != "Strength")
                                    {
                                        flag4 = false;
                                    }
                                }
                                else
                                {
                                    Trace.LogAssertion(string.Format("RequiredProperty {0} not implemented for {1}.", provider.RequiredProperty, provider.TriggerCondition));
                                }
                            }
                            if (flag1 && flag4)
                            {
                                __instance.ComputeAndNotifyAdditionalDamage(attacker, defender, provider, actualEffectForms, reactionParams, attackMode);
                            }
                        }
                    }
                    if (attacker.RulesetCharacter.UsablePowers.Count > 0)
                    {
                        foreach (var usablePower in attacker.RulesetCharacter.UsablePowers)
                        {
                            if (!attacker.RulesetCharacter.IsPowerOverriden(usablePower) && attacker.RulesetCharacter.GetRemainingUsesOfPower(usablePower) > 0 && (usablePower.PowerDefinition.ActivationTime == RuleDefinitions.ActivationTime.OnAttackHit && attackMode != null || usablePower.PowerDefinition.ActivationTime == RuleDefinitions.ActivationTime.OnAttackHitWithBow && attackMode != null && attacker.RulesetCharacter.IsWieldingBow()))
                            {
                                var reactionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendPower)
                                {
                                    StringParameter = usablePower.PowerDefinition.Name
                                };
                                if (usablePower.PowerDefinition.OverriddenPower != null)
                                {
                                    reactionParams.StringParameter = reactionParams.StringParameter.Trim(GameLocationBattleManager.DigitsToTrim);
                                }

                                var service1 = ServiceRepository.GetService<IRulesetImplementationService>();
                                reactionParams.RulesetEffect = service1.InstantiateEffectPower(attacker.RulesetCharacter, usablePower, false);
                                reactionParams.TargetCharacters.Add(defender);
                                reactionParams.IsReactionEffect = true;
                                var service2 = ServiceRepository.GetService<IGameLocationActionService>();
                                var count = service2.PendingReactionRequestGroups.Count;
                                service2.ReactToSpendPower(reactionParams);
                                yield return __instance.WaitForReactions(attacker, service2, count);
                            }
                            else if (attacker.RulesetCharacter.GetRemainingUsesOfPower(usablePower) > 0 && usablePower.PowerDefinition.ActivationTime == RuleDefinitions.ActivationTime.OnAttackSpellHitAutomatic)
                            {
                                var actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendPower)
                                {
                                    StringParameter = usablePower.PowerDefinition.Name
                                };
                                var service = ServiceRepository.GetService<IRulesetImplementationService>();
                                actionParams.RulesetEffect = service.InstantiateEffectPower(attacker.RulesetCharacter, usablePower, false);
                                actionParams.TargetCharacters.Add(defender);
                                ServiceRepository.GetService<IGameLocationActionService>().ExecuteAction(actionParams, null, true);
                            }
                        }
                    }
                    if (attackMode != null && attackMode.Ranged && defender.GetActionStatus(ActionDefinitions.Id.DeflectMissile, ActionDefinitions.ActionScope.Battle, ActionDefinitions.ActionStatus.Available) == ActionDefinitions.ActionStatus.Available)
                    {
                        var reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.DeflectMissile);
                        reactionParams.ActionModifiers.Add(attackModifier);
                        reactionParams.TargetCharacters.Add(attacker);
                        var service = ServiceRepository.GetService<IGameLocationActionService>();
                        var count = service.PendingReactionRequestGroups.Count;
                        service.ReactToDeflectMissile(reactionParams);
                        yield return __instance.WaitForReactions(attacker, service, count);
                    }
                    if (defender.GetActionTypeStatus(ActionDefinitions.ActionType.Reaction) == ActionDefinitions.ActionStatus.Available)
                    {
                        if (defender.GetActionStatus(ActionDefinitions.Id.UncannyDodge, ActionDefinitions.ActionScope.Battle, ActionDefinitions.ActionStatus.Available) == ActionDefinitions.ActionStatus.Available && defender.PerceivedFoes.Contains(attacker))
                        {
                            var reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.UncannyDodge);
                            reactionParams.ActionModifiers.Add(attackModifier);
                            reactionParams.TargetCharacters.Add(attacker);
                            var service = ServiceRepository.GetService<IGameLocationActionService>();
                            var count = service.PendingReactionRequestGroups.Count;
                            service.ReactToUncannyDodge(reactionParams);
                            yield return __instance.WaitForReactions(attacker, service, count);
                        }
#pragma warning disable S3358 // Ternary operators should not be nested
                        if (((defender.GetActionStatus(ActionDefinitions.Id.LeafScales, ActionDefinitions.ActionScope.Battle, ActionDefinitions.ActionStatus.Available) != ActionDefinitions.ActionStatus.Available ? 0 : (defender.PerceivedFoes.Contains(attacker) ? 1 : 0)) & (rangedAttack ? 1 : 0)) != 0)
#pragma warning restore S3358 // Ternary operators should not be nested
                        {
                            var reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.LeafScales);
                            reactionParams.ActionModifiers.Add(attackModifier);
                            reactionParams.TargetCharacters.Add(attacker);
                            var service = ServiceRepository.GetService<IGameLocationActionService>();
                            var count = service.PendingReactionRequestGroups.Count;
                            service.ReactToLeafScales(reactionParams);
                            yield return __instance.WaitForReactions(attacker, service, count);
                        }
                    }
                    if (defender.RulesetCharacter != null)
                    {
                        defender.RulesetCharacter.EnumerateFeaturesToBrowse<IDamageAffinityProvider>(__instance.featuresToBrowseReaction);
#pragma warning disable S3217 // "Explicit" conversions of "foreach" loops should not be used
                        foreach (IDamageAffinityProvider damageAffinityProvider in __instance.featuresToBrowseReaction)
#pragma warning restore S3217 // "Explicit" conversions of "foreach" loops should not be used
                        {
                            if (damageAffinityProvider.RetaliateWhenHit && attackMode != null && (attackMode.Ranged && damageAffinityProvider.RetaliateProximity == RuleDefinitions.AttackProximity.Range || !attackMode.Ranged && damageAffinityProvider.RetaliateProximity == RuleDefinitions.AttackProximity.Melee) && (__instance.IsWithinXCells(attacker, defender, damageAffinityProvider.RetaliateRangeCells) && damageAffinityProvider.RetaliatePower != null))
                            {
                                var damageRetaliated = defender.RulesetCharacter.DamageRetaliated;
                                if (damageRetaliated != null)
                                {
                                    damageRetaliated(defender.RulesetCharacter, attacker.RulesetCharacter, damageAffinityProvider);
                                }

                                var actionParams = new CharacterActionParams(defender, ActionDefinitions.Id.SpendPower, attacker);
                                var usablePower = new RulesetUsablePower(damageAffinityProvider.RetaliatePower, null, null);
                                var service = ServiceRepository.GetService<IRulesetImplementationService>();
                                actionParams.RulesetEffect = service.InstantiateEffectPower(defender.RulesetCharacter, usablePower, false);
                                actionParams.StringParameter = damageAffinityProvider.RetaliatePower.Name;
                                actionParams.IsReactionEffect = true;
                                ServiceRepository.GetService<IGameLocationActionService>().ExecuteInstantSingleAction(actionParams);
                            }
                        }
                        yield return __instance.HandleReactionToDamage(attacker, defender, attackModifier, actualEffectForms);
                    }
                }
            }
        }
    }
}
