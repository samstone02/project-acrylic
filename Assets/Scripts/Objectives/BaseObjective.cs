using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class BaseObjective : NetworkBehaviour
{
    [field: SerializeField] public float TimeToStartSeconds {  get; set; }

    [field: SerializeField] public float CaptureTimeSeconds { get; set; }

    public Team ControllingTeam { get; private set; }

    public float ObjectiveStartTimer { get; private set; }

    public float CaptureTimer { get; private set; }

    public event Action<Team> ObjectiveCapturedEvent;

    private TeamManager _TeamManager { get; set; }

    private NetworkVariable<float> ObjectiveStartTimerNetVar { get; } = new NetworkVariable<float>();

    private NetworkVariable<float> CaptureTimerNetVar { get; } = new NetworkVariable<float>();

    private List<ulong> ContestingTanks { get; } = new List<ulong>();

    private NetworkVariable<Team> ControllingTeamNetVar { get; } = new NetworkVariable<Team>(Team.None);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _TeamManager = FindObjectsByType<TeamManager>(FindObjectsSortMode.None).First();

        if (IsServer)
        {
            ObjectiveStartTimerNetVar.Value = TimeToStartSeconds;
            CaptureTimerNetVar.Value = CaptureTimeSeconds;
        }
        if (IsClient)
        {
            ControllingTeamNetVar.OnValueChanged = (_, next) => ControllingTeam = next;
            ObjectiveStartTimerNetVar.OnValueChanged = (_, next) => ObjectiveStartTimer = next;
            CaptureTimerNetVar.OnValueChanged = (_, next) => CaptureTimer = next;
        }
    }

    private void Update()
    {
        if (!IsServer || !IsSpawned)
        {
            return;
        }

        if (ObjectiveStartTimerNetVar.Value > 0)
        {
            ObjectiveStartTimerNetVar.Value -= Time.deltaTime;
            ObjectiveStartTimerNetVar.Value = Mathf.Clamp(ObjectiveStartTimerNetVar.Value, 0, TimeToStartSeconds);

            if (ObjectiveStartTimerNetVar.Value <= 0)
            {
                StartObjective();
            }

            return;
        }

        var isBlueContesting = ContestingTanks.Any(t => _TeamManager.GetTeam(t) == Team.Blue);
        var isOrangeContesting = ContestingTanks.Any(t => _TeamManager.GetTeam(t) == Team.Orange);

        if (isBlueContesting && isOrangeContesting)
        {
            // ...
        }
        else if (!isBlueContesting && !isOrangeContesting)
        {
            ControllingTeamNetVar.Value = Team.None;

            if (CaptureTimerNetVar.Value < CaptureTimeSeconds)
            {
                CaptureTimerNetVar.Value += Time.deltaTime;
                CaptureTimerNetVar.Value = Mathf.Clamp(CaptureTimerNetVar.Value, 0, CaptureTimeSeconds);
            }
        }
        else if (isBlueContesting)
        {
            if (ControllingTeamNetVar.Value == Team.Blue)
            {
                if (CaptureTimerNetVar.Value > 0)
                {
                    CaptureTimerNetVar.Value -= Time.deltaTime;
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
        else if (isOrangeContesting)
        {
            if (ControllingTeamNetVar.Value == Team.Orange)
            {
                if (CaptureTimerNetVar.Value > 0)
                {
                    CaptureTimerNetVar.Value -= Time.deltaTime;
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
            ObjectiveCapturedEvent?.Invoke(ControllingTeamNetVar.Value);
            var winnerClientIds = isBlueContesting
                ? ContestingTanks.Where(t => _TeamManager.GetTeam(t) == Team.Blue)
                : ContestingTanks.Where(t => _TeamManager.GetTeam(t) == Team.Orange);
            OnCapture(winnerClientIds);
            Destroy(this.gameObject);
        }
    }

    protected abstract void StartObjective();

    protected virtual void OnCapture(IEnumerable<ulong> teamMemberClientIds)
    {
        NetworkLog.LogInfoServer($"Objective captured by {ControllingTeamNetVar}!");
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
}