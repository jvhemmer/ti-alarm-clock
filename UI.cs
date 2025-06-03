using ModKit;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

namespace AlarmClock
{
    public static class UI
    {
        public static string NewDateInput = string.Empty;
        public static string NewNoteInput = string.Empty;

        public static void Draw(Settings ModSettings)
        {
            if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))
            {
                Event.current.Use(); // Consume the event so Unity doesn't propagate it further
                AlarmManager.TryAddAlarm(NewDateInput, NewNoteInput);
            }

            UIElements.Label("These options are overwritten by the individual alarm configuration.", 750);
            ModKit.UI.Toggle("Pause game when alarm triggers", ref ModSettings.PauseOnAlarm);
            ModKit.UI.Toggle("Show a notification when alarm triggers", ref ModSettings.ShowNotification);
            ModKit.UI.Toggle("Councilor reminder before alarm triggers", ref ModSettings.ShowReminder);

            ModKit.UI.Space(10);
            ModKit.UI.Div();
            ModKit.UI.Space(5);


            if (AlarmManager.Alarms.Count == 0)
            {
                UIElements.Label("No alarms set. Use the input field below to add a new alarm using the format dd-MM-yyyy HH-mm-ss (time is optional).", 750);
            }
            else
            {
                UIElements.Label($"{AlarmManager.Alarms.Count} alarm(s) set.", 750);
            }

            foreach (var alarm in AlarmManager.Alarms.ToList())
            {
                using (ModKit.UI.HorizontalScope())
                {

                    if (alarm.IsEditing)
                    {
                        // Editable fields
                        UIElements.TextField(ref alarm.EditedTime, 150);
                        UIElements.LabelRight("Note:", 50);
                        UIElements.TextField(ref alarm.EditedNote, 550);

                        // Create save button
                        if (GUILayout.Button("Save")) AlarmManager.TrySaveAlarm(alarm);
                    }
                    else
                    { // Display fields (not editing)
                        UIElements.Label(alarm.Time.ToString("dd-MM-yyyy HH-mm-ss"), 150);
                        UIElements.LabelRight("Note:", 50);
                        UIElements.Label(" " + alarm.Note, 550);

                        // Create edit button
                        if (GUILayout.Button("Edit")) AlarmManager.TryEditAlarm(alarm);
                    }

                    // Create delete button
                    if (GUILayout.Button("Delete")) AlarmManager.Alarms.Remove(alarm);

                    // Create options button
                    if (GUILayout.Button("*", GUILayout.MaxWidth(25)))
                    {
                        // Collapse all others
                        foreach (var other in AlarmManager.Alarms)
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

                    ModKit.UI.Toggle("Delete alarm on trigger", ref alarm.RemoveOnTrigger);

                    ModKit.UI.Toggle("Pause game on trigger", ref alarm.PauseOnAlarm);

                    ModKit.UI.Toggle("Show notification (will pause)", ref alarm.ShowNotification);

                    ModKit.UI.Toggle("Show reminder", ref alarm.ShowReminder);
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
            ModKit.UI.Space(10);
            ModKit.UI.Div();
            ModKit.UI.Space(5);

            // New alarm input
            using (ModKit.UI.HorizontalScope())
            {
                UIElements.TextField(ref NewDateInput, 150);

                UIElements.LabelRight("Note:", 50);

                UIElements.TextField(ref NewNoteInput, 200);
            }

            if (GUILayout.Button("Add")) AlarmManager.TryAddAlarm(NewDateInput, NewNoteInput);
        }
    
        public static void ResetInputs()
        {
            NewDateInput = string.Empty;
            NewNoteInput = string.Empty;
        }
    }
}
