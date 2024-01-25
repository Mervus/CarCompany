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
    public GameObject CarGameObject = null;
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

    private Vector3 CarSpawnPosition(ulong clientId)
    {
        Vector3 spawnPosition = StartOfRound.Instance.allPlayerScripts[clientId].gameObject.transform.position;
        //TODO: Calculate forward spawn
        spawnPosition.x += 2f;
        spawnPosition.y += 2f;

        return spawnPosition;
    }

    [ClientRpc]
    private void SpawnCarClientRpc(ulong clientId)
    {
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnCarServerRpc(ulong clientId)
    {
        CarModBase.Logger.LogInfo("SpawnCarServerRpc");

        if (CarGameObject == null)
        {
        
            GameObject gameObject = Object.Instantiate(CarModBase.FiatPrefab, CarSpawnPosition(clientId), new Quaternion(0f, 0f, 0f, 0f));
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
                CarGameObject.transform.position = CarSpawnPosition(clientId);
                CarGameObject.transform.rotation = Quaternion.identity;
            }
        }
        CarModBase.Logger.LogInfo("Spawned Car on Client" + StartOfRound.Instance.allPlayerScripts[clientId].playerUsername);
        SpawnCarClientRpc(clientId);
    }

    [ServerRpc]
    public void DespawnCarServerRpc()
    {
        CarGameObject.GetComponent<NetworkObject>().Despawn();
        CarGameObject = null;
    }
   
}