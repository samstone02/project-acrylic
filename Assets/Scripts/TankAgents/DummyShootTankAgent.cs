using TankAgents;
using UnityEngine;

namespace TankAgents
{
    public class DummyShootTankAgent : BaseTankAgent
    {
        [SerializeField] public int fireIntervalSeconds = 5;
    
        private float _fireTimer = 0f;
    
        protected void Update()
        {
            _fireTimer -= Time.deltaTime;
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

        public override bool GetDecisionReload() => false;

        public override float GetDecisionRotateTurret() => 0f;
    
        public override (float, float) GetDecisionRollTracks() => (0f, 0f);
    }   
}
