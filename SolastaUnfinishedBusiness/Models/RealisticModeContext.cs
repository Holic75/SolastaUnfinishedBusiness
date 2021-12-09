using System.Collections.Generic;
using System.IO;
using I2.Loc;

namespace SolastaUnfinishedBusiness.Models
{
    internal static class RealisticModeContext
    {
        internal static void Load()
        {
            if (Main.Settings.RealisticMode)
            {
                var tinkererPath = $"{Main.MOD_FOLDER}/../CJDTinkerer";

                if (Directory.Exists(tinkererPath))
                {
                    ReloadTranslations(tinkererPath, tinkererReplaces);
                }

                Models.SharedSpellsContext.CasterTypeNames[3] = "Artificer";
            }
        }

        internal static void ReloadTranslations(string fromFolder, Dictionary<string, string> searchReplace)
        {
            var path = Path.Combine(fromFolder, "Translations-en.txt");
            var languageSourceData = LocalizationManager.Sources[0];
            var languageIndex = languageSourceData.GetLanguageIndexFromCode("en");

            foreach (var line in File.ReadLines(path))
            {
                try
                {
                    var splitted = line.Split(new[] { '\t', ' ' }, 2);
                    var term = splitted[0];
                    var text = splitted[1];

                    foreach (var k in searchReplace.Keys)
                    {
                        text = text.Replace(k, searchReplace[k]);
                    }

                    languageSourceData.RemoveTerm(term);
                    languageSourceData.AddTerm(term).Languages[languageIndex] = text;
                }
                catch
                {
                    Main.Error($"invalid translation line \"{line}\".");
                }
            }
        }

        internal static readonly Dictionary<string, string> tinkererReplaces = new Dictionary<string, string>()
        {
            { "Tinkerer", "Artificer" },
            { "tinkerer", "artificer" }
        };
    }
}