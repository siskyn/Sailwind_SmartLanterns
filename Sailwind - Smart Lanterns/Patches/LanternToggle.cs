﻿using System;
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
                ToggleLanterns(true);
            }
            else
            {
                ToggleLanterns(false);
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
        private static void ToggleLanterns(bool turnOn)
        {
            ShipItemLight[] allLanterns = UnityEngine.Object.FindObjectsOfType<ShipItemLight>();
            foreach (var lantern in allLanterns)
            {
                if (lantern != null)
                {
                    // Set lantern's state based on the time range
                    MethodInfo setLightMethod = typeof(ShipItemLight).GetMethod("SetLight", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (setLightMethod != null)
                    {
                        setLightMethod.Invoke(lantern, new object[] { turnOn });
                        FileLog.Log($"Lantern {lantern.name} turned {(turnOn ? "on" : "off")}");
                    }
                    else
                    {
                        FileLog.Log("SetLight method not found on " + lantern.name);
                    }
                }
            }
        }

    }
}
