using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceMeter : MonoBehaviour
{
    private void Start()
    {
        var tank = NetcodeHelper.GetLocalClientTankOrNull();

        if (tank == null)
        {
            return;
        }

        var slider = GetComponent<Slider>();
        var currentValue = transform.Find("Numerics").Find("CurrentValue").GetComponent<TextMeshProUGUI>();
        var maxValue = transform.Find("Numerics").Find("MaxValue").GetComponent<TextMeshProUGUI>();

        maxValue.text = (tank != null ? tank.HealthCapacity : 100f).ToString();
        slider.onValueChanged.AddListener((newValue) => currentValue.text = newValue.ToString());
    }
}
