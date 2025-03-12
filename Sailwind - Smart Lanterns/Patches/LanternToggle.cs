using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib.Tools;
using System.Collections.Concurrent;
using System.Reflection;

namespace Smart_Lanterns.Patches
{
    /*
     * Original game code below. 


	*/

    [HarmonyPatch(typeof(Sun), "Update")]  // class type followed by overwritten method.

    internal static class LanternToggle
    {
        static bool isSwitchedOn = false;
        internal static ManualLogSource myLog = Plugin.logSource;

        static void Postfix(Sun __instance, ref float ___localTime)
        {
            
            //float localTime = Sun.sun.localTime; // Get the local (timezone-adjusted) time

            float startTime = Plugin.lanternStartTime.Value;
            float stopTime = Plugin.lanternStopTime.Value;            

            //FileLog.Log("Local time is: " + ___localTime);
            //FileLog.Log($"Lantern start time: {startTime}");
            //FileLog.Log($"Lantern stop time: {stopTime}");
            if (IsTimeInRange(___localTime, startTime, stopTime))
            {
                if (!isSwitchedOn || Plugin.DEBUGMODE)
                {
                    SetLanterns(true);
                    isSwitchedOn = true;
                }
            }
            else
            {
                if (isSwitchedOn || Plugin.DEBUGMODE)
                {
                    SetLanterns(false);
                    isSwitchedOn = false;
                }
            }

        }

        private static bool IsTimeInRange(float currentTime, float startTime, float stopTime)
        {
            // Simple time range check, handling case where the time range spans over midnight
            if (startTime < stopTime)
            {
                // If the start time is less than the stop time, check if the current time is within that range
                return currentTime >= startTime && currentTime <= stopTime;
            }
            else
            {
                // If the time range spans midnight, check if the time is within the two ranges
                return currentTime >= startTime || currentTime <= stopTime;
            }
        }
        private static void SetLanterns(bool turnOn)
        {
            int i = 1;
            myLog.LogInfo("Switching all hooked lanterns to: " + (turnOn ? "on" : "off"));
            ShipItemLight[] allLanterns = UnityEngine.Object.FindObjectsOfType<ShipItemLight>();
            foreach (var lantern in allLanterns)
            {
                if (lantern != null)
                {

                    myLog.LogInfo("Lantern " + i + " found. Does it use oil? " + lantern.usesOil);
                    //FileLog.Log("Lantern " + lantern.name + " uses oil: " + lantern.usesOil);
                    //FileLog.Log("Lantern " + lantern.name + " uses oil: " + lantern.);

                    //FileLog.Log("Lantern " + lantern.name + " is hanging: " + lantern.GetComponent<HangableItem>().IsHanging());


                    if ((!lantern.usesOil && Plugin.controlCandleLanterns.Value) || (lantern.usesOil && Plugin.controlOilLanterns.Value)) 
                    {//only touch it if we're meant to control the type of lantern the current one is.
                        myLog.LogInfo("Lantern  " + i + "is meant to be controlled.");

                        if (lantern.GetComponent<HangableItem>().IsHanging())
                        {//only touch the lantern if it's hanging on a hook
                            myLog.LogInfo("Lantern  " + i + "is hanging on a hook. Pull the trigger.");

                            MethodInfo setLightMethod = typeof(ShipItemLight).GetMethod("SetLight", BindingFlags.NonPublic | BindingFlags.Instance);
                            if (setLightMethod != null)
                            {
                                setLightMethod.Invoke(lantern, new object[] { turnOn });
                                //FileLog.Log($"Lantern {lantern.name} turned {(turnOn ? "on" : "off")}");
                            }
                            else
                            {
                                //FileLog.Log("SetLight method not found on " + lantern.name);
                            }
                            myLog.LogInfo("Lantern  " + i + " has been switched.");
                        }
                    }
                }
                i++;
            }
        }

    }
}
