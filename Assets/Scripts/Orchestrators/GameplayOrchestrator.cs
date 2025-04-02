using System;
using Unity.Netcode;
using UnityEngine;

public class GameplayOrchestrator : NetworkBehaviour
{
    public event Action<Team> DeclareWinnerServerEvent;
    public event Action<Team> DeclareWinnerClientEvent;

    private GameplaySceneManager _gameplaySceneManager;
    private TeamManager _teamManager;
    private ObjectiveManager _objectiveManager;
    private RespawnManager _respawnManager;

    private void Start()
    {
        _teamManager = FindAnyObjectByType<TeamManager>();
        _gameplaySceneManager = FindAnyObjectByType<GameplaySceneManager>();
        _objectiveManager = FindAnyObjectByType<ObjectiveManager>();
        _respawnManager = FindAnyObjectByType<RespawnManager>();

        if (IsClient)
        {
            _gameplaySceneManager.LoadGameplayOverlay();
        }

        if (IsServer)
        {
            _gameplaySceneManager.LoadGameplayScene();
            _gameplaySceneManager.GameplaySceneLoadEvent += OnGameplaySceneLoad;
        }
    }

    private void OnGameplaySceneLoad()
    {
        // TODO: Should unsubscribe to the event on perform?

        _objectiveManager.FindObjectives();
        _objectiveManager.StartTimer();
        _respawnManager.FindSpawnPoints();

        var clientIds = NetworkManager.ConnectedClientsIds;
        foreach (var clientId in clientIds)
        {
            DeployTank(clientId);
        }
    }

    private void DeployTank(ulong clientId)
    {
        var tank = NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<Tank>();
        _respawnManager.Respawn(tank);
        tank.DeathServerEvent += () => HandleTankDeath(clientId, tank);
    }

    private void HandleTankDeath(ulong clientId, Tank tank)
    {
        NetworkLog.LogInfoServer($"Player [{clientId}] died.");
        if (_teamManager.GetTeam(clientId) == Team.None)
        {
            return;
        }

        // TODO: How should win conditions be resolved with varying amounts of team members?
        if (_teamManager.AnyMembers(Team.Blue) && !_teamManager.AnyMembersAlive(Team.Blue))
        {
            DeclareWinner(Team.Orange);
            return;
        }
        else if (_teamManager.AnyMembers(Team.Orange) && !_teamManager.AnyMembersAlive(Team.Orange))
        {
            DeclareWinner(Team.Blue);
            return;
        }

        _respawnManager.RespawnAfterDelay(tank);
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
    }
}
