using System;
using UnityEngine;

namespace TankAgents
{
    public class RandomActionTankAgent : BaseTankAgent
    {
        [SerializeField] public int seed = 0;
        
        [SerializeField] public int fireIntervalSeconds = 5;

        [SerializeField]
        [Tooltip("The chance that the turret rotation direction will change each frame")]
        public float changeTurretRotationDirectionChance = 0.05f;
        
        [SerializeField]
        [Tooltip("The chance that the left and/or right track will change roll direction")]
        public float changeTrackRollDirectionChance = 0.05f;
                
        private System.Random _random;
    
        private float _fireTimer = 0f;

        private float _turretRotationDirection = 0f;
        
        private float _leftTrackRollDirection = 0f;
        
        private float _rightTrackRollDirection = 0f;
        
        protected override void Awake()
        {
            _fireTimer = fireIntervalSeconds;
            _random = new System.Random(seed);
        }

        protected void Update()
        {
            _fireTimer -= Time.deltaTime;

            if (_random.NextDouble() < changeTurretRotationDirectionChance)
            {
                _turretRotationDirection = (float) (System.Math.Floor(_random.NextDouble() * 3) - 1);
            }

            if (_random.NextDouble() < changeTrackRollDirectionChance)
            {
                _leftTrackRollDirection = (float) (System.Math.Floor(_random.NextDouble() * 3) - 1);
            }
            
            if (_random.NextDouble() < changeTrackRollDirectionChance)
            {
                _rightTrackRollDirection = (float) (System.Math.Floor(_random.NextDouble() * 3) - 1);
            }
        }

        public override bool GetDecisionFire()
        {
            if (_fireTimer <= 0f)
            {
                _fireTimer = fireIntervalSeconds;
                return true;
            }

            return false;
        }
    
        public override bool GetDecisionReload()
        {
            return false;
        }
    
        public override Vector3 GetDecisionRotateTurret()
        {
            Debug.LogError("Not implemented");
            throw new NotImplementedException("Not implemented");
        }

        public override (float, float) GetDecisionRollTracks()
        {
            return (_leftTrackRollDirection, _rightTrackRollDirection);
        }
    }
}