using System;
using Unity.Netcode;

public class TeamManager : NetworkBehaviour
{
    private NetworkList<ulong> BlueTeam { get; } = new NetworkList<ulong>();
    private NetworkList<ulong> OrangeTeam { get; } = new NetworkList<ulong>();

    public event Action<ulong, Team> PlayerChangeTeamClientEvent;

    public override void OnNetworkSpawn()
    {
        NetworkLog.LogInfoServer("Team Manager spawned!");
    }

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

        PlayerChangeTeamRpc(clientId, Team.Blue);
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

        PlayerChangeTeamRpc(clientId, Team.Orange);
        OrangeTeam.Add(clientId);
    }

    [Rpc(SendTo.Server)]
    public void LeaveBlueTeamRpc(ulong clientId)
    {
        BlueTeam.Remove(clientId);
        PlayerChangeTeamRpc(clientId, Team.None);
    }

    [Rpc(SendTo.Server)]
    public void LeaveOrangeTeamRpc(ulong clientId)
    {
        OrangeTeam.Remove(clientId);
        PlayerChangeTeamRpc(clientId, Team.None);
    }

    public Team GetTeam(ulong clientId)
    {
        if (BlueTeam.Contains(clientId))
        {
            return Team.Blue;
        }
        else if (OrangeTeam.Contains(clientId))
        {
            return Team.Orange;
        }
        else
        {
            return Team.None;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayerChangeTeamRpc(ulong clientId, Team newTeam)
    {
        NetworkLog.LogInfoServer($"Joined {newTeam}");
        PlayerChangeTeamClientEvent?.Invoke(clientId, newTeam);
    }
}