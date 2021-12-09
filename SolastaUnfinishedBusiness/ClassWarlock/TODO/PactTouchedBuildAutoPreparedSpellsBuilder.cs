using System.Collections.Generic;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class PactTouchedBuildAutoPreparedSpellsBuilder : BaseDefinitionBuilder<FeatureDefinitionAutoPreparedSpells>
    {
        private const string PactTouchedBuildAutoPreparedSpellsName = "ZSPactTouchedBuildAutoPreparedSpells";

        protected PactTouchedBuildAutoPreparedSpellsBuilder(CharacterClassDefinition characterClass, string name, string guid) : base(DatabaseHelper.FeatureDefinitionAutoPreparedSpellss.AutoPreparedSpellsDomainBattle, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&ZSPactTouchedBuildAutoPreparedSpellsTitle";
            Definition.GuiPresentation.Description = "Feat/&ZSPactTouchedBuildAutoPreparedSpellsDescription";
            Definition.SetSpellcastingClass(characterClass);
            Definition.AutoPreparedSpellsGroups.Clear();
            Definition.AutoPreparedSpellsGroups.Add(new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup() { SpellsList = new List<SpellDefinition> { PactMarkSpellBuilder.PactMarkSpell, EldritchBlastSpellBuilder.EldritchBlastSpell, DatabaseHelper.SpellDefinitions.Shatter }, ClassLevel = 0 });
        }

        public static FeatureDefinitionAutoPreparedSpells CreateAndAddToDB(CharacterClassDefinition characterClass)
        {
            return new PactTouchedBuildAutoPreparedSpellsBuilder(characterClass, PactTouchedBuildAutoPreparedSpellsName + characterClass.Name, GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, PactTouchedBuildAutoPreparedSpellsName + characterClass.Name).ToString()).AddToDB();
        }

        public static FeatureDefinitionAutoPreparedSpells GetOrAdd(CharacterClassDefinition characterClass)
        {
            var db = DatabaseRepository.GetDatabase<FeatureDefinitionAutoPreparedSpells>();
            return db.TryGetElement(PactTouchedBuildAutoPreparedSpellsName + characterClass.Name, GuidHelper.Create(PactTouchedFeatBuilder.PactTouchedMainGuid, PactTouchedBuildAutoPreparedSpellsName + characterClass.Name).ToString()) ?? CreateAndAddToDB(characterClass);
        }
    }
}
