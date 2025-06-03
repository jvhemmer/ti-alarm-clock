using HarmonyLib;
using Newtonsoft.Json;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlarmClock
{
    [Serializable]
    public class Alarm
    {
        // Internal properties
        public DateTime Time;
        public string Note = string.Empty;

        public string EditedTime = string.Empty;
        public string EditedNote = string.Empty;

        // Boolean flags
        public bool IsEditing = false;
        public bool HasReminded = false;
        public bool ShowOptions = false;

        [JsonIgnore]
        public bool IsDue => Time <= TITimeState.Now().ExportTime();

        [JsonIgnore]
        public bool IsReminderDue => (Time - TITimeState.Now().ExportTime()).TotalDays <= ReminderDaysBefore;

        public bool RemoveOnTrigger = true;
        public bool Triggered = false;

        // Alarm settings
        public int ReminderDaysBefore = 7; // Default: remind user about alarm 7 days before it triggers
        public bool PauseOnAlarm = true;
        public bool ShowReminder = true;
        public bool ShowNotification = true;

        public bool SaveTime(string date)
        {
            try
            {
                Time = DateTime.ParseExact(date, "dd-MM-yyyy HH-mm-ss", null);
            }
            catch (FormatException ex)
            {
                Main.Entry?.Logger.Error($"Failed to parse date: {date}. Exception: {ex.Message}");
                return false;
            }
            return true;
        }

        public bool SaveNote(string note)
        {
            Note = note;
            return true;
        }

        public void Trigger()
        {
            if (!Triggered) Triggered = true;
            if (ShowNotification) LogAlarmTriggered();
            if (PauseOnAlarm) GameTimeManager.Singleton.Pause();
        }

        public void Remind()
        {
            if (ShowReminder && !HasReminded)
            {
                ShowCouncilorReminder();
            }
            else if (!ShowReminder)
            {
                Main.Entry?.Logger.Log($"Reminder for alarm {Time:dd-MM-yyyy HH-mm-ss} with note '{Note}' is due, but reminders are disabled for this alarm.");
            }
            HasReminded = true;
        }

        public Alarm Duplicate()
        {
            return new Alarm
            {
                Time = this.Time,
                Note = this.Note,
                ReminderDaysBefore = this.ReminderDaysBefore
            };
        }

        public void LogAlarmTriggered()
        {
            var notification = new NotificationQueueItem
            {
                templateName = "LogAlarmTriggered",
                relevantFactions = new List<TIFactionState> { GameControl.control.activePlayer },
                primaryFactions = new List<TIFactionState> { GameControl.control.activePlayer },
                icon = "icons_2d/ICO_clock",
                itemHeadline = "Alarm Triggered",
            };

            if (Note.Length > 0)
            {
                notification.itemDetail = $"The alarm titled {Note} set for {Time:dd-MM-yyyy HH-mm-ss} has triggered.";
                notification.itemSummary = $"Alarm triggered: {Note}";
            }
            else
            {
                notification.itemDetail = $"An untitled alarm set for {Time:dd-MM-yyyy HH-mm-ss} has triggered.";
                notification.itemSummary = $"Alarm triggered.";
            }

            var addItem = AccessTools.Method(typeof(TINotificationQueueState), "AddItem");
            addItem.Invoke(GameStateManager.NotificationQueue(), new object[] { notification, false });
        }

        public void ShowCouncilorReminder()
        {
            var councilor = GameControl.control.activePlayer.activeCouncilors.FirstOrDefault();

            var queue = GameStateManager.NotificationQueue();

            string message;

            if (Note.Length > 0)
            {
                message = $"Your alarm titled {Note} will trigger soon.";
            }
            else
            {
                message = $"An untitled alarm will trigger soon.";
            }

            queue.councilorMessages.Enqueue(
                new CouncilorMessage(councilor, message)
                );

            HasReminded = true;
        }
    }
}
