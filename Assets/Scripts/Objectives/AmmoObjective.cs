using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AmmoObjective : BaseObjective
{
    [field: SerializeField] public int AmmoFillAmount { get; set; }

    protected override void StartObjective()
    {
        NetworkLog.LogInfoServer("Ammo objective started!");
    }

    protected override void OnCapture(IEnumerable<ulong> teamMembersClientIds)
    {
        base.OnCapture(teamMembersClientIds);

        foreach (var teamMemberClientId in teamMembersClientIds)
        {
            var client = NetworkManager.Singleton.ConnectedClients[teamMemberClientId];
            var tank = client.PlayerObject.GetComponent<Tank>();
            tank.FillAmmo(AmmoFillAmount);
        }
    }
}