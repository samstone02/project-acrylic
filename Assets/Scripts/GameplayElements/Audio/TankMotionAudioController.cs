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
        var (leftRoll, rightRoll) = _agent.GetDecisionRollTracks();

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

    public void StartEngine()
    {
        IdleEngine.Play();
        RunningEngine.Play();
        RollingTracks.Play();

        RunningEngine.Pause();
        RollingTracks.Pause();
    }
}
