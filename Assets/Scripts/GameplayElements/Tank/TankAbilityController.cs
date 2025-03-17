using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Tank))]
[RequireComponent(typeof(TankAbilityInputMap))]
public class TankAbilityController : NetworkBehaviour
{
    [field: SerializeField] public float MaxEnergy { get; private set; }
    [field: SerializeField] public BaseTankAbility Ability0 { get; private set; }
    public float CurrentEnergy { get; private set; }

    private Tank _tank;
    private TankAbilityInputMap _inputMap;

    private void Awake()
    {
        CurrentEnergy = MaxEnergy;
    }

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
            if (MaxEnergy >= Ability0.EnergyCost)
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
        BaseTankAbility triggeredAbility = null;

        switch (abilityNumber)
        {
            case 0:
                triggeredAbility = Ability0;
                break;
            case 1:
                triggeredAbility = Ability0;
                break;
             case 2:
                triggeredAbility = Ability0;
                break;
            default:
                NetworkLog.LogInfoServer("An invalid ability number was provided: " +  abilityNumber);
                break;
        }

        if (CurrentEnergy >= Ability0.EnergyCost)
        {
            triggeredAbility.OnTrigger(_tank);
            CurrentEnergy -= triggeredAbility.EnergyCost;
        }
    }
}