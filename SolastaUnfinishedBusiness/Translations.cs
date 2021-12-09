﻿using System.IO;
using I2.Loc;

namespace SolastaUnfinishedBusiness
{
    internal static class Translations
    {
        internal static void Load(string fromFolder)
        {
            var languageSourceData = LocalizationManager.Sources[0];

            foreach (var path in Directory.EnumerateFiles(fromFolder, $"Translations-??.txt"))
            {
                var filename = Path.GetFileName(path);
                var code = filename.Substring(13, 2);
                var languageIndex = languageSourceData.GetLanguageIndexFromCode(code);

                if (languageIndex < 0)
                {
                    Main.Error($"language {code} not currently loaded.");
                    continue;
                }

                foreach (var line in File.ReadLines(path))
                {
                    try
                    {
                        var splitted = line.Split(new[] { '\t', ' ' }, 2);
                        var term = splitted[0];
                        var text = splitted[1];

                        languageSourceData.AddTerm(term).Languages[languageIndex] = text;
                    }
                    catch
                    {
                        Main.Error($"invalid translation line \"{line}\".");
                    }
                }
            }
        }
    }
}