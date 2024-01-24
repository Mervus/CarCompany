using System;
using System.Collections.Generic;
using GameNetcodeStuff;
using LethalNetworkAPI;
using RuntimeNetcodeRPCValidator;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace CarCompany.Scripts;

public class HandlePlayerOnCar : NetworkBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (!this.IsOwner && !IsLocalPlayer)
        {
            return;
        }
        
        if (collision.gameObject.name == "PlayerOnCarHandler")
        {
            CarModBase.Logger.LogInfo("Entering Car Trigger");

            AttachPlayerToCarServerRpc(this.OwnerClientId, collision.gameObject.GetComponentInParent<NetworkObject>().NetworkObjectId);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (!this.IsOwner && !IsLocalPlayer)
        {
            return;
        }
        
        if (collision.gameObject.name == "PlayerOnCarHandler")
        {
            CarModBase.Logger.LogInfo("Exiting Car Trigger");

            DetachPlayerFromCarServerRpc(this.OwnerClientId, collision.gameObject.GetComponentInParent<NetworkObject>().NetworkObjectId);
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void AttachPlayerToCarServerRpc(ulong senderId, ulong carNetworkId)
    {
        this.AttachPlayerToCarClientRpc(senderId, carNetworkId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DetachPlayerFromCarServerRpc(ulong senderId, ulong carNetworkId)
    {
        this.DetachPlayerFromCarClientRpc(senderId, carNetworkId);
    }

    [ClientRpc]
    private void AttachPlayerToCarClientRpc(ulong senderId, ulong carNetworkId)
    {
        PlayerControllerB playerFromId = StartOfRound.Instance.allPlayerScripts[senderId];
        if (playerFromId == null)
        {
            CarModBase.Logger.LogError((object) "AttachPlayerToCarServerRpc: playerController is null");
        }
        else
        {
            GameObject carObj = GetNetworkObject(carNetworkId).gameObject;
            this.transform.parent = carObj.transform;
        }
    }

    [ClientRpc]
    private void DetachPlayerFromCarClientRpc(ulong senderId, ulong carNetworkId)
    {
        PlayerControllerB playerFromId = StartOfRound.Instance.allPlayerScripts[senderId];

        playerFromId.transform.parent = StartOfRound.Instance.playersContainer;
    }
    
 }