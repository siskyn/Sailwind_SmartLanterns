using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using HarmonyLib.Tools;
using System.Reflection;

namespace Smart_Lanterns
{
    [BepInPlugin(PLUGIN_ID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin

    {
        public const string PLUGIN_ID = "com.couladin.SmartLanterns";
        public const string PLUGIN_NAME = "SmartLanterns";
        public const string PLUGIN_VERSION = "0.0.1";


        internal static ManualLogSource logSource;

        //- settings -
        //internal static ConfigEntry<bool> storage;
        public static ConfigEntry<float> lanternStartTime;
        public static ConfigEntry<float> lanternStopTime;

        private void Awake()
        {
            HarmonyFileLog.Enabled = true;

            InitializeLaternSchedule();


            // Plugin startup logic
            logSource = Logger;
            Logger.LogInfo($"Plugin {PLUGIN_NAME} is loaded!");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_ID);          
        }

        private void InitializeLaternSchedule()
        {
            lanternStartTime = Config.Bind("LanternSettings",
                                           "LanternStartTime",
                                           17.0f,
                                           "The time when lanterns should turn on (24-hour decimal format, e.g. 13.5 is 1:30 PM, and 16.25 is 4:15 PM)");

            lanternStopTime = Config.Bind("LanternSettings",
                                          "LanternStopTime",
                                          7.0f,
                                          "The time when lanterns should turn off (24-hour decimal format, e.g. 13.5 is 1:30 PM, and 16.25 is 4:15 PM)");

            Logger.LogInfo($"Lantern Start Time: {lanternStartTime.Value}");
            Logger.LogInfo($"Lantern Stop Time: {lanternStopTime.Value}");
        }
    }
}
