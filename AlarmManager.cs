using Newtonsoft.Json;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
//using static UnityModManagerNet.UnityModManager;

namespace AlarmClock
{
    internal class AlarmManager
    {
        public static List<Alarm> Alarms = new();

        public static void CheckAlarms(List<Alarm> Alarms)
        {
            // Check for alarms and trigger them
            foreach (var alarm in Alarms.ToList())
            {
                if (!alarm.HasReminded)
                {
                    if (alarm.IsReminderDue)
                    {
                        if (Main.ModSettings.ShowReminder)
                        {
                            alarm.Remind();
                        }
                        else
                        {
                            Main.Entry?.Logger.Log($"Reminder for alarm {alarm.Time:dd-MM-yyyy HH-mm-ss} with note '{alarm.Note}' is due, but reminders are disabled.");
                        }
                    }
                    else
                    {
                        // Do nothing if reminder is not due
                    }
                }
                if (alarm.IsDue && !alarm.Triggered)
                {
                    Main.Entry?.Logger.Log($"Alarm {alarm.Time:dd-MM-yyyy HH-mm-ss} is now due.");

                    alarm.Trigger();

                    if (Main.ModSettings.PauseOnAlarm)
                    {
                        GameTimeManager.Singleton.Pause(); // Pause the game
                    }
                    else
                    {
                        Main.Entry?.Logger.Log($"Alarm {alarm.Time:dd-MM-yyyy HH-mm-ss} with note '{alarm.Note}' has triggered, but game pause is disabled.");
                    }

                    if (alarm.RemoveOnTrigger) Alarms.Remove(alarm);

                    Save();
                }
            }
        }

        public static void TryAddAlarm(string dateInput, string noteInput)
        {
            string fullDateStr = dateInput.Trim();

            if (!fullDateStr.Contains(" ")) fullDateStr += " 00-00-00";

            if (DateTime.TryParseExact(fullDateStr, "dd-MM-yyyy HH-mm-ss", null, System.Globalization.DateTimeStyles.None, out var parsed))
            {
                Alarms.Add(new Alarm { Time = parsed, Note = noteInput });
                dateInput = string.Empty;
                noteInput = string.Empty;

                Main.Entry?.Logger.Log($"Alarm added. Total alarms: {Alarms.Count}");

                UI.ResetInputs();

                Save();
            }
            else
            {
                Main.Entry?.Logger.Error($"Invalid date format: {fullDateStr}. Expected format: dd-MM-yyyy HH-mm-ss");
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

            Save();
        }

        public static void TrySaveAlarm(Alarm alarm)
        {
            alarm.IsEditing = true;

            alarm.SaveTime(alarm.EditedTime);
            alarm.SaveNote(alarm.EditedNote);
            alarm.IsEditing = false;

            Save();
        }

        public static void Load(List<Alarm> SavedAlarms)
        {
            //Alarms = SavedAlarms ?? new List<Alarm>();
        }

        public static void Save()
        {
            //Main.ModSettings.SavedAlarms = new List<Alarm>(Alarms);
            //Main.ModSettings.Save(Main.Entry);
        }

        public static void SaveAlarms(string filepath)
        {
            var alarmFile = GetAlarmFilePath(filepath);
            var json = JsonConvert.SerializeObject(Alarms, Formatting.Indented);
            File.WriteAllText(alarmFile, json);
        }

        public static void LoadAlarms(string filepath)
        {
            var alarmFile = GetAlarmFilePath(filepath);
            if (File.Exists(alarmFile))
            {
                var json = File.ReadAllText(alarmFile);
                Alarms = JsonConvert.DeserializeObject<List<Alarm>>(json) ?? new List<Alarm>();
            }
            else
            {
                Alarms = new List<Alarm>();
            }
        }

        private static string GetAlarmFilePath(string filepath)
        {
            var saveName = Path.GetFileNameWithoutExtension(filepath);
            var modDir = Path.Combine(UnityModManagerNet.UnityModManager.modsPath, "Alarm Clock");
            Directory.CreateDirectory(modDir);
            return Path.Combine(modDir, $"Alarms_{saveName}.json");
        }
    }
}
