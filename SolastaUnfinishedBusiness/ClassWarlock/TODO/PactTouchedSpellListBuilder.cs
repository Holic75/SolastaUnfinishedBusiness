using System.Collections.Generic;
using SolastaModApi;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class PactTouchedSpellListBuilder : BaseDefinitionBuilder<SpellListDefinition>
    {
        private const string PactTouchedSpellListName = "ZSPactTouchedSpellList";
        private static readonly string PactTouchedSpellListGuid = GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, PactTouchedSpellListName).ToString();

        protected PactTouchedSpellListBuilder(string name, string guid) : base(DatabaseHelper.SpellListDefinitions.SpellListWizardGreenmage, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ZSPactTouchedSpellListTitle";
            Definition.GuiPresentation.Description = "Feat/&ZSPactTouchedSpellListDescription";
            Definition.SpellsByLevel.Clear();
            Definition.SpellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet() { Spells = new List<SpellDefinition>() { EldritchBlastSpellBuilder.EldritchBlastSpell }, Level = 0 });
            Definition.SpellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet() { Spells = new List<SpellDefinition>() { PactMarkSpellBuilder.PactMarkSpell }, Level = 1 });
            Definition.SpellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet() { Spells = new List<SpellDefinition>() { DatabaseHelper.SpellDefinitions.Shatter }, Level = 2 });
        }

        public static SpellListDefinition CreateAndAddToDB(string name, string guid)
        {
            return new PactTouchedSpellListBuilder(name, guid).AddToDB();
        }

        public static SpellListDefinition PactTouchedSpellList = CreateAndAddToDB(PactTouchedSpellListName, PactTouchedSpellListGuid);
    }
}
