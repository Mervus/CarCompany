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
    static void patchShipLeave(ref StartOfRound __instance)
    {
        MervusNetworkHandler mervusNetworkHandler = __instance.gameObject.GetComponent<MervusNetworkHandler>();

        if (mervusNetworkHandler == null)
        {
            CarModBase.Logger.LogError("Could not find MervusNetworkHandler");
            return;
        }

        mervusNetworkHandler.DespawnCarServerRpc();
    }
}