using HarmonyLib;
using System.IO;

namespace AlarmClock
{
    [HarmonyPatch(typeof(PavonisInteractive.TerraInvicta.GameStateManager))]
    public static class GameStateManagerPatch
    {
        [HarmonyPatch("SaveAllGameStates")]
        [HarmonyPostfix]
        public static void SaveAllGameStatesPatch(string filepath)
        {
            AlarmManager.SaveAlarms(filepath);
        }

        [HarmonyPatch("LoadAllGameStates")]
        [HarmonyPostfix]
        public static void LoadAllGameStatesPatch(string filepath)
        {
            AlarmManager.LoadAlarms(filepath);
        }
    }
}