using Unity.Netcode;
using UnityEngine;

public class TankMotionAudioController : MonoBehaviour
{
    [SerializeField] public AudioSource RunningEngine;
    [SerializeField] public AudioSource IdleEngine;
    [SerializeField] public AudioSource RollingTracks;
    [SerializeField] public float FastEngineThreshold;

    private Tank _tank;
    private TankAgents.BaseTankAgent _agent;

    private void Start()
    {
        _tank = GetComponentInParent<Tank>();
        _agent = _tank.Agent;
        _tank.DeployClientEvent += StartEngine;
    }

    private void Update()
    {
        if (_agent == null)
        {
            return;
        }

        var (leftRoll, rightRoll) = _agent.GetDecisionRollTracks();

        BroadcastTankMotionClientRpc(leftRoll, rightRoll);

        PlaySounds(leftRoll, rightRoll);
    }

    public void StartEngine()
    {
        IdleEngine.Play();
        RunningEngine.Play();
        RollingTracks.Play();

        RunningEngine.Pause();
        RollingTracks.Pause();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void BroadcastTankMotionClientRpc(float leftRoll, float rightRoll)
    {
        PlaySounds(leftRoll, rightRoll);
    }

    private void PlaySounds(float leftRoll, float rightRoll)
    {
        if (leftRoll > FastEngineThreshold || rightRoll > FastEngineThreshold)
        {
            RunningEngine.UnPause();
            RollingTracks.UnPause();

            IdleEngine.Pause();
        }
        else
        {
            RunningEngine.Pause();
            RollingTracks.Pause();

            IdleEngine.UnPause();
        }
    }
}
