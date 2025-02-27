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
        public const string PLUGIN_VERSION = "0.0.3";


        public static bool DEBUGMODE = false;

        internal static ManualLogSource logSource;

        //- settings -
        //internal static ConfigEntry<bool> storage;
        public static ConfigEntry<float> lanternStartTime;
        public static ConfigEntry<float> lanternStopTime;
        public static ConfigEntry<bool> controlCandleLanterns;
        public static ConfigEntry<bool> controlOilLanterns;
        public static ConfigEntry<bool> refillCandleLanterns;

        private void Awake()
        {

            if (DEBUGMODE) HarmonyFileLog.Enabled = true;
            else HarmonyFileLog.Enabled = false;


            LoadConfigValues();


            // Plugin startup logic
            logSource = Logger;
            Logger.LogInfo($"Plugin {PLUGIN_NAME} is loaded!");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_ID);          
        }

        private void LoadConfigValues()
        {
            lanternStartTime = Config.Bind("LanternSettings",
                                           "LanternStartTime",
                                           16.0f, // default value
                                           "The time when lanterns should turn on (24-hour decimal format, e.g. 13.5 is 1:30 PM, and 16.25 is 4:15 PM)");

            lanternStopTime = Config.Bind("LanternSettings",
                                          "LanternStopTime",
                                          8.0f, // default value
                                          "The time when lanterns should turn off (24-hour decimal format, e.g. 13.5 is 1:30 PM, and 16.25 is 4:15 PM)");

            controlCandleLanterns = Config.Bind("LanternSettings",
                                                "ControlCandleLanterns",
                                                true, // default value
                                                "Enable or disable control for candle lanterns (true or false)");

            controlOilLanterns = Config.Bind("LanternSettings",
                                             "ControlOilLanterns",
                                             true, // default value
                                             "Enable or disable control for oil lanterns (true or false)");
            refillCandleLanterns = Config.Bind("LanternSettings",
                                             "RefillCandleLanterns",
                                             true, // default value
                                             "Enable or disable refilling candles in candle lanterns (true or false)");

            Logger.LogInfo($"Lantern Start Time: {lanternStartTime.Value}");
            Logger.LogInfo($"Lantern Stop Time: {lanternStopTime.Value}");
        }
    }
}
