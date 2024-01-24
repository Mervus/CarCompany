using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace CarCompany.Patches;

[HarmonyPatch(typeof(StartOfRound))]
public class StartOfRoundPatch
{
    [HarmonyPatch("ShipLeave")]
    [HarmonyPostfix]
    static void patchShipLeave()
    {
        UnityEngine.Object.Destroy(MervusNetworkHandler.CarGameObject);
        MervusNetworkHandler.CarGameObject = null;
    }
}