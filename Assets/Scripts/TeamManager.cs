using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

public class TeamManager : NetworkBehaviour
{
    private NetworkList<ulong> BlueTeam { get; set; } = new NetworkList<ulong>();
    private NetworkList<ulong> OrangeTeam { get; set; } = new NetworkList<ulong>();

    public event Action<ulong, Team> PlayerChangeTeamClientEvent;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        NetworkLog.LogInfoServer("Team Manager spawned!");
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        NetworkLog.LogInfoServer("Team Manager despawned!");
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

    public bool AnyMembers(Team team)
    {
        List<ulong> teamMemberIds = team == Team.Blue
            ? BlueTeam.ToList()
            : OrangeTeam.ToList();
        return teamMemberIds.Any();
    }

    public bool AnyMembersAlive(Team team)
    {
        if (team == Team.None)
        {
            throw new ArgumentException("Team.None is an invalid argument.");
        }

        List<ulong> teamMemberIds= team == Team.Blue
            ? BlueTeam.ToList()
            : OrangeTeam.ToList();
        IEnumerable<Tank> teamMemberTanks = teamMemberIds.Select(tmid => NetworkManager.ConnectedClients[tmid].PlayerObject.GetComponent<Tank>());
        return teamMemberTanks.Any(t => t.Lives > 0);
    }

    public IEnumerable<ulong> GetTeamMembers(Team team)
    {
        if (team == Team.Blue)
        {
            return BlueTeam.ToList();
        }
        else if (team == Team.Orange)
        {
            return OrangeTeam.ToList();
        }
        else
        {
            return Enumerable.Empty<ulong>();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayerChangeTeamRpc(ulong clientId, Team newTeam)
    {
        NetworkLog.LogInfoServer($"Joined {newTeam}");
        PlayerChangeTeamClientEvent?.Invoke(clientId, newTeam);
    }
}