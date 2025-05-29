using UnityModManagerNet;

namespace AlarmClock
{
    // ReSharper disable InconsistentNaming
    public class Settings : UnityModManager.ModSettings
    {
        public bool Enabled = true;
        public bool PauseOnAlarm = true;
        public bool ShowReminder = true;
        public bool ShowNotification = true;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
}
