using TankGuns;
using UnityEngine;

namespace AudioHooks.PlayerEventHooks
{
    [RequireComponent(typeof(AudioSource))]
    public class MagazineReloadStartAudioHook : MonoBehaviour
    {
        private AudioSource AudioSource { get; set; }
        
        void Start()
        {
            AudioSource = GetComponent<AudioSource>();
            var playerTank = GetComponentInParent<Tank>();
            var autoloadingTankGun = playerTank.GetComponentInChildren<AutoLoadingCannon>();
            autoloadingTankGun.ReloadStartEvent += () => AudioSource.Play();
        }
    }
}