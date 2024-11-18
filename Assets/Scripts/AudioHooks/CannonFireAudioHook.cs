using TankGuns;
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
            var cannon = GetComponentInParent<Tank>().GetComponentInChildren<BaseCannon>();
            cannon.FireEvent += () => AudioSource.Play();
        }
    }
}
