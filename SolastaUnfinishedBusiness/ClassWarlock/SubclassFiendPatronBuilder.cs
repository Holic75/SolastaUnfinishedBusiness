using SolastaModApi;
using UnityEngine;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionMagicAffinitys;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    public static class SubclassFiendPatronBuilder
    {
        public static CharacterSubclassDefinition ClassWarlockSubclassFiendPatron { get; private set; }

        public const string ClassWarlockSubclassFiendPatronName = "ClassWarlockSubclassFiendPatron";

        public static readonly string ClassWarlockSubclassFiendPatronGuid = GuidHelper.Create(new System.Guid(Settings.GUID), ClassWarlockSubclassFiendPatronName).ToString();

        internal static void Build()
        {
            var featureDefinitionMagicAffinity = Object.Instantiate(MagicAffinityGreenmageGreenMagicList);

            featureDefinitionMagicAffinity.name = "ClassWarlockSubclassFiendPatronMagicAffinity";
            featureDefinitionMagicAffinity.guid = GuidHelper.Create(new System.Guid(Settings.GUID), "ClassWarlockSubclassFiendPatronMagicAffinity").ToString();
            featureDefinitionMagicAffinity.guiPresentation = new GuiPresentationBuilder("Feature/&ClassWarlockSubclassFiendPatronMagicAffinityDescription", "Feature/&ClassWarlockSubclassFiendPatronMagicAffinityTitle").Build();
            featureDefinitionMagicAffinity.extendedSpellList = SubclassFiendPatronSpellListBuilder.Build();

            DatabaseRepository.GetDatabase<FeatureDefinition>().Add(featureDefinitionMagicAffinity);
            // DatabaseRepository.GetDatabase<FeatureDefinitionMagicAffinity>().Add(featureDefinitionMagicAffinity);

            var classWarlockSubclassFiendPatronPresentationBuilder = new GuiPresentationBuilder("Subclass/&ClassWarlockSubclassFiendPatronDescription", "Subclass/&ClassWarlockSubclassFiendPatronTitle")
                .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.DomainSun.GuiPresentation.SpriteReference);

            ClassWarlockSubclassFiendPatron = new CharacterSubclassDefinitionBuilder(ClassWarlockSubclassFiendPatronName, ClassWarlockSubclassFiendPatronGuid)
                .SetGuiPresentation(classWarlockSubclassFiendPatronPresentationBuilder.Build())
                .AddFeatureAtLevel(featureDefinitionMagicAffinity, 1)
                .AddToDB();
        }
    }
}
