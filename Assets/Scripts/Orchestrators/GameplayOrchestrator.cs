using System;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;

public class GameplayOrchestrator : NetworkBehaviour
{
    [field: SerializeField] public GameObject SpecatorCameraPrefab;
    [field: SerializeField] public GameObject MainCameraPrefab;

    public event Action<Team> DeclareWinnerServerEvent;
    public event Action<Team> DeclareWinnerClientEvent;

    private GameplaySceneManager _gameplaySceneManager;
    private TeamManager _teamManager;
    private ObjectiveManager _objectiveManager;
    private RespawnManager _respawnManager;

    private Tank _ownerTank;
    private MainCamera _ownerMainCamera;
    private SpectatorCameraController _ownerSpectatorCameraController;

    private void Start()
    {
        _teamManager = FindAnyObjectByType<TeamManager>();
        _gameplaySceneManager = FindAnyObjectByType<GameplaySceneManager>();
        _objectiveManager = FindAnyObjectByType<ObjectiveManager>();
        _respawnManager = FindAnyObjectByType<RespawnManager>();

        if (IsClient)
        {
            _gameplaySceneManager.LoadGameplayOverlay();
            
            _ownerTank = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Tank>();

            var mainCameraPos = _ownerTank.transform.Find("MainCameraPosition").transform;

            _ownerMainCamera = Instantiate(MainCameraPrefab, mainCameraPos).GetComponent<MainCamera>();
            _ownerSpectatorCameraController = Instantiate(SpecatorCameraPrefab, mainCameraPos).GetComponent<SpectatorCameraController>();

            _ownerTank.DeathClientEvent += OnTankDeathOwner;
            _ownerTank.RevivalClientEvent += OnTankRevivalOwner;
        }

        if (IsServer)
        {
            _gameplaySceneManager.LoadLevelScene();
            _gameplaySceneManager.GameplaySceneLoadEvent += OnGameplaySceneLoad_Server;
        }
    }

    private void OnGameplaySceneLoad_Server()
    {
        NetworkLog.LogInfoServer("Gameplay scene loaded!");

        // TODO: Should unsubscribe to the event on perform?

        _objectiveManager.FindObjectives();
        _objectiveManager.StartTimer();
        _respawnManager.FindSpawnPoints();

        var clientIds = NetworkManager.ConnectedClientsIds;
        foreach (var clientId in clientIds)
        {
            DeployTank_Server(clientId);
        }
    }

    private void DeployTank_Server(ulong clientId)
    {
        NetworkLog.LogInfoServer("Deploying tank for clientid: " + clientId);

        var tank = NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<Tank>();
        _respawnManager.Respawn(tank);
        tank.DeathServerEvent += () => HandleTankDeath(clientId, tank);
        tank.Deploy_Server();
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

    private void OnTankDeathOwner()
    {
        _ownerMainCamera.enabled = false;
        _ownerSpectatorCameraController.BeginSpectating();
        _ownerSpectatorCameraController.transform.SetPositionAndRotation(_ownerMainCamera.transform.position, _ownerMainCamera.transform.rotation);
    }

    private void OnTankRevivalOwner()
    {
        _ownerMainCamera.enabled = true;
        _ownerSpectatorCameraController.EndSpectating();
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
