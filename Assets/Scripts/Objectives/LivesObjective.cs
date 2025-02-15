using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LivesObjective : BaseObjective
{
    [field: SerializeField] public int NumLivesToAdd { get; set; }

    protected override void StartObjective()
    {
        NetworkLog.LogInfoServer("Lives objective started!");
    }

    protected override void OnCapture(IEnumerable<ulong> teamMembersClientIds)
    {
        base.OnCapture(teamMembersClientIds);

        foreach (var teamMemberClientId in teamMembersClientIds)
        {
            var client = NetworkManager.Singleton.ConnectedClients[teamMemberClientId];
            var tank = client.PlayerObject.GetComponent<Tank>();
            tank.AddLives(NumLivesToAdd);
        }
    }
}