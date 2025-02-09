using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(TeamManager))]
[RequireComponent(typeof(ObjectiveManager))]
public class GameManager : NetworkBehaviour
{
    private TeamManager teamManager;
    private ObjectiveManager objectiveManager;
}
