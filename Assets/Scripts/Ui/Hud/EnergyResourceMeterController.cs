using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EnergyResourceMeterController : MonoBehaviour
{
    private Slider _energyBarSlider;

    private TankAbilityController _abilityController;

    private void Start()
    {
        _abilityController = GetTankAbilityControllerOrNull();
        _energyBarSlider = GetComponent<Slider>();
        _energyBarSlider.maxValue = _abilityController != null ? _abilityController.MaxEnergy : 100f;
    }

    private void Update()
    {
        _energyBarSlider.value = _abilityController != null ? _abilityController.CurrentEnergy: Time.timeSinceLevelLoad;
    }

    private TankAbilityController GetTankAbilityControllerOrNull()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening && NetworkManager.Singleton.IsClient)
        {
            return NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<TankAbilityController>();
        }

        return null;
    }
}