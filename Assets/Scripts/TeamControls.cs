using Unity.Netcode;
using UnityEngine;

public class TeamControls : MonoBehaviour
{
    private TeamManager _teamManager;

    void Start()
    {
        _teamManager = GetComponent<TeamManager>();
    }

    protected void OnGUI()
    {
        if (NetworkManager.Singleton.IsClient)
        {
            string teamName = _teamManager.GetTeamName(NetworkManager.Singleton.LocalClientId);
            
            if (teamName == "blue")
            {
                RenderBlueTeamButtons();
            }
            else if (teamName == "orange")
            {
                RenderOrangeTeamButtons();
            }
            else
            {
                RenderJoinTeamButtons();
            }
        }
    }

    private void RenderBlueTeamButtons()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));

        if (GUILayout.Button("Leave Blue Team"))
        {
            _teamManager.LeaveBlueTeamRpc(NetworkManager.Singleton.LocalClientId);
        }

        GUILayout.EndArea();
    }

    private void RenderOrangeTeamButtons()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));

        if (GUILayout.Button("Leave Orange Team"))
        {
            _teamManager.LeaveOrangeTeamRpc(NetworkManager.Singleton.LocalClientId);
        }

        GUILayout.EndArea();
    }

    private void RenderJoinTeamButtons()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));

        if (GUILayout.Button("Join Blue Team"))
        {
            _teamManager.JoinBlueTeamRpc(NetworkManager.Singleton.LocalClientId);
        }

        if (GUILayout.Button("Join Orange Team"))
        {
            _teamManager.JoinOrangeTeamRpc(NetworkManager.Singleton.LocalClientId);
        }

        GUILayout.EndArea();
    }
}
