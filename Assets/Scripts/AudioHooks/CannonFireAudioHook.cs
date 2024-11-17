using UnityEngine;

namespace AudioHooks
{
    [RequireComponent(typeof(AudioSource))]
    public class CannonFireAudioHook : MonoBehaviour
    {
        private AudioSource AudioSource { get; set; }

        void Start()
        {
            AudioSource = GetComponent<AudioSource>();
            var tank = transform.parent.parent.GetComponent<Tank>();
            tank.OnFire += () => AudioSource.Play();
        }
    }
}
