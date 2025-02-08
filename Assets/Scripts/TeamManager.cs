using System;
using System.Diagnostics;
using Unity.Netcode;

public class TeamManager : NetworkBehaviour
{
    private NetworkList<ulong> BlueTeam { get; } = new NetworkList<ulong>();
    private NetworkList<ulong> OrangeTeam { get; } = new NetworkList<ulong>();

    public event Action<ulong, string> PlayerChangeTeamClientEvent;

    public const string BLUE = "blue";
    public const string ORANGE = "orange";
    public const string NO_TEAM = "no team";

    [Rpc(SendTo.Server)]
    public void JoinBlueTeamRpc(ulong clientId)
    {
        if (BlueTeam.Contains(clientId))
        {
            return;
        }

        if (OrangeTeam.Contains(clientId))
        {
            OrangeTeam.Remove(clientId);
        }

        PlayerChangeTeamRpc(clientId, BLUE);
        BlueTeam.Add(clientId);
    }

    [Rpc(SendTo.Server)]
    public void JoinOrangeTeamRpc(ulong clientId)
    {
        if (OrangeTeam.Contains(clientId))
        {
            return;
        }

        if (BlueTeam.Contains(clientId))
        {
            BlueTeam.Remove(clientId);
        }

        PlayerChangeTeamRpc(clientId, ORANGE);
        OrangeTeam.Add(clientId);
    }

    [Rpc(SendTo.Server)]
    public void LeaveBlueTeamRpc(ulong clientId)
    {
        BlueTeam.Remove(clientId);
        PlayerChangeTeamRpc(clientId, NO_TEAM);
    }

    [Rpc(SendTo.Server)]
    public void LeaveOrangeTeamRpc(ulong clientId)
    {
        OrangeTeam.Remove(clientId);
        PlayerChangeTeamRpc(clientId, NO_TEAM);
    }

    public string GetTeamName(ulong clientId)
    {
        if (BlueTeam.Contains(clientId))
        {
            return BLUE;
        }
        else if (OrangeTeam.Contains(clientId))
        {
            return ORANGE;
        }
        else
        {
            return NO_TEAM;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayerChangeTeamRpc(ulong clientId, string newTeam)
    {
        NetworkLog.LogInfoServer($"Joined {newTeam}");
        PlayerChangeTeamClientEvent?.Invoke(clientId, newTeam);
    }
}