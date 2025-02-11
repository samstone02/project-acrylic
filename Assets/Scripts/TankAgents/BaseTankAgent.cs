using TankGuns;
using UnityEngine;

namespace TankAgents
{
    public abstract class BaseTankAgent : MonoBehaviour
    {
        protected Tank Tank { get; private set; }
    
        protected GameObject Turret { get; private set; }

        protected BaseCannon Gun { get; private set; }
    
        protected virtual void Awake()
        {
            Tank = GetComponentInParent<Tank>();
            Turret = Tank.transform.Find("Turret").gameObject;
            Gun = Turret.transform.GetComponentInChildren<BaseCannon>();
        }
    
        public abstract bool GetDecisionFire();
    
        public abstract bool GetDecisionReload();
    
        /// <returns>The direction the agent wants the turret to face as a Vector3.</returns>
        public abstract Vector3 GetDecisionRotateTurret();
    
        public abstract (float, float) GetDecisionRollTracks();
    }
}