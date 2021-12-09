using System;
using System.Collections.Generic;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class ClassWarlockSpellListBuilder : BaseDefinitionBuilder<SpellListDefinition>
    {
        private ClassWarlockSpellListBuilder(string name, string guid, GuiPresentation guiPresentation) : base(name, guid)
        {
            var cantrips = new SpellListDefinition.SpellsByLevelDuplet
            {
                Level = 0,
                Spells = new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.AnnoyingBee,
                        DatabaseHelper.SpellDefinitions.ChillTouch,
                        DatabaseHelper.SpellDefinitions.DancingLights,
                        DatabaseHelper.SpellDefinitions.PoisonSpray,
                        DatabaseHelper.SpellDefinitions.TrueStrike
                    }
            };

            var level1 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Level = 1,
                Spells = new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.CharmPerson,
                        DatabaseHelper.SpellDefinitions.ComprehendLanguages,
                        DatabaseHelper.SpellDefinitions.ExpeditiousRetreat,
                        DatabaseHelper.SpellDefinitions.ProtectionFromEvilGood,
                    }
            };

            var level2 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Level = 2,
                Spells = new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.Darkness,
                        DatabaseHelper.SpellDefinitions.HoldPerson,
                        DatabaseHelper.SpellDefinitions.Invisibility,
                        DatabaseHelper.SpellDefinitions.MistyStep,
                        DatabaseHelper.SpellDefinitions.RayOfEnfeeblement,
                        DatabaseHelper.SpellDefinitions.Shatter,
                        DatabaseHelper.SpellDefinitions.SpiderClimb
                    },
            };

            var level3 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Level = 3,
                Spells = new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.Counterspell,
                        DatabaseHelper.SpellDefinitions.DispelMagic,
                        DatabaseHelper.SpellDefinitions.Fear,
                        DatabaseHelper.SpellDefinitions.Fly,
                        DatabaseHelper.SpellDefinitions.HypnoticPattern,
                        DatabaseHelper.SpellDefinitions.RemoveCurse,
                        DatabaseHelper.SpellDefinitions.Tongues,
                        DatabaseHelper.SpellDefinitions.VampiricTouch
                    },
            };

            var level4 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Level = 4,
                Spells = new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.Banishment,
                        DatabaseHelper.SpellDefinitions.Blight,
                        DatabaseHelper.SpellDefinitions.DimensionDoor
                    },
            };

            var level5 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Level = 5,
                Spells = new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.HoldMonster,
                        DatabaseHelper.SpellDefinitions.MindTwist
                    },
            };

            Definition.SetHasCantrips(true);
            Definition.SetGuiPresentation(guiPresentation);
            Definition.SetMaxSpellLevel(5);

            Definition.SpellsByLevel.Add(cantrips);
            Definition.SpellsByLevel.Add(level1);
            Definition.SpellsByLevel.Add(level2);
            Definition.SpellsByLevel.Add(level3);
            Definition.SpellsByLevel.Add(level4);
            Definition.SpellsByLevel.Add(level5);
        }

        public static SpellListDefinition Build()
        {
            var spellListName = "SpellListClassWarlock";
            var spellListGuid = GuidHelper.Create(new Guid(Settings.GUID), spellListName).ToString();

            return new ClassWarlockSpellListBuilder(spellListName, spellListGuid, new GuiPresentationBuilder("Feature/&NoContentTitle", "SpellList/&SpellListClassWarlockDescription").Build()).AddToDB();
        }
    }
}
