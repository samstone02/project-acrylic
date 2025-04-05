using System;
using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class TurretRotationAudioController : MonoBehaviour
{
    private AudioSource _audioSource;
    private Tank _tank;
    private Transform _turret;
    private Quaternion _previousRotation;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _tank = GetComponentInParent<Tank>();
        _turret = _tank.transform.Find("Turret");

        _audioSource.Play();
        _audioSource.Pause();
    }

    void Update()
    {
        if (Math.Abs((_previousRotation.eulerAngles - _turret.localRotation.eulerAngles).magnitude) > 0.05)
        {
            _audioSource.UnPause();
        }
        else
        {
            _audioSource.Pause();
        }

        _previousRotation = _turret.localRotation;
    }
}
