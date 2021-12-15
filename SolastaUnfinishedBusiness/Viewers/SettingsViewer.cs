using ModKit;
using UnityModManagerNet;

namespace SolastaUnfinishedBusiness.Viewers
{
    public class SettingsViewer : IMenuSelectablePage
    {
        public string Name => "Settings";

        public int Priority => 0;

        private static void DisplayMainSettings()
        {
            var game = ServiceRepository.GetService<IGameService>()?.Game;
            bool toggle;
            int value;

            UI.Label("");

            value = Main.Settings.MaxAllowedClasses;
            if (UI.Slider("Max allowed classes".white(), ref value, 0, 4, 2, "", UI.Width(50)))
            {
                Main.Settings.MaxAllowedClasses = value;
            }

            UI.Label("");

            toggle = Main.Settings.EnableClassWarlock;
            if (UI.Toggle("Enable the alpha " + "Warlock".orange() + " class " + "[requires restart]".bold().italic().red(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableClassWarlock = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.EnableMinInOutAttributes;
            if (UI.Toggle("Enforce ability scores minimum in & out pre-requisites", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableMinInOutAttributes = toggle;
            }

            toggle = Main.Settings.EnableNonStackingExtraAttacks;
            if (UI.Toggle("Extra attacks won't stack if granted from different classes", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableNonStackingExtraAttacks = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.EnableGrantHolySymbol;
            if (UI.Toggle("Grant holy symbol to divine casters", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableGrantHolySymbol = toggle;
            }

            toggle = Main.Settings.EnableGrantComponentPouch;
            if (UI.Toggle("Grant component pouch to arcane casters", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableGrantComponentPouch = toggle;
            }

            toggle = Main.Settings.EnableGrantDruidicFocus;
            if (UI.Toggle("Grant druidic focus to " + "Druid".orange(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableGrantDruidicFocus = toggle;
            }

            toggle = Main.Settings.EnableGrantCLothesWizard;
            if (UI.Toggle("Grant clothes to " + "Wizard".orange(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableGrantCLothesWizard = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.EnableRelearnCantrips;
            if (UI.Toggle("Same cantrip can be learned by different classes", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableRelearnCantrips = toggle;
            }

            toggle = Main.Settings.EnableRelearnSpells;
            if (UI.Toggle("Same spell can be learned or scribed by different classes", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableRelearnSpells = toggle;
            }

            toggle = Main.Settings.EnableRelearnScribedSpells;
            if (UI.Toggle("Scribed spells can be learned by different classes", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableRelearnScribedSpells = toggle;
            }

            toggle = Main.Settings.EnableDisplayAllKnownSpellsOnLevelUp;
            if (UI.Toggle("Display known spells from other classes during level up", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableDisplayAllKnownSpellsOnLevelUp = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.EnableCantripsAtCharacterLevel;
            if (UI.Toggle("Cantrips are cast at character level", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableCantripsAtCharacterLevel = toggle;
            }

            toggle = Main.Settings.EnableFixTwinnedLogic;
            if (UI.Toggle("Fix " + "Sorcerer".orange() + " twinned metamagic " + "[a spell must be incapable of targeting more than one creature at the spell’s current level]".italic().yellow(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableFixTwinnedLogic = toggle;
            }

            if (game == null)
            {
                UI.Label("");

                toggle = Main.Settings.EnableSharedSpellCasting;
                if (UI.Toggle("Enable the shared slots system", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableSharedSpellCasting = toggle;
                    if (!toggle)
                    {
                        Main.Settings.EnableSharedSpellCasting = toggle;
                    }
                }

                if (Main.Settings.EnableSharedSpellCasting && Main.Settings.EnableClassWarlock)
                {
                    toggle = Main.Settings.EnableCombinedSpellCasting;
                    if (UI.Toggle("Combine " + "Warlock".orange() + " pact magic and the shared slots system", ref toggle, UI.AutoWidth()))
                    {
                        Main.Settings.EnableCombinedSpellCasting = toggle;
                    }

                    toggle = Main.Settings.EnableConsumeLongRestSlotFirst;
                    if (UI.Toggle("Use long rest slots whenever the " + "Warlock".orange() + " spell level is greater than the shared spell level", ref toggle, UI.AutoWidth()))
                    {
                        Main.Settings.EnableConsumeLongRestSlotFirst = toggle;
                    }
                }
                else
                {
                    Main.Settings.EnableCombinedSpellCasting = false;
                    Main.Settings.EnableConsumeLongRestSlotFirst = false;
                }
            }
            else
            {
                UI.Label("");
                UI.Label("you cannot change the slot system behavior while in game...".bold().italic().yellow());
                UI.Label("");
            }
        }

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Unfinished Business".yellow().bold());
            UI.Div();

            if (Main.Enabled)
            {
                DisplayMainSettings();
            }
        }
    }
}
