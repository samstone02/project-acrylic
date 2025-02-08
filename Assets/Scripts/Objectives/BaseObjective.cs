using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class BaseObjective : MonoBehaviour
{
    [field: SerializeField] public float TimeToStartSeconds {  get; set; }

    [field: SerializeField] public float CaptureTimeSeconds { get; set; }

    [field: SerializeField] private TeamManager _TeamManager { get; set; }

    public event Action<string> ObjectiveCapturedEvent;

    private float _objectiveStartTimer { get; set; }

    private float _captureTimer { get; set; }

    private List<ulong> ContestingTanks { get; set; } = new List<ulong>();

    public string ControllingTeam { get; private set; }

    private void Start()
    {
        _objectiveStartTimer = TimeToStartSeconds;
        _captureTimer = CaptureTimeSeconds;
        ControllingTeam = TeamManager.NO_TEAM;
    }

    private void Update()
    {
        if (_objectiveStartTimer > 0)
        {
            _objectiveStartTimer -= Time.deltaTime;
            _objectiveStartTimer = Mathf.Clamp(_objectiveStartTimer, 0, TimeToStartSeconds);

            if (_objectiveStartTimer <= 0)
            {
                StartObjective();
            }

            return;
        }

        var isBlueContesting = ContestingTanks.Any(t => _TeamManager.GetTeamName(t) == TeamManager.TEAM_BLUE);
        var isOrangeContesting = ContestingTanks.Any(t => _TeamManager.GetTeamName(t) == TeamManager.TEAM_ORANGE);

        if (isBlueContesting && isOrangeContesting)
        {
            // ...
        }
        else if (!isBlueContesting && !isOrangeContesting)
        {
            ControllingTeam = TeamManager.NO_TEAM;

            if (_captureTimer < CaptureTimeSeconds)
            {
                _captureTimer += Time.deltaTime;
                _captureTimer = Mathf.Clamp(_captureTimer, 0, CaptureTimeSeconds);
            }
        }
        else if (isBlueContesting)
        {
            if (ControllingTeam == TeamManager.TEAM_BLUE)
            {
                if (_captureTimer > 0)
                {
                    _captureTimer -= Time.deltaTime;
                }
            }
            else
            {
                if (_captureTimer < CaptureTimeSeconds)
                {
                    _captureTimer += Time.deltaTime;
                    _captureTimer = Mathf.Clamp(_captureTimer, 0, CaptureTimeSeconds);
                }

                if (_captureTimer == CaptureTimeSeconds)
                {
                    ControllingTeam = TeamManager.TEAM_BLUE;
                }
            }
        }
        else if (isOrangeContesting)
        {
            if (ControllingTeam == TeamManager.TEAM_ORANGE)
            {
                if (_captureTimer > 0)
                {
                    _captureTimer -= Time.deltaTime;
                }
            }
            else
            {
                if (_captureTimer < CaptureTimeSeconds)
                {
                    _captureTimer += Time.deltaTime;
                    _captureTimer = Mathf.Clamp(_captureTimer, 0, CaptureTimeSeconds);
                }

                if (_captureTimer == CaptureTimeSeconds)
                {
                    ControllingTeam = TeamManager.TEAM_ORANGE;
                }
            }
        }

        if (_captureTimer <= 0)
        {
            ObjectiveCapturedEvent?.Invoke(ControllingTeam);
            var winnerClientIds = isBlueContesting
                ? ContestingTanks.Where(t => _TeamManager.GetTeamName(t) == TeamManager.TEAM_BLUE)
                : ContestingTanks.Where(t => _TeamManager.GetTeamName(t) == TeamManager.TEAM_ORANGE);
            OnCapture(winnerClientIds);
            Destroy(this.gameObject);
        }
    }

    protected abstract void StartObjective();

    protected abstract void OnCapture(IEnumerable<ulong> teamMemberClientIds);

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