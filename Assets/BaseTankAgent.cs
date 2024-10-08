using UnityEngine;

public abstract class BaseTankAgent : MonoBehaviour
{
    public abstract bool GetDecisionShoot();
    
    public abstract float GetDecisionRotateTurret();
    
    public abstract (float, float) GetDecisionMoveTreads();
}
