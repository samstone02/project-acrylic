using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomAudioSource : MonoBehaviour
{
    [field: SerializeField] public List<AudioClip> AudioClips { get; private set; }
    [Range(0.0f, 1.0f)] public float PitchMin;
    [Range(0.0f, 1.0f)] public float PitchMax;
    [Range(0.0f, 1.0f)] public float VolumeMin;
    [Range(0.0f, 1.0f)] public float VolumeMax;

    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        var idx = Random.Range(0, AudioClips.Count);
        var clip = AudioClips[idx];

        _audioSource.clip = clip;
        _audioSource.pitch = Random.Range(PitchMin, PitchMax);
        _audioSource.volume = Random.Range(VolumeMin, VolumeMax);
        _audioSource.Play();
    }
}
