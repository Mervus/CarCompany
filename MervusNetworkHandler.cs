using System.Collections.Generic;
using CarCompany.Scripts;
using GameNetcodeStuff;
using LethalNetworkAPI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

namespace CarCompany;

public class MervusNetworkHandler
{
    public static LethalClientMessage<Vector3> SpawnCarClientMessage = new ("spawnCar");
    public static LethalServerMessage<Vector3> SpawnCarServerMessage = new ("spawnCar");

    public static LethalClientMessage<ulong> AttachPlayerToCarClientMessage = new("attachToCar");
    public static LethalServerMessage<ulong> AttachPlayerToCarServerMessage = new("attachToCar");

    public static LethalClientMessage<ulong> DetachPlayerFromCarClientMessage = new("detachToCar");
    public static LethalServerMessage<ulong> DetachPlayerFromCarServerMessage = new("detachToCar");
    
    public static GameObject CarGameObject = null;
    
    public static bool isOnCar = false;    
    public static void Start()
    {
        SpawnCarServerMessage.OnReceived += SERVER_SpawnCar;
        SpawnCarClientMessage.OnReceivedFromClient += CLIENT_SpawnCar;

        AttachPlayerToCarServerMessage.OnReceived += SERVER_AttachPlayerToCar;
        AttachPlayerToCarClientMessage.OnReceivedFromClient += CLIENT_AttachToCar;

        DetachPlayerFromCarServerMessage.OnReceived += SERVER_DetachFromCar;
        DetachPlayerFromCarClientMessage.OnReceivedFromClient += CLIENT_DetachFromCar;
    }
    
    public static void CLIENT_SpawnCar(Vector3 spawnPos, ulong clientId)
    {
        CarModBase.Logger.LogInfo("Spawned Car on Client" + clientId.GetPlayerFromId().playerUsername);
    }

    private static void SERVER_SpawnCar(Vector3 spawnPos, ulong clientId)
    {
        CarModBase.Logger.LogInfo("ClientSpawnCar");

        if (CarGameObject == null)
        {
            GameObject gameObject = Object.Instantiate(CarModBase.FiatPrefab, spawnPos, new Quaternion(0f, 0f, 0f, 0f));
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
                CarGameObject.transform.position = spawnPos;
                CarGameObject.transform.rotation = Quaternion.identity;
            }
        }
        
        SpawnCarServerMessage.SendAllClients(spawnPos);
    }
    
    private static void CLIENT_AttachToCar(ulong playerId, ulong clientId)
    {
        CarModBase.Logger.LogInfo("Attached player to car on Client" + playerId.GetPlayerFromId().playerUsername);
    }
    
    private static void SERVER_AttachPlayerToCar(ulong playerId, ulong clientId)
    {
        CarModBase.Logger.LogInfo(MervusNetworkHandler.CarGameObject.name);
        
        PlayerControllerB playerController = playerId.GetPlayerFromId();
        
        if (playerController == null)
        {
            CarModBase.Logger.LogError("AttachPlayerToCarServerRpc: playerController is null");
            return;
        }

        if (CarGameObject == null)
        {
            CarModBase.Logger.LogError("AttachPlayerToCarServerRpc: CarGameObject is null");
            return;
        }
        
        CarModBase.Logger.LogInfo(CarGameObject.transform.parent.gameObject);
        
        playerController.gameObject.transform.parent = CarGameObject.transform.parent;
        
        AttachPlayerToCarClientMessage.SendAllClients(playerId);
    }

    private static void CLIENT_DetachFromCar(ulong playerId, ulong clientId)
    {
        CarModBase.Logger.LogError("CLIENT_DetachFromCar: hello  niggo client msseage");
    }

    private static void SERVER_DetachFromCar(ulong playerId, ulong clientId)
    {
        PlayerControllerB playerController = playerId.GetPlayerFromId();

        if (StartOfRound.Instance == null)
        {
            CarModBase.Logger.LogError("DetachPlayerFromCarServerRpc: StartOfRound Instance is null");
            return;
        }

        if (playerController == null)
        {
            CarModBase.Logger.LogError("DetachPlayerFromCarServerRpc: playerController is null");
            return;
        }

        playerController.gameObject.transform.parent = StartOfRound.Instance.playersContainer.transform;
        
        DetachPlayerFromCarServerMessage.SendAllClients(playerId);
    }
}