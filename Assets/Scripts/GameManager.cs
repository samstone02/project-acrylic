using System;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public event Action<Team> DeclareWinnerClientEvent;
    public event Action<Team> DeclareWinnerServerEvent;
    private TeamManager teamManager;
    private GameplaySceneManager gameplaySceneManager;
    private TMP_Text addressInputText;
    private TMP_Text portInputText;

    public void Awake()
    {
        teamManager = FindAnyObjectByType<TeamManager>();
        gameplaySceneManager = FindAnyObjectByType<GameplaySceneManager>();
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
            if (!IsServer)
            {
                return;
            }

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

    public void HostSession()
    {
        var camera = GameObject.Find("Camera");
        var canvas = GameObject.Find("Canvas");
        var eventSystem = GameObject.Find("EventSystem");
        Destroy(camera);
        Destroy(canvas);
        Destroy(eventSystem);

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
        var camera = GameObject.Find("Camera");
        var canvas = GameObject.Find("Canvas");
        var eventSystem = GameObject.Find("EventSystem");
        Destroy(camera);
        Destroy(canvas);
        Destroy(eventSystem);

        var unityTransport = NetworkManager.GetComponent<UnityTransport>();
        unityTransport.ConnectionData = new UnityTransport.ConnectionAddressData
        {
            Address = addressInputText.text.Substring(0, addressInputText.text.Length - 1),
            Port = ushort.Parse(portInputText.text.Substring(0, portInputText.text.Length - 1)),
            ServerListenAddress = "0.0.0.0",
        };
        NetworkManager.StartClient();
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
