using System;
using CarCompany.Scripts;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace CarCompany;

public class MervusNetworkHandler : NetworkBehaviour
{
    public static GameObject CarGameObject = null;
    private PlayerControllerB localPlayer;

    private void Update()
    {
        if (IsOwner)
        {
            if (localPlayer == null)
            {
                localPlayer = StartOfRound.Instance.localPlayerController;
            }
            
            if (Keyboard.current[Key.F8].wasPressedThisFrame)
            {
                CarModBase.Logger.LogInfo(localPlayer.playerUsername);
                SpawnCarServerRpc(localPlayer.playerClientId);
                CarModBase.Logger.LogInfo("Calling Server Spawn Car");
            }
        }
    }

    [ClientRpc]
    private void SpawnCarClientRpc(ulong clientId)
    {
        CarModBase.Logger.LogInfo("ClientSpawnCar");

        Vector3 spawnPosition = StartOfRound.Instance.allPlayerScripts[clientId].gameObject.transform.position;
        
        if (CarGameObject == null)
        {
            GameObject gameObject = Object.Instantiate(CarModBase.FiatPrefab, spawnPosition, new Quaternion(0f, 0f, 0f, 0f));
            gameObject.transform.GetChild(0).Find("WheelColliders").gameObject.AddComponent<WheelController>();
            gameObject.transform.Find("PlayerOnCarHandler").gameObject.AddComponent<HandlePlayerOnCar>();
            
            gameObject.GetComponent<NetworkObject>().Spawn();
            
            gameObject.SetActive(true);
            CarGameObject = gameObject;
        }
        else
        {
            if (clientId == CarGameObject.GetComponent<NetworkObject>().OwnerClientId)
            {
                CarGameObject.transform.position = spawnPosition;
                CarGameObject.transform.rotation = Quaternion.identity;
            }
        }
        CarModBase.Logger.LogInfo("Spawned Car on Client" + StartOfRound.Instance.allPlayerScripts[clientId].playerUsername);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnCarServerRpc(ulong clientId)
    {
        SpawnCarClientRpc(clientId);
    }

   
}