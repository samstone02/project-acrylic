using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

namespace TankAgents
{
    public class RushdownTankAgent : BaseTankAgent
    {
        [field: SerializeField] public float MinAngleDifferenceAim { get; set; } = 1f;
        
        [field: SerializeField] public float MinAngleDifferenceRollForward { get; set; } = 25f;
        
        private GameObject _playerTank;
        
        private LineRenderer _lineRenderer;

        protected void Start()
        {
            _playerTank = GameObject.Find("PlayerTank");
            _lineRenderer = GetComponent<LineRenderer>();
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
            
            Vector3 immediateDestination = Vector3.zero;
            
            if (path.corners.Length > 1)
            {
                _lineRenderer.positionCount = path.corners.Length;
                _lineRenderer.SetPositions(path.corners);
                immediateDestination = path.corners[1] - Tank.transform.position;
            }
            
            float angleDifference = Vector3.Angle(immediateDestination, Tank.transform.forward);
            
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