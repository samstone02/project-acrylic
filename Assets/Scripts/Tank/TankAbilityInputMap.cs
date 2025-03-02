using UnityEngine;
using UnityEngine.InputSystem;

public class TankAbilityInputMap : MonoBehaviour
{
    [field: SerializeField] public InputActionReference Ability1ActionReference { get; private set; }

    public bool Ability1 { get; private set; }

    private void OnEnable()
    {
        Ability1ActionReference.action.Enable();
    }

    private void OnDisable()
    {
        Ability1ActionReference.action.Disable();
    }

    private void Update()
    {
        Ability1 = Ability1ActionReference.action.triggered;
    }
}