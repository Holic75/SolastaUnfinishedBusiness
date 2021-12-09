using System.Collections.Generic;
using HarmonyLib;
using static SolastaModApi.DatabaseHelper.MetamagicOptionDefinitions;

namespace SolastaUnfinishedBusiness.Patches
{
    // fix twinned spells offering
    internal static class RulesetImplementationManagerLocationPatcher
    {
        [HarmonyPatch(typeof(RulesetImplementationManagerLocation), "IsMetamagicOptionAvailable")]
        internal static class RulesetImplementationManagerLocationIsMetamagicOptionAvailable
        {
            private static readonly List<string> allowedIfBellowHeroLevel5Spells = new List<string>()
            {
            };

            private static readonly List<string> allowedIfNotUpcastSpells = new List<string>()
            {
                // level 1
                "CharmPerson",
                "Longstrider",

                // level 2
                "Blindness",
                "HoldPerson",
                "Invisibility",

                // level 4
                "Banishment",

                // level 5
                "HoldMonster"
            };

            private static readonly List<string> allowedSpells = new List<string>()
            {
                // cantrips
                "AnnoyingBee",
                "ChillTouch",
                "Dazzle",
                "FireBolt",
                "PoisonSpray",
                "RayOfFrost",
                "ShadowDagger",
                "ShockingGrasp",
                "Guidance",
                "Resistance",
                "SacredFlame",
                "SpareTheDying",
                "Shine",

                // level 1
                "HideousLaughter",
                "Jump",
                "MageArmor",
                "ProtectionFromEvilGood",
                "HuntersMark",
                "CureWounds",
                "Heroism",
                "ShieldOfFaith",
                "GuidingBolt",
                "HealingWord",
                "InflictWounds",
                "AnimalFriendship",

                // level 2
                "AcidArrow",
                "Darkvision",
                "Levitate",
                "RayOfEnfeeblement",
                "SpiderClimb",
                "Barkskin",
                "LesserRestoration",
                "ProtectionFromPoison",
                "EnhanceAbility",

                // level 3
                "BestowCurse",
                "DispelMagic",
                "Fly",
                "Haste",
                "ProtectionFromEnergy",
                "RemoveCurse",
                "Tongues",
                "Revivify",

                // level 4
                "Blight",
                "GreaterInvisibility",
                "PhantasmalKiller",
                "Stoneskin",
                "FreedomOfMovement",
                "DeathWard",
                "DominateBeast",

                // level 5
                "DominatePerson",
                "GreaterRestoration",
                "RaiseDead"
            };

            internal static void Postfix(
                ref bool __result,
                RulesetEffectSpell rulesetEffectSpell,
                RulesetCharacter caster,
                MetamagicOptionDefinition metamagicOption,
                ref string failure)
            {
                if (Main.Settings.EnableFixTwinnedLogic && metamagicOption == MetamagicTwinnedSpell)
                {
                    var rulesetSpellRepertoire = rulesetEffectSpell?.SpellRepertoire;
                    var spellDefinition = rulesetEffectSpell?.SpellDefinition;
                    var spellLevel = spellDefinition?.SpellLevel;
                    var slotLevel = rulesetEffectSpell?.SlotLevel;
                    int classLevel;

                    if (rulesetSpellRepertoire != null && spellDefinition != null && rulesetSpellRepertoire.KnownCantrips.Contains(spellDefinition))
                    {
                        classLevel = ((RulesetCharacterHero)caster).ClassesHistory.Count;
                    }
                    else
                    {
                        classLevel = rulesetEffectSpell.GetClassLevel(caster);
                    }

                    var alwaysAllow = allowedSpells.Contains(spellDefinition.Name);
                    var allowIfNotUpcast = allowedIfNotUpcastSpells.Contains(spellDefinition.Name) && spellLevel == slotLevel;
                    var allowIfBellowHeroLevel5 = allowedIfBellowHeroLevel5Spells.Contains(spellDefinition.Name) && classLevel < 5;

                    if (!alwaysAllow && !allowIfNotUpcast && !allowIfBellowHeroLevel5)
                    {
                        var postfix = "";

                        if (allowedIfBellowHeroLevel5Spells.Contains(spellDefinition.Name))
                        {
                            postfix = " above level 4";
                        }
                        else if (allowedIfNotUpcastSpells.Contains(spellDefinition.Name))
                        {
                            postfix = " and upcasted";
                        }

                        failure = $"Cannot be twinned{postfix}";
                        __result = false;
                    }
                }
            }
        }
    }
}
