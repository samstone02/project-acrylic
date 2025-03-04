﻿using System;
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
            Debug.Log("Start");
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

        public override Vector3 GetDecisionRotateTurret()
        {
            Vector3 playerDirection = _playerTank.transform.position - transform.position;
            playerDirection.y = 0;
            playerDirection.Normalize();

            return playerDirection;
        }

        public override (float, float) GetDecisionRollTracks()
        {
            var path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, _playerTank.transform.position, NavMesh.AllAreas, path);

            // get the current and next target position
            Vector3 immediateNextTargetPos =  path.corners.Length > 1 ? path.corners[1] : transform.position;
            immediateNextTargetPos.y = 0;
            Vector3 currentPos = Tank.transform.position;
            currentPos.y = 0;
            
            // get the angle between current forward direction and target direction
            Vector3 immediateNextTargetDirection = immediateNextTargetPos - currentPos;
            immediateNextTargetDirection.Normalize();
            float angleDifference = Vector3.Angle(immediateNextTargetDirection, Tank.transform.forward);
            
            // get rotation direction between current forward direction and target direction
            Vector3 turretTargetCross = Vector3.Cross(transform.forward, immediateNextTargetDirection);
            int direction = turretTargetCross.y > 0 ? 1 : -1;

            if (_lineRenderer is { enabled: true } && path.corners.Length > 1)
            {
                _lineRenderer.positionCount = path.corners.Length;
                _lineRenderer.SetPositions(path.corners);
            }

            if (angleDifference < MinAngleDifferenceRollForward)
            {
                return (1f, 1f);
            }
            else if (angleDifference * direction <= 0)
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