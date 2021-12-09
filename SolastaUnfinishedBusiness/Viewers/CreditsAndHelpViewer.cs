using System.Collections.Generic;
using ModKit;
using UnityModManagerNet;

namespace SolastaUnfinishedBusiness.Viewers
{
    public class CreditsAndHelpViewer : IMenuSelectablePage
    {
        public string Name => "Help & Credits";

        public int Priority => 1;

        private static void DisplayMulticlassHelp()
        {
            UI.Label("");
            UI.Label("Instructions [Character Inspection Screen]:".yellow());
            UI.Label(". press the " + "UP".yellow().bold() + " arrow to toggle the character panel selector [useful if more than 2 spell selection tabs]");
            UI.Label(". press the " + "DOWN".yellow().bold() + " arrow to toggle between different class or subclass label styles");
            UI.Label(". press the " + "LEFT".yellow().bold() + " and " + "RIGHT".yellow().bold() + " arrows to present other hero classes details");

            UI.Label("");
            UI.Label("Instructions [Casting]:".yellow());
            UI.Label(". " + "default".italic() + " - consumes a short rest slot whenever one is available");
            UI.Label(". " + "intermediate".italic() + " - consumes a long rest slot whenever the Warlock spell level is greater than the shared spell level [can be enabled in the settings panel]");
            UI.Label(". " + "advanced".italic() + " - " + "SHIFT-click".yellow().bold() + " a spell to consume a long rest slot whenever one is available");

            UI.Label("");
            UI.Label("Features:".yellow());
            UI.Label(". combines up to 4 different classes");
            UI.Label(". enforces ability scores minimum in & out pre-requisites");
            UI.Label(". enforces the correct subset of starting proficiencies");
            UI.Label(". implements the shared slots system combining all casters with an option to also combine Warlock pact magic");
            UI.Label(". " + "extra attacks".cyan() + " / " + "unarmored defenses".cyan() + " won't stack whenever combining Barbarian, Fighter, Paladin or Ranger");
            UI.Label(". " + "channel divinity".cyan() + " won't stack whenever combining Cleric and Paladin");
            UI.Label(". supports official game classes, Tinkerer and all subclass in Community Expansion mod");
            UI.Label(". Warlock class in development");
        }

        private static readonly Dictionary<string, string> creditsTable = new Dictionary<string, string>
        {
            { "Zappastuff".bold(), "head developer, UI reverse-engineer, pact magic integration" },
            { "ImpPhil", "code support" },
            { "AceHigh", "shared slot system, pact magic, code support" },
            { "Esker", "ruleset support, quality assurance, tests" },
            { "Lyraele", "ruleset support, quality assurance, tests" },
            { "PraiseThyJeebus", "slot colors idea, quality assurance, tests" },
            { "[MAD] SirMadnessTv", "traduction française" },
            { "Narria", "modKit creator, developer" }
        };

        private static void DisplayCredits()
        {
            UI.Label("");
            UI.Label("Credits".yellow().bold());
            UI.Div();

            UI.Label("");

            foreach (var kvp in creditsTable)
            {
                using (UI.HorizontalScope())
                {
                    UI.Label(kvp.Key.orange(), UI.Width(120));
                    UI.Label(kvp.Value, UI.Width(400));
                }
            }

            UI.Label("");
        }

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Unfinished Business".yellow().bold());
            UI.Div();

            DisplayMulticlassHelp();
            DisplayCredits();
        }
    }
}
