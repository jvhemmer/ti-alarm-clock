using HarmonyLib;
using ModKit;
using Newtonsoft.Json;
using PavonisInteractive.TerraInvicta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace AlarmClock
{
    //[EnableReloading]
    public class Main
    {
        public static Settings ModSettings = new Settings();
        public static ModEntry? Entry;

        public static bool Load(ModEntry entry)
        {
            Entry = entry;
            var harmony = new Harmony(entry.Info.Id);
            harmony.PatchAll();

            ModSettings = UnityModManager.ModSettings.Load<Settings>(entry);

            // Migration: Only if global alarms exist and migration hasn't happened yet
            if (ModSettings.SavedAlarms != null && ModSettings.SavedAlarms.Count > 0)
            {
                AlarmManager.Alarms = new List<Alarm>(ModSettings.SavedAlarms);

                // Export global alarms to a file just in case this process fails
                var globalAlarmsFile = Path.Combine(UnityModManagerNet.UnityModManager.modsPath, "Alarm Clock", "GlobalAlarmsBackup.json");
                var backupJson = JsonConvert.SerializeObject(AlarmManager.Alarms, Formatting.Indented);
                File.WriteAllText(globalAlarmsFile, backupJson);
                Entry?.Logger.Log($"Exported global alarms to {globalAlarmsFile}.");

                // Clear global alarms so this only happens once
                ModSettings.SavedAlarms.Clear();
                ModSettings.Save(entry);
                Entry?.Logger.Log("Migrated global alarms to per-savefile system.");
            }

            //entry.OnSaveGUI = OnSaveGUI;
            entry.OnGUI = OnGUI;
            entry.OnToggle = OnToggle;
            entry.OnUnload = Unload;
            entry.OnUpdate = OnUpdate;

            entry.Hotkey = new KeyBinding();
            entry.Hotkey.Change(KeyCode.C, true, false, false);
            return true;
        }

        public static void OnUpdate(ModEntry entry, float dt)
        {
            if (!ModSettings.Enabled || !IsInGame) return;

            AlarmManager.CheckAlarms(AlarmManager.Alarms);
        }

        public static bool OnToggle(ModEntry entry, bool value)
        {
            ModSettings.Enabled = value;
            return true;
        }

        public static void OnGUI(ModEntry entry)
        {
            if (!ModSettings.Enabled || !IsInGame) return;

            //Entry?.Logger.Log("Running OnGUI.");

            UI.Draw(ModSettings);
        }

        static bool Unload(ModEntry entry)
        {
            ModSettings.Save(entry);
            new Harmony(entry.Info.Id).UnpatchAll();

            return true;
        }

        private static bool IsInGame => GameStateManager.IsValid();
    }
}
