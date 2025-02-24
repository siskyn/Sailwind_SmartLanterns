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

namespace Smart_Lanterns.Patches
{
    /*
     * Original game code below. 
     * plan: prefix check health. if above 0, run original method. if less or equal to 0, , must remove a candle from storage and restore health to full then run original method.
     * 
     * 	// Token: 0x060003B8 RID: 952 RVA: 0x00018858 File Offset: 0x00016A58
	public override void ExtraLateUpdate()
	{
		if (this.on)
		{
			this.health -= Time.deltaTime * Sun.sun.timescale / 50f * this.fuelConsumptionRate;
			if (this.health <= 0f)
			{
				this.health = 0f;
				this.SetLight(false);
			}
			if (Settings.lanternShadows)
			{
				this.light.shadows = LightShadows.Soft;
				return;
			}
			this.light.shadows = LightShadows.None;
		}
	}

	*/

    [HarmonyPatch(typeof(ShipItemLight), "ExtraLateUpdate")]  // class type followed by overwritten method.

    internal static class LanternRefill
    {

        static bool Prefix(ShipItemLight __instance, ref int ___health, ref int ___initialHealth)
        {

            if (___health <= 0f) // but we can read it directly without ref
            {
                FileLog.Log("health <= 0, triggering candle restock.");
                bool result = consumeCandle();
                if (result)
                {
                    ___health = ___initialHealth;//we succesfully removed a candle from storage, so refill the lantern.
                    FileLog.Log("Lantern restocked succesfully.");
                }
            }

            //FileLog.Log("candle health is " + ___health);
            return true; // if a candle ran out, we already replaced it, so allow the original code to run. as far as it's concerned, the candle never ran out.
        }

        private static bool consumeCandle()
        {
            ShipItemCrate[] allCrates = UnityEngine.Object.FindObjectsOfType<ShipItemCrate>();
            foreach (ShipItemCrate crate in allCrates)
            {
                if (crate.name == "lantern candles")//this will only identify sealed candle crates. we actually want to avoid those. need to verify contents instead. See methods in class "ShipitemCrate"
                {
                    FileLog.Log("found sealed lantern crate with " + crate.amount + " candles inside.");
                    if (crate.amount > 0 && crate.sold)
                    {//valid crate to pull a candle from
                        
                        crate.amount--;  // Remove one candle
                        FileLog.Log("Used 1 candle from storage.");
                        return true;  // Found and consumed a candle, exit early
                    }
                }
            }
                return false; // could not find a candle to use.
        }

    }
}
