using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public event Action<Team> DeclareWinnerClientEvent;
    public event Action<Team> DeclareWinnerServerEvent;
    private TeamManager teamManager;
    private GameplaySceneManager gameplaySceneManager;
    private TMP_Text displayNameInputText;
    private TMP_Text addressInputText;
    private TMP_Text portInputText;
    private FixedString32Bytes localPlayerName;

    public void Awake()
    {
        teamManager = FindAnyObjectByType<TeamManager>();
        gameplaySceneManager = FindAnyObjectByType<GameplaySceneManager>();
        displayNameInputText = GameObject.Find("DisplayNameInputField").transform.Find("Text Area").Find("Text").GetComponentInChildren<TMP_Text>();
        addressInputText = GameObject.Find("IpAddressInputField").transform.Find("Text Area").Find("Text").GetComponentInChildren<TMP_Text>();
        portInputText = GameObject.Find("PortNumberInputField").transform.Find("Text Area").Find("Text").GetComponentInChildren<TMP_Text>();

        var hostSessionButton = GameObject.Find("HostSessionButton").GetComponent<Button>();
        var joinSessionButton = GameObject.Find("JoinSessionButton").GetComponent<Button>();
        hostSessionButton.onClick.AddListener(HostSession);
        joinSessionButton.onClick.AddListener(JoinSession);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        NetworkManager.OnClientConnectedCallback += (connectedClientId) =>
        {
            if (NetworkManager.LocalClientId == connectedClientId)
            {
                var clientTank = NetworkManager.LocalClient.PlayerObject.GetComponent<Tank>();
                clientTank.SetPlayerNameServerRpc(localPlayerName);
            }

            if (!IsServer)
            {
                return;
            }

            var tank = NetworkManager.ConnectedClients[connectedClientId].PlayerObject.GetComponent<Tank>();
            tank.transform.position = new Vector3(0, 10, 0);
            tank.gameObject.SetActive(false); // TODO: There must be a better way to "disable" a network object? I want the NetObj to be completely disabled on client and host 
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

    public void HostSession()
    {
        PrepareSession();
        //var unityTransport = NetworkManager.GetComponent<UnityTransport>();
        //unityTransport.ConnectionData = new UnityTransport.ConnectionAddressData
        //{
        //    Address = "127.0.0.1",
        //    Port = ushort.Parse(portInputText.text.Substring(0, portInputText.text.Length - 1)),
        //    ServerListenAddress = "0.0.0.0",
        //};

        NetworkManager.StartHost();
    }

    public void JoinSession()
    {
        PrepareSession();
        var unityTransport = NetworkManager.GetComponent<UnityTransport>();
        unityTransport.ConnectionData = new UnityTransport.ConnectionAddressData
        {
            Address = addressInputText.text.Substring(0, addressInputText.text.Length - 1),
            Port = ushort.Parse(portInputText.text.Substring(0, portInputText.text.Length - 1)),
            ServerListenAddress = "0.0.0.0",
        };
        NetworkManager.StartClient();
    }

    private void PrepareSession()
    {
        // Clear unneeded objects from the "Gameplay" scene
        var camera = GameObject.Find("Camera");
        var canvas = GameObject.Find("Canvas");
        var eventSystem = GameObject.Find("EventSystem");
        Destroy(camera);
        Destroy(canvas);
        Destroy(eventSystem);

        localPlayerName = displayNameInputText.text.Substring(0, displayNameInputText.text.Length - 1);
    }

    public void BeginGame()
    {
        gameplaySceneManager.LoadGameplayScene();
        foreach (var tank in NetworkManager.ConnectedClientsList.Select(c => c.PlayerObject.GetComponent<Tank>()))
        {
            tank.gameObject.SetActive(true);
        }
    }

    public void LeaveSession()
    {
        NetworkManager.Shutdown();
        Destroy(NetworkManager.gameObject); // Destroy manually since NetworkManager lives in DDoL
        EndSessionClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void EndSessionClientRpc()
    {
        gameplaySceneManager.LeaveGame();
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
