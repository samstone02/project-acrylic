using System;
using UnityEngine;
using UnityEngine.AI;

namespace TankAgents
{
    public class RushdownTankAgent : BaseTankAgent
    {
        [field: SerializeField] public float MinAngleDifferenceAim { get; set; } = 1f;
        
        [field: SerializeField] public float MinAngleDifferenceRollForward { get; set; } = 25f;
        
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
            
            return angleDifference <= MinAngleDifferenceAim;
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
            var path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, _playerTank.transform.position, NavMesh.AllAreas, path);
            Vector3 immediateDestination = path.corners[1] - Tank.transform.position;
            float angleDifference = Vector3.Angle(Tank.transform.forward, immediateDestination);
            
            if (angleDifference < MinAngleDifferenceRollForward)
            {
                return (1f, 1f);
            }
            else if (angleDifference <= 0)
            {
                return (-1f, 1f);
            }
            else
            {
                return (1f, -1f);
            }
        }
    }
}