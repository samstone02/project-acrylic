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
        if (NetworkManager.Singleton == null)
        {
            // On session leave, NetworkManager would be set to null and NullReferenceException would be thrown.
            return;
        }

        GUILayout.BeginArea(new Rect(10, 60, 300, 300));

        if (NetworkManager.Singleton.IsClient)
        {
            Team team = _teamManager.GetTeam(NetworkManager.Singleton.LocalClientId);
            
            if (team == Team.Blue)
            {
                RenderBlueTeamButtons();
            }
            else if (team == Team.Orange)
            {
                RenderOrangeTeamButtons();
            }
            else
            {
                RenderJoinTeamButtons();
            }
        }

        GUILayout.EndArea();
    }

    private void RenderBlueTeamButtons()
    {
        if (GUILayout.Button("Leave Blue Team"))
        {
            _teamManager.LeaveBlueTeamRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    private void RenderOrangeTeamButtons()
    {
        if (GUILayout.Button("Leave Orange Team"))
        {
            _teamManager.LeaveOrangeTeamRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    private void RenderJoinTeamButtons()
    {
        if (GUILayout.Button("Join Blue Team"))
        {
            _teamManager.JoinBlueTeamRpc(NetworkManager.Singleton.LocalClientId);
        }

        if (GUILayout.Button("Join Orange Team"))
        {
            _teamManager.JoinOrangeTeamRpc(NetworkManager.Singleton.LocalClientId);
        }
    }
}
