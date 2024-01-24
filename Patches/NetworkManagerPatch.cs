using CarCompany.Scripts;
using HarmonyLib;
using Unity.Netcode;

namespace CarCompany.Patches;

[HarmonyPatch(typeof(GameNetworkManager))]
internal class NetworkManagerPatch
{
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    static void patchStart(ref GameNetworkManager __instance )
    {
        CarModBase.Logger.LogInfo("Adding CarModBase to NetworkPrefab List.");
        NetworkManager.Singleton.AddNetworkPrefab(CarModBase.FiatPrefab);
    }
}