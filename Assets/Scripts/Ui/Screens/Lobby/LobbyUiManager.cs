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
    private TMP_Text _selectedLevel;

    private void Start()
    {
        _teamManager = FindAnyObjectByType<TeamManager>();
        var gameManager = FindAnyObjectByType<SessionOrchestrator>();
        var buttons = GetComponentsInChildren<Button>();
        _joinBlueButton = buttons.First(b => b.name == "JoinBlueTeamButton");
        _joinOrangeButton = buttons.First(b => b.name == "JoinOrangeTeamButton");
        _joinBlueButton.onClick.AddListener(() => _teamManager.JoinBlueTeamRpc(NetworkManager.Singleton.LocalClientId));
        _joinOrangeButton.onClick.AddListener(() => _teamManager.JoinOrangeTeamRpc(NetworkManager.Singleton.LocalClientId));
        _blueTeamList = transform.Find("Panel")
            .transform.Find("PlayersTable")
            .transform.Find("BlueTeamColumn")
            .transform.Find("TeamMemberList").gameObject;
        _orangeTeamList = transform.Find("Panel")
            .transform.Find("PlayersTable")
            .transform.Find("OrangeTeamColumn")
            .transform.Find("TeamMemberList").gameObject;
        _teamManager.PlayerChangeTeamClientEvent += BuildTeamTable;

        _selectedLevel = transform.Find("Panel")
            .transform.Find("SelectedLevel")
            .GetComponent<TMP_Text>();

        var levelButtons = transform.Find("Panel")
            .transform.Find("LevelSelect")
            .GetComponentsInChildren<Button>();
        foreach (var button in levelButtons)
        {
            button.onClick.AddListener(() => SelectLevel(button.GetComponentInChildren<TMP_Text>().text));
        }

        var startGameButton = buttons.First(b => b.name == "StartSessionButton");
        if (NetworkManager.Singleton.IsHost)
        {
            startGameButton.onClick.AddListener(() =>
            {
                var text = _selectedLevel.GetComponent<TMP_Text>().text;
                SelectedLevelScene.SelectedLevelSceneName = _selectedLevel.GetComponent<TMP_Text>().text.Substring(0, text.Length);
                gameManager.BeginGame();
            });
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

    private void SelectLevel(string name)
    {
        _selectedLevel.text = name;
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