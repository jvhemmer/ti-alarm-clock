using HarmonyLib;

using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Systems.GameTime;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

using ModKit;

namespace AlarmClock
{
    [EnableReloading]
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
            AlarmManager.Load(ModSettings.SavedAlarms);

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
