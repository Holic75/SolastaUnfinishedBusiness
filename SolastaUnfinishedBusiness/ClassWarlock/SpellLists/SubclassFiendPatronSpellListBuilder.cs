using System;
using System.Collections.Generic;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class SubclassFiendPatronSpellListBuilder : BaseDefinitionBuilder<SpellListDefinition>
    {
        private SubclassFiendPatronSpellListBuilder(string name, string guid, GuiPresentation guiPresentation) : base(name, guid)
        {
            var cantrips = new SpellListDefinition.SpellsByLevelDuplet
            {
                Level = 0,
                Spells = new List<SpellDefinition>
                {

                }
            };

            var level1 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Level = 1,
                Spells = new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.BurningHands,
                        DatabaseHelper.SpellDefinitions.Bane,
                    }
            };

            var level2 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Level = 2,
                Spells = new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.Blindness,
                        DatabaseHelper.SpellDefinitions.ScorchingRay,
                    },
            };

            var level3 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Level = 3,
                Spells = new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.Fireball,
                        DatabaseHelper.SpellDefinitions.StinkingCloud,
                    },
            };

            var level4 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Level = 4,
                Spells = new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.FireShield,
                        DatabaseHelper.SpellDefinitions.WallOfFire,
                    },
            };

            var level5 = new SpellListDefinition.SpellsByLevelDuplet
            {
                Level = 5,
                Spells = new List<SpellDefinition>
                    {
                        DatabaseHelper.SpellDefinitions.FlameStrike,
                        DatabaseHelper.SpellDefinitions.CloudKill,
                    },
            };

            Definition.SetHasCantrips(false);
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
            var spellListName = "SpellListClassWarlockSubclassFiendPatron";
            var spellListGuid = GuidHelper.Create(new Guid(Settings.GUID), spellListName).ToString();

            return new SubclassFiendPatronSpellListBuilder(spellListName, spellListGuid, new GuiPresentationBuilder("Feature/&NoContentTitle", "SpellList/&SpellListClassWarlockDescription").Build()).AddToDB();
        }
    }
}
