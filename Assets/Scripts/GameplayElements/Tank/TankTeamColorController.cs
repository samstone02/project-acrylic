using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class TankTeamColorController : NetworkBehaviour
{
    [field: SerializeField] public Color BlueTeamColor { get; set; }
    [field: SerializeField] public Color OrangeTeamColor { get; set; }
    [field: SerializeField] public Color NoTeamColor { get; set; }

    private MeshRenderer ChassisMesh { get; set; }
    private MeshRenderer TurretMesh { get; set; }

    private void Start()
    {
        if (!IsClient)
        {
            return;
        }

        var meshes = GetComponentsInChildren<MeshRenderer>();
        ChassisMesh = meshes.First(m => m.name.Contains("chassis"));
        TurretMesh = meshes.First(m => m.name.Contains("turret"));

        var teamManager = FindAnyObjectByType<TeamManager>();
        teamManager.PlayerChangeTeamClientEvent += SetMeshColor;
        SetMeshColor(OwnerClientId, teamManager.GetTeam(NetworkObject.OwnerClientId));
    }

    private void SetMeshColor(ulong netId, Team team)
    {
        if (netId != OwnerClientId)
        {
            return;
        }

        if (team == Team.Blue)
        {
            ChassisMesh.material.color = BlueTeamColor;
            TurretMesh.material.color = BlueTeamColor;
        }
        else if (team == Team.Orange)
        {
            ChassisMesh.material.color = OrangeTeamColor;
            TurretMesh.material.color = OrangeTeamColor;
        }
        else
        {
            ChassisMesh.material.color = NoTeamColor;
            TurretMesh.material.color = NoTeamColor;
        }
    }
}
