using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUiManager : MonoBehaviour
{
    [field: SerializeField] public GameObject TeamMemberPanel { get; private set; }
    private Button _joinBlueButton;
    private Button _joinOrangeButton;
    private TeamManager _teamManager;
    private GameObject _blueTeamList;
    private GameObject _orangeTeamList;

    private void Start()
    {
        _teamManager = FindAnyObjectByType<TeamManager>();
        var gameManager = FindAnyObjectByType<SessionOrchestrator>();
        var buttons = GetComponentsInChildren<Button>();
        _joinBlueButton = buttons.First(b => b.name == "JoinBlueTeamButton");
        _joinOrangeButton = buttons.First(b => b.name == "JoinOrangeTeamButton");
        _joinBlueButton.onClick.AddListener(() => _teamManager.JoinBlueTeamRpc(NetworkManager.Singleton.LocalClientId));
        _joinOrangeButton.onClick.AddListener(() => _teamManager.JoinOrangeTeamRpc(NetworkManager.Singleton.LocalClientId));
        _blueTeamList = GameObject.Find("Panel")
            .transform.Find("PlayersTable")
            .transform.Find("BlueTeamColumn")
            .transform.Find("TeamMemberList").gameObject;
        _orangeTeamList = GameObject.Find("Panel")
            .transform.Find("PlayersTable")
            .transform.Find("OrangeTeamColumn")
            .transform.Find("TeamMemberList").gameObject;
        _teamManager.PlayerChangeTeamClientEvent += BuildTeamTable;
        var gameplaySceneManager = FindAnyObjectByType<GameplaySceneManager>();
        var startGameButton = buttons.First(b => b.name == "StartSessionButton");
        if (NetworkManager.Singleton.IsHost)
        {
            startGameButton.onClick.AddListener(() => gameManager.BeginGame());
        }
        else
        {
            startGameButton.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        _teamManager.PlayerChangeTeamClientEvent -= BuildTeamTable;
    }

    private void BuildTeamTable(ulong clientId, Team team)
    {
        var panels = _blueTeamList.GetComponentsInChildren<TextMeshProUGUI>()
            .Concat(_orangeTeamList.GetComponentsInChildren<TextMeshProUGUI>());

        foreach (var b in panels)
        {
            DestroyImmediate(b.gameObject);
        }

        var nametags = FindObjectsByType<PlayerNametag>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        var blueTeam = _teamManager.GetTeamMembers(Team.Blue)
            .Where(tm => clientId != tm);

        var orangeTeam = _teamManager.GetTeamMembers(Team.Orange)
            .Where(tm => clientId != tm);

        if (team == Team.Blue)
        {
            blueTeam = blueTeam.Append(clientId);
        }
        else if (team == Team.Orange)
        {
            orangeTeam = orangeTeam.Append(clientId);
        }

        foreach (var cid in blueTeam)
        {
            var panel = Instantiate(TeamMemberPanel, _blueTeamList.transform);
            var playerName = panel.GetComponentInChildren<TextMeshProUGUI>();
            var t = nametags.First(t => t.OwnerClientId == cid);
            playerName.text = t.PlayerName.ToString();
        }

        foreach (var cid in orangeTeam)
        {
            var panel = Instantiate(TeamMemberPanel, _orangeTeamList.transform);
            var playerName = panel.GetComponentInChildren<TextMeshProUGUI>();
            var t = nametags.First(t => t.OwnerClientId == cid);
            playerName.text = t.PlayerName.ToString();
        }
    }
}