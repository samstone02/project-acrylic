using TankGuns;
using UnityEngine;

namespace AudioHooks.PlayerEventHooks
{
    [RequireComponent(typeof(AudioSource))]
    public class MagazineReloadEndAudioHook : MonoBehaviour
    {
        private AudioSource AudioSource { get; set; }
        
        void Start()
        {
            AudioSource = GetComponent<AudioSource>();
            var playerTank = GameObject.Find("PlayerTank");
            var autoloadingTankGun = playerTank.GetComponentInChildren<AutoLoadingCannon>();
            autoloadingTankGun.ReloadEndEvent += () => AudioSource.Play();
        }
    }
}