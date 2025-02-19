using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LivesObjectiveType : BaseObjectiveType
{
    [field: SerializeField] public int NumLivesToAdd { get; set; }

    public override void OnStart()
    {
        NetworkLog.LogInfoServer("Lives objective started!");
    }

    public override void OnCapture(IEnumerable<ulong> teamMembersClientIds)
    {
        foreach (var teamMemberClientId in teamMembersClientIds)
        {
            var client = NetworkManager.Singleton.ConnectedClients[teamMemberClientId];
            var tank = client.PlayerObject.GetComponent<Tank>();
            tank.AddLives(NumLivesToAdd);
        }
    }
}