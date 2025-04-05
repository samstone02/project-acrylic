using UnityEngine;

public class TankMotionAudioController : MonoBehaviour
{
    [SerializeField] public AudioSource RunningEngine;
    [SerializeField] public AudioSource IdleEngine;
    [SerializeField] public AudioSource RollingTracks;

    [SerializeField] public float FastEngineThreshold; 

    private Tank _tank;

    private void Start()
    {
        _tank = GetComponentInParent<Tank>();
        _tank.DeployClientEvent += StartEngine;
    }

    private void Update()
    {
        var rb = _tank.GetComponent<Rigidbody>();
        var lv = rb.linearVelocity;

        if (rb.linearVelocity.magnitude > 0.1)
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
