using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public event Action<Team> DeclareWinnerClientEvent;
    public event Action<Team> DeclareWinnerServerEvent;
    private TeamManager teamManager;
    //private ObjectiveManager objectiveManager;

    public void Awake()
    {
        teamManager = FindObjectsByType<TeamManager>(FindObjectsSortMode.None).First();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        NetworkManager.OnClientConnectedCallback += (connectedClientId) =>
        {
            var tank = NetworkManager.ConnectedClients[connectedClientId].PlayerObject.GetComponent<Tank>();
            tank.DeathServerEvent += () =>
            {
                NetworkLog.LogInfoServer($"Player [{connectedClientId}] died.");
                if (teamManager.GetTeam(connectedClientId) == Team.None)
                {
                    return;
                }

                // TODO: How should win conditions be resolved with varying amounts of team members?
                if (teamManager.AnyMembers(Team.Blue) && !teamManager.AnyMembersAlive(Team.Blue))
                {
                    DeclareWinner(Team.Orange);
                }
                else if (teamManager.AnyMembers(Team.Orange) && !teamManager.AnyMembersAlive(Team.Orange))
                {
                    DeclareWinner(Team.Blue);
                }
            };
        };
    }

    private void DeclareWinner(Team team)
    {
        NetworkLog.LogInfoServer($"{team.ToString()} team won!");
        DeclareWinnerServerEvent?.Invoke(team);
        DeclareWinnerClientRpc(team);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DeclareWinnerClientRpc(Team team)
    {
        DeclareWinnerClientEvent?.Invoke(team);

        // TODO: UI listeners for overlay updates
    }
}
