using System;
using UnityEngine;

namespace TankAgents
{
    public class RushdownTankAgent : BaseTankAgent
    {
        [field: SerializeField] public float MinAngleDifference { get; set; } = 1f;
        
        private GameObject _playerTank;

        protected void Start()
        {
            _playerTank = GameObject.Find("PlayerTank");
        }

        public override bool GetDecisionFire()
        {
            Vector3 playerDirection = _playerTank.transform.position - transform.position;
            Vector3 turretDirection = Turret.transform.forward;
            
            float angleDifference = Math.Abs(Vector3.Angle(playerDirection, turretDirection));
            
            return angleDifference <= MinAngleDifference;
        }

        public override bool GetDecisionReload()
        {
            return false;
        }

        public override float GetDecisionRotateTurret()
        {
            Vector3 playerDirection = _playerTank.transform.position - transform.position;
            playerDirection.y = 0;
            playerDirection.Normalize();
            Vector3 turretDirection = Turret.transform.forward;
            turretDirection.y = 0;
            turretDirection.Normalize();
            return CalculateTurretRotationDirection(playerDirection, turretDirection, Tank.TurretRotationSpeed);
        }

        public override (float, float) GetDecisionRollTracks()
        {
            return (0f, 0f);
        }
    }
}