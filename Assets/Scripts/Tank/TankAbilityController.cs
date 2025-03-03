using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Tank))]
[RequireComponent(typeof(TankAbilityInputMap))]
public class TankAbilityController : NetworkBehaviour
{
    [field: SerializeField] public float Energy { get; set; }
    [field: SerializeField] public BaseTankAbility Ability0 { get; private set; }

    private Tank _tank;
    private TankAbilityInputMap _inputMap;

    private void Start()
    {
        _tank = GetComponent<Tank>();
        _inputMap = GetComponent<TankAbilityInputMap>();
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (_inputMap.Ability1)
        {
            if (Energy >= Ability0.EnergyCost)
            {
                TriggerAbilityServerRpc(0);
            }
            else
            {
                //...
            }
        }
    }

    [Rpc(SendTo.Server)]
    private void TriggerAbilityServerRpc(byte abilityNumber)
    {
        var tank = GetComponentInParent<Tank>();
        switch (abilityNumber)
        {
            case 0:
                Ability0.OnTrigger(tank);
                break;
            case 1:
                Ability0.OnTrigger(tank);
                break;
             case 2:
                Ability0.OnTrigger(tank);
                break;
            default:
                NetworkLog.LogInfoServer("An invalid ability number was provided: " +  abilityNumber);
                break;
        }
    }
}