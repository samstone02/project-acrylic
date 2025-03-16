using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Objective : NetworkBehaviour
{
    [field: SerializeField] public float PrepTimeSeconds {  get; set; }

    [field: SerializeField] public float CaptureTimeSeconds { get; set; }

    [field: SerializeField] public string ObjectiveLocationName { get; set; }

    public Team ControllingTeam { get; private set; }

    public bool IsBlueContesting => IsBlueContestingNetVar.Value;
    public bool IsOrangeContesting => IsOrangeContestingNetVar.Value;

    public float PrepTimer { get; private set; }

    public float CaptureTimer { get; private set; }

    public event Action<Team> ObjectiveCapturedServerEvent;

    public event Action ObjectiveCapturedClientEvent;

    private TeamManager _TeamManager { get; set; }

    private MeshRenderer _MeshRenderer { get; set; }

    private NetworkVariable<float> PrepTimeNetVar { get; } = new NetworkVariable<float>();

    private NetworkVariable<float> CaptureTimerNetVar { get; } = new NetworkVariable<float>();

    private List<ulong> ContestingTanks { get; } = new List<ulong>();

    private NetworkVariable<Team> ControllingTeamNetVar { get; } = new NetworkVariable<Team>(Team.None);

    private BaseObjectiveType ObjectiveType { get; set; }

    private NetworkVariable<bool> IsBlueContestingNetVar = new NetworkVariable<bool>();

    private NetworkVariable<bool> IsOrangeContestingNetVar = new NetworkVariable<bool>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _TeamManager = FindObjectsByType<TeamManager>(FindObjectsSortMode.None).First();

        if (IsServer)
        {
            PrepTimeNetVar.Value = PrepTimeSeconds;
            CaptureTimerNetVar.Value = CaptureTimeSeconds;
        }
        if (IsClient)
        {
            _MeshRenderer = GetComponent<MeshRenderer>();
            _MeshRenderer.enabled = false;

            // Used for rendering in the inspector
            ControllingTeamNetVar.OnValueChanged = (_, next) => ControllingTeam = next;
            PrepTimeNetVar.OnValueChanged = (_, next) => PrepTimer = next;
            CaptureTimerNetVar.OnValueChanged = (_, next) => CaptureTimer = next;

            PrepTimer = PrepTimeSeconds;
        }
    }

    private void Update()
    {
        if (!IsServer || !IsSpawned || ObjectiveType == null)
        {
            return;
        }

        if (PrepTimeNetVar.Value > 0)
        {
            PrepTimeNetVar.Value -= Time.deltaTime;
            PrepTimeNetVar.Value = Mathf.Clamp(PrepTimeNetVar.Value, 0, PrepTimeSeconds);

            if (PrepTimeNetVar.Value <= 0)
            {
                ObjectiveType.OnStart();
                ShowObjectiveClientRpc();
            }

            return;
        }

        IsBlueContestingNetVar.Value = ContestingTanks.Any(t => _TeamManager.GetTeam(t) == Team.Blue);
        IsOrangeContestingNetVar.Value = ContestingTanks.Any(t => _TeamManager.GetTeam(t) == Team.Orange);

        if (IsBlueContestingNetVar.Value && IsOrangeContestingNetVar.Value)
        {
            // ...
        }
        else if (!IsBlueContestingNetVar.Value && !IsOrangeContestingNetVar.Value)
        {
            if (CaptureTimerNetVar.Value < CaptureTimeSeconds)
            {
                CaptureTimerNetVar.Value += Time.deltaTime;
                CaptureTimerNetVar.Value = Mathf.Clamp(CaptureTimerNetVar.Value, 0, CaptureTimeSeconds);
            }

            if (CaptureTimerNetVar.Value == 0)
            {
                ControllingTeamNetVar.Value = Team.None;
            }
        }
        else if (IsBlueContestingNetVar.Value)
        {
            if (ControllingTeamNetVar.Value == Team.Blue)
            {
                if (CaptureTimerNetVar.Value > 0)
                {
                    CaptureTimerNetVar.Value -= Time.deltaTime;
                    CaptureTimerNetVar.Value = Mathf.Clamp(CaptureTimerNetVar.Value, 0, CaptureTimeSeconds);
                }
            }
            else
            {
                if (CaptureTimerNetVar.Value < CaptureTimeSeconds)
                {
                    CaptureTimerNetVar.Value += Time.deltaTime;
                    CaptureTimerNetVar.Value = Mathf.Clamp(CaptureTimerNetVar.Value, 0, CaptureTimeSeconds);
                }

                if (CaptureTimerNetVar.Value == CaptureTimeSeconds)
                {
                    ControllingTeamNetVar.Value = Team.Blue;
                }
            }
        }
        else if (IsOrangeContestingNetVar.Value)
        {
            if (ControllingTeamNetVar.Value == Team.Orange)
            {
                if (CaptureTimerNetVar.Value > 0)
                {
                    CaptureTimerNetVar.Value -= Time.deltaTime;
                    CaptureTimerNetVar.Value = Mathf.Clamp(CaptureTimerNetVar.Value, 0, CaptureTimeSeconds);
                }
                else
                {
                    NetworkLog.LogInfoServer("Ok");
                }
            }
            else
            {
                if (CaptureTimerNetVar.Value < CaptureTimeSeconds)
                {
                    CaptureTimerNetVar.Value += Time.deltaTime;
                    CaptureTimerNetVar.Value = Mathf.Clamp(CaptureTimerNetVar.Value, 0, CaptureTimeSeconds);
                }

                if (CaptureTimerNetVar.Value == CaptureTimeSeconds)
                {
                    ControllingTeamNetVar.Value = Team.Orange;
                }
            }
        }

        if (CaptureTimerNetVar.Value <= 0)
        {
            ObjectiveCapturedServerEvent?.Invoke(ControllingTeamNetVar.Value);

            var winnerClientIds = IsBlueContestingNetVar.Value
                ? ContestingTanks.Where(t => _TeamManager.GetTeam(t) == Team.Blue)
                : ContestingTanks.Where(t => _TeamManager.GetTeam(t) == Team.Orange);

            // Reset timers. Cannot do in PrepObjective because it can be called before this Objective is Spawned
            CaptureTimerNetVar.Value = CaptureTimeSeconds;
            PrepTimeNetVar.Value = PrepTimeSeconds;

            _MeshRenderer.enabled = false;
            ObjectiveType.OnCapture(winnerClientIds);
            ObjectiveType = null;
            ObjectiveCapturedClientRpc();

            IsBlueContestingNetVar.Value = false;
            IsOrangeContestingNetVar.Value = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Tank>(out var _))
        {
            return;
        }

        var netObj = other.GetComponent<NetworkObject>();
        ContestingTanks.Add(netObj.OwnerClientId);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<Tank>(out var _))
        {
            return;
        }

        var netObj = other.GetComponent<NetworkObject>();
        ContestingTanks.Remove(netObj.OwnerClientId);
    }

    /// <summary>
    /// "Preps" the objective (becomes visible on the map).
    /// After the configured "TimeToStartSeconds", the objective will "Start" and become available for capture.
    /// </summary>
    public void PrepObjective(BaseObjectiveType objectiveType)
    {
        PrepTimeNetVar.Value = PrepTimeSeconds;
        CaptureTimerNetVar.Value = CaptureTimeSeconds;
        ObjectiveType = objectiveType;
        ShowObjectiveClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ShowObjectiveClientRpc()
    {
        _MeshRenderer.enabled = true;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ObjectiveCapturedClientRpc()
    {
        _MeshRenderer.enabled = false;
        ObjectiveCapturedClientEvent?.Invoke();
    }
}