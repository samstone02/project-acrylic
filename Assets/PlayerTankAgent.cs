using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerTankAgent : BaseTankAgent
{
    [field: SerializeField]
    public InputActionReference Shoot { get; set; }

    [field: SerializeField]
    public InputActionReference LeftTreadRoll { get; set; }

    [field: SerializeField]
    public InputActionReference RightTreadRoll { get; set; }

    public override bool GetDecisionShoot()
    {
        return Shoot.action.triggered;
    }

    public override float GetDecisionRotateTurret()
    {
        return 1.0f;
    }

    public override (float, float) GetDecisionMoveTreads()
    {
        float left =  LeftTreadRoll.action.ReadValue<float>();
        float right =  RightTreadRoll.action.ReadValue<float>();
        return (left, right);
    }
}
