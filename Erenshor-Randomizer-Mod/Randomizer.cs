using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Erenshor_Randomizer_Mod.Core;
using MelonLoader;
using UnityEngine;
using Erenshor_Randomizer_Mod.Core.Classes;

namespace Erenshor_Randomizer_Mod
{
    public class Randomizer : MelonMod
    {
        // Load Config
        public static Config config;

        private bool npcSized = false;

        // Start Shot for the Randomizer
        private RandomizerCore rC;

        public override void OnEarlyInitializeMelon()
        {
            // Path to the .cfg file
            string configFilePath = "RandomConfig.cfg";

            // Check if the config file exists
            if (ConfigFileExists(configFilePath))
            {
                config = new Config(configFilePath);
                LoggerInstance.Msg($"Config file exists at: {configFilePath}");
            }
            else
            {
                LoggerInstance.Msg("Config file does not exist. Creating config file...");
                // Create and write the config file
                WriteConfigFile(configFilePath);
                LoggerInstance.Msg($"Config file created at: {configFilePath}");
                config = new Config(configFilePath);
            }
        }

        public override void OnLateInitializeMelon()
        {
            showConfigSettings();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (sceneName != "Stowaway")
                return;

            // Start Shot for the Randomizer
            rC = new RandomizerCore();

            // Start Randomizer
            rC.start(GameObject.Find("Player"));
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            if (sceneName != "Stowaway")
                return;

            rC = null;
        }

        public static void WriteConfigFile(string filePath)
        {
            // The content of the .cfg file
            string[] lines = {
            "# Player Randomizes",
            "RandomizeStats=true",
            "RandomizeRunSpeed=true",
            "RandomizeItems=true",
            "OnlyForMyClass=true # You will only receive random items for your class",
            "RandomizeGold=true",
            "RandomizeScale=true",
            "RandomizeAuras=true",
            "RandomizeSkills=true",
            "RandomizePlayerSpells=true",
            "RandomizePlayerGearOnLevelUp=true",
            "",
            "# NPC Randomizes",
            "RandomizeNPCLevels=true",
            "RandomizeNPCSizes=true",
            "RandomizeNPCLootWindows=true"
        };

            // Write the lines to the file
            File.WriteAllLines(filePath, lines);
        }

        public static bool ConfigFileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        private void showConfigSettings()
        {
            // Show all keys
            LoggerInstance.Msg("RANDOMIZER CONFIG LOADED!");
            foreach (var item in config._settings)
            {
                LoggerInstance.Msg($"{item.Key}={item.Value}");
            }
        }
    }
}
