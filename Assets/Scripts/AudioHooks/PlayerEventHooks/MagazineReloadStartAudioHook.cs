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
            var playerTank = GameObject.Find("PlayerTank");
            var autoloadingTankGun = playerTank.GetComponentInChildren<AutoloadingTankGun>();
            autoloadingTankGun.ReloadStartEvent += () => AudioSource.Play();
        }
    }
}