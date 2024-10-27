using UnityEngine;

public abstract class BaseTankAgent : MonoBehaviour
{
    protected GameObject Turret;
    
    protected void Start()
    {
        Turret = transform.Find("Turret").gameObject;
    }
    
    public abstract bool GetDecisionFire();
    
    public abstract bool GetDecisionReload();
    
    public abstract float GetDecisionRotateTurret();
    
    public abstract (float, float) GetDecisionRollTracks();
}
