using UnityEngine;

public abstract class BaseTankAbility : MonoBehaviour
{
    [field: SerializeField] public float EnergyCost { get; private set; }
    public abstract void OnTrigger(Tank tank);
}