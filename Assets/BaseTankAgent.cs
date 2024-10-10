using UnityEngine;

public abstract class BaseTankAgent : MonoBehaviour
{
    protected GameObject _turret;
    
    protected void Start()
    {
        _turret = transform.Find("Turret").gameObject;
    }
    
    public abstract bool GetDecisionShoot();
    
    public abstract float GetDecisionRotateTurret();
    
    public abstract (float, float) GetDecisionMoveTreads();
}
