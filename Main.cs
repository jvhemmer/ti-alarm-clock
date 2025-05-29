using HarmonyLib;

using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Systems.GameTime;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

using UI = ModKit.UI;

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

            // Check for alarms and trigger them
            foreach (var alarm in Alarms.ToList())
            {
                if (!alarm.HasReminded)
                {
                    if (alarm.IsReminderDue && ModSettings.ShowReminder)
                    {

                        alarm.Remind();
                    }
                    else
                    {
                        Entry?.Logger.Log($"Reminder for alarm {alarm.Time:dd-MM-yyyy HH-mm-ss} with note '{alarm.Note}' is due, but reminders are disabled.");

                    }
                }
                if (alarm.IsDue && !alarm.Triggered)
                {
                    Entry?.Logger.Log($"Alarm {alarm.Time:dd-MM-yyyy HH-mm-ss} is now due.");

                    alarm.Trigger();

                    if (ModSettings.PauseOnAlarm)
                    {
                        GameTimeManager.Singleton.Pause(); // Pause the game
                    }
                    else
                    {
                        Entry?.Logger.Log($"Alarm {alarm.Time:dd-MM-yyyy HH-mm-ss} with note '{alarm.Note}' has triggered, but game pause is disabled.");
                    }
                    
                    if (alarm.RemoveOnTrigger) Alarms.Remove(alarm);
                }
            }
        }

        public static bool OnToggle(ModEntry entry, bool value)
        {
            ModSettings.Enabled = value;
            return true;
        }

        public static void OnGUI(ModEntry entry)
        {
            if (!ModSettings.Enabled || !IsInGame) return;

            if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))
            {
                Event.current.Use(); // Consume the event so Unity doesn't propagate it further
                TryAddAlarm();
            }

            UIElements.Label("These options are overwritten by the individual alarm configuration.", 750);
            UI.Toggle("Pause game when alarm triggers", ref ModSettings.PauseOnAlarm);
            UI.Toggle("Show a notification when alarm triggers", ref ModSettings.ShowNotification);
            UI.Toggle("Councilor reminder before alarm triggers", ref ModSettings.ShowReminder);

            UI.Space(10);
            UI.Div();
            UI.Space(5);


            if (Alarms.Count == 0)
            {
                UIElements.Label("No alarms set. Use the input field below to add a new alarm using the format dd-MM-yyyy HH-mm-ss (time is optional).", 750);
            }
            else
            {
                UIElements.Label($"{Alarms.Count} alarm(s) set.", 750);
            }

            foreach (var alarm in Alarms.ToList())
            {
                using (UI.HorizontalScope())
                {

                    if (alarm.IsEditing)
                    {
                        // Editable fields
                        UIElements.TextField(ref alarm.EditedTime, 150);
                        UIElements.LabelRight("Note:", 50);
                        UIElements.TextField(ref alarm.EditedNote, 550);

                        // Create save button
                        if (GUILayout.Button("Save")) TrySaveAlarm(alarm);
                    }
                    else
                    { // Display fields (not editing)
                        UIElements.Label(alarm.Time.ToString("dd-MM-yyyy HH-mm-ss"), 150);
                        UIElements.LabelRight("Note:", 50);
                        UIElements.Label(" " + alarm.Note, 550);

                        // Create edit button
                        if (GUILayout.Button("Edit")) TryEditAlarm(alarm);
                    }

                    // Create delete button
                    if (GUILayout.Button("Delete")) Alarms.Remove(alarm);

                    // Create options button
                    if (GUILayout.Button("*", GUILayout.MaxWidth(25)))
                    {
                        // Collapse all others
                        foreach (var other in Main.Alarms)
                            if (other != alarm) other.ShowOptions = false;

                        // Toggle this one
                        alarm.ShowOptions = !alarm.ShowOptions;
                    }
                }
                if (alarm.ShowOptions)
                {
                    GUIStyle numberBoxStyle = new GUIStyle(GUI.skin.textField)
                    {
                        fixedHeight = 16, // You can go as low as 16 depending on UI scaling
                        padding = new RectOffset(2, 2, 2, 2),
                        margin = new RectOffset(0, 0, 0, 0),
                        alignment = TextAnchor.MiddleCenter,
                    };

                    GUILayout.BeginVertical("box");

                    UI.Toggle("Delete alarm on trigger", ref alarm.RemoveOnTrigger);

                    UI.Toggle("Pause game on trigger", ref alarm.PauseOnAlarm);

                    UI.Toggle("Show notification (will pause)", ref alarm.ShowNotification);

                    UI.Toggle("Show reminder", ref alarm.ShowReminder);
                    GUILayout.BeginHorizontal();

                    UIElements.Label("        -> Remind ", 100);

                    string reminderStr = alarm.ReminderDaysBefore.ToString();
                    UIElements.TextField(ref reminderStr, 50);
                    if (int.TryParse(reminderStr, out int days))
                    {
                        alarm.ReminderDaysBefore = Mathf.Clamp(days, 0, 999);
                    }

                    UIElements.Label(" days prior to alarm.", 125);

                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                }
            }

            // Spacer
            UI.Space(10);
            UI.Div();
            UI.Space(5);

            // New alarm input
            using (UI.HorizontalScope())
            {
                UIElements.TextField(ref NewDateInput, 150);

                UIElements.LabelRight("Note:", 50);

                UIElements.TextField(ref NewNoteInput, 200);
            }

            if (GUILayout.Button("Add")) TryAddAlarm();
        }

        public static void TryAddAlarm()
        {
            string fullDateStr = NewDateInput.Trim();

            if (!fullDateStr.Contains(" ")) fullDateStr += " 00-00-00";

            if (DateTime.TryParseExact(fullDateStr, "dd-MM-yyyy HH-mm-ss", null, System.Globalization.DateTimeStyles.None, out var parsed))
            {
                Alarms.Add(new Alarm { Time = parsed, Note = NewNoteInput });
                NewDateInput = string.Empty;
                NewNoteInput = string.Empty;

                Entry?.Logger.Log($"Alarm added. Total alarms: {Alarms.Count}");
            }
            else
            {
                Entry?.Logger.Error($"Invalid date format: {fullDateStr}. Expected format: dd-MM-yyyy HH-mm-ss");
            }
        }

        public static void TryEditAlarm(Alarm alarm)
        {
            // Discard edits on other alarms to prevent multiple edits at once
            foreach (var other in Alarms)
            {
                other.IsEditing = false;
            }

            alarm.IsEditing = true;
            alarm.ShowOptions = false;

            alarm.EditedTime = alarm.Time.ToString("dd-MM-yyyy HH-mm-ss");
            alarm.EditedNote = alarm.Note;
        }

        public static void TrySaveAlarm(Alarm alarm)
        {
            alarm.IsEditing = true;

            alarm.SaveTime(alarm.EditedTime);
            alarm.SaveNote(alarm.EditedNote);
            alarm.IsEditing = false;
        }

        public static string TryAutoFormatDateInput(string input)
        {
            var digits = new string(input.Where(char.IsDigit).ToArray());
            if (digits.Length == 0) return string.Empty;

            List<char> result = new();
            for (int i = 0; i < digits.Length && i < 14; i++)
            {
                if (i == 2 || i == 4) result.Add('-');       // after day and month
                if (i == 8) result.Add(' ');                 // after year
                if (i == 10 || i == 12) result.Add('-');     // after hour and minute
                result.Add(digits[i]);
            }

            return new string(result.ToArray());
        }

        static bool Unload(ModEntry entry)
        {
            ModSettings.Save(entry);
            new Harmony(entry.Info.Id).UnpatchAll();

            return true;
        }

        private static bool IsInGame => GameStateManager.IsValid();

        public static List<Alarm> Alarms = new();
        public static string NewDateInput = string.Empty;
        public static string NewNoteInput = string.Empty;
    }
}
