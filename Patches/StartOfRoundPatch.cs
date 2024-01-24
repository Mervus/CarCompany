using System.Runtime.CompilerServices;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CarCompany.Patches;

[HarmonyPatch(typeof(StartOfRound))]
public class StartOfRoundPatch
{
    [HarmonyPatch("ShipLeave")]
    [HarmonyPostfix]
    static void patchShipLeave()
    {
        UnityEngine.Object.Destroy(MervusNetworkHandler.CarGameObject);
    }
    
    [HarmonyPatch("Update")]
    [HarmonyPrefix]
    static void patchUpdate(ref StartOfRound __instance)
    {
       
    }
}