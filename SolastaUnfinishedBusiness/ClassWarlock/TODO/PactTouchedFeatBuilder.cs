using System;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaUnfinishedBusiness.ClassWarlock
{
    internal class PactTouchedFeatBuilder : BaseDefinitionBuilder<FeatDefinition>
    {
        public static readonly Guid PactTouchedMainGuid = new Guid("05ff22e3-1709-4081-9147-1df506b0507e");
        private const string PactTouchedFeatName = "PactTouchedFeat";
        private static readonly string PactTouchedFeatNameGuid = GuidHelper.Create(PactTouchedMainGuid, PactTouchedFeatName).ToString();

        protected PactTouchedFeatBuilder(string name, string guid) : base(DatabaseHelper.FeatDefinitions.FollowUpStrike, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&PactTouchedFeatTitle";
            Definition.GuiPresentation.Description = "Feat/&PactTouchedFeatDescription";

            Definition.Features.Clear();
            Definition.Features.Add(EldritchBlastPowerBuilder.PactEldricthBlastPower);
            Definition.Features.Add(PactMarkFeatPowerBuilder.PactMarkedPower);
            Definition.Features.Add(PactTouchedShatterFeatPowerBuilder.PactShatter);
            foreach (var characterClass in DatabaseRepository.GetDatabase<CharacterClassDefinition>().GetAllElements())
            {
                Definition.Features.Add(PactTouchedBuildAutoPreparedSpellsBuilder.GetOrAdd(characterClass));
            }
            //Try as I might, these do not actually work :(
            //Definition.Features.Add(PactTouchedBonusCantripsBuilder.PactTouchCantrips); 
            //Definition.Features.Add(PactTouchedMagicAffinityBuilder.PactTouchedSpellList);

            Definition.SetMinimalAbilityScorePrerequisite(false);
        }

        public static FeatDefinition CreateAndAddToDB(string name, string guid)
        {
            return new PactTouchedFeatBuilder(name, guid).AddToDB();
        }

        public static FeatDefinition PactTouchedFeat = CreateAndAddToDB(PactTouchedFeatName, PactTouchedFeatNameGuid);
    }
}
