using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthResourceMeterController : MonoBehaviour
{
    private Slider _healthBarSlider;

    private Tank _tank;

    private void Start()
    {
        _tank = NetcodeHelper.GetLocalClientTankOrNull();
        _healthBarSlider = GetComponent<Slider>();
        _healthBarSlider.maxValue = _tank != null ? _tank.HealthCapacity : 100f;
    }

    private void Update()
    {
        _healthBarSlider.value = _tank != null ? _tank.Health : Time.timeSinceLevelLoad;
    }
}