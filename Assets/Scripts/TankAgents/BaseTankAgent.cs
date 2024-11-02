using UnityEngine;

namespace TankAgents
{
    public abstract class BaseTankAgent : MonoBehaviour
    {
        protected Tank Tank;
    
        protected GameObject Turret;
    
        protected void Awake()
        {
            Tank = GetComponent<Tank>();
            Turret = transform.Find("Turret").gameObject;
        }
    
        public abstract bool GetDecisionFire();
    
        public abstract bool GetDecisionReload();
    
        public abstract float GetDecisionRotateTurret();
    
        public abstract (float, float) GetDecisionRollTracks();

        /// <summary>
        /// Returns the rotation direction as a percentage of the rotation speed.
        /// Positive values are clockwise, negative values are counterclockwise.
        /// </summary>
        protected static float CalculateTurretRotationDirection(
            Vector3 targetDirection,
            Vector3 currentDirection,
            float rotationSpeed)
        {
            Vector3 turretTargetCross = Vector3.Cross(currentDirection, targetDirection);

            int direction = turretTargetCross.y > 0 ? 1 : -1;
            float angleDifference = Vector3.Angle(currentDirection, targetDirection);
            float angleRotation = rotationSpeed * Time.deltaTime;

            if (angleDifference < angleRotation)
            {
                return angleDifference / angleRotation * direction;
            }
        
            return direction;
        }
    }
}