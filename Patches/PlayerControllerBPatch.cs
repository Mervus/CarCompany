using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CarCompany.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
internal class PlayerControllerBPatch
{
    [HarmonyPatch("Update")]
    [HarmonyPrefix]
    static void patchUpdate(ref PlayerControllerB __instance)
    {
        if (__instance.IsOwner && __instance.isPlayerControlled && (!__instance.IsServer || __instance.isHostPlayerObject) || __instance.isTestingPlayer)
        {
            if (Keyboard.current[Key.F8].wasPressedThisFrame)
            {
                Vector3 spawnPosition = __instance.transform.position;

                CarModBase.Logger.LogInfo(__instance.playerUsername);
                CarModBase.Logger.LogInfo(__instance.transform.position);

                spawnPosition.x += 2f;
                spawnPosition.y += 2f;
            
                MervusNetworkHandler.SpawnCarClientMessage.SendServer(spawnPosition);
                CarModBase.Logger.LogInfo("Calling Server Spawn Car");
            }
        }
    }
    [HarmonyPatch("LateUpdate")]
    [HarmonyPrefix]
    static void patchLateUpdate(ref PlayerControllerB __instance)
    {
        if (__instance.IsOwner && __instance.isPlayerControlled && (!__instance.IsServer || __instance.isHostPlayerObject) || __instance.isTestingPlayer)
        {
            
        }
    }
    
}
