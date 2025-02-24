using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using BepInEx;

namespace Smart_Lanterns.Patches
{
    /*
     * Example original code:
     * 
    public class SomeGameClass
    {
        public bool isRunning;
        public int counter;

        private int DoSomething()
        {
            if (isRunning)
            {
                counter++;
            }
            return counter * 10;
        }
    }

	*/


    /*
    example overwriting code:

    [HarmonyPatch(typeof(SomeGameClass))]
    [HarmonyPatch("DoSomething")] // if possible use nameof() here
    internal static class PatchTemplateClass
    {
        static AccessTools.FieldRef<SomeGameClass, bool> isRunningRef =
                AccessTools.FieldRefAccess<SomeGameClass, bool>("isRunning");

        static bool Prefix(SomeGameClass __instance, ref int ___counter)
        {
            isRunningRef(__instance) = true;
            if (___counter > 100)
                return false;
            ___counter = 0;
            return true;
        }

        static void Postfix(ref int __result) => __result *= 2;

    }*/
}
