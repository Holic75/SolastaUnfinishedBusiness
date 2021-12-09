using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using ModKit;
using UnityModManagerNet;

namespace SolastaUnfinishedBusiness
{
    internal static class Main
    {
        internal static bool Enabled { get; set; }

        internal static readonly string MOD_FOLDER = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        [Conditional("DEBUG")]
        internal static void Log(string msg)
        {
            Logger.Log(msg);
        }

        internal static void Error(Exception ex)
        {
            Logger?.Error(ex.ToString());
        }

        internal static void Error(string msg)
        {
            Logger?.Error(msg);
        }

        internal static void Warning(string msg)
        {
            Logger?.Warning(msg);
        }

        internal static UnityModManager.ModEntry.ModLogger Logger { get; private set; }

        internal static Settings Settings { get; private set; }

        internal static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var menu = new MenuManager();
                var mod = new ModManager<Core, Settings>();

                mod.Enable(modEntry, assembly);
                menu.Enable(modEntry, assembly);
                Logger = modEntry.Logger;
                Settings = mod.Settings;
                Translations.Load(MOD_FOLDER);
            }
            catch (Exception ex)
            {
                Error(ex);
                throw;
            }

            return true;
        }
    }
}
