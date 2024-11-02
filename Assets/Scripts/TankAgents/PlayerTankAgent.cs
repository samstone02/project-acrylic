using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace TankAgents
{
    [RequireComponent(typeof(Tank))]
    public class PlayerTankAgent : BaseTankAgent
    {
        [SerializeField] public InputActionReference fireInput;
        
        [SerializeField] public InputActionReference reloadInput;

        [SerializeField] public InputActionReference leftTrackRollInput;

        [SerializeField] public InputActionReference rightTrackRollInput;
        
        private Camera _mainCamera;

        private MeshCollider _ground;

        private LayerMask _playerAimMask;

        private void OnEnable()
        {
            fireInput.action.Enable();
            reloadInput.action.Enable();
            leftTrackRollInput.action.Enable();
            rightTrackRollInput.action.Enable();
        }

        private void OnDisable()
        {
            fireInput.action.Disable();
            reloadInput.action.Disable();
            leftTrackRollInput.action.Disable();
            rightTrackRollInput.action.Disable();
        }
        
        protected void Start()
        {
            _mainCamera = Camera.main;
            _playerAimMask = LayerMask.GetMask("Player Aim");
        }

        public override bool GetDecisionFire()
        {
            return fireInput.action.triggered;
        }
        
        public override bool GetDecisionReload()
        {
            return reloadInput.action.triggered;
        }

        public override float GetDecisionRotateTurret()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 mousePosWorld = _mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
            Physics.Raycast(mousePosWorld, _mainCamera.transform.forward, out RaycastHit hit, 100, _playerAimMask);
            
            Vector3 targetDirection = hit.point - Turret.transform.position;
            targetDirection.y = 0;
            targetDirection.Normalize();
            Vector3 turretDirection = Turret.transform.forward;
            turretDirection.y = 0;
            turretDirection.Normalize();
            
            return CalculateTurretRotationDirection(targetDirection, turretDirection, Tank.TurretRotationSpeed);
        }

        public override (float, float) GetDecisionRollTracks()
        {
            float left = leftTrackRollInput.action.ReadValue<float>();
            float right = rightTrackRollInput.action.ReadValue<float>();
            return (left, right);
        }
    }   
}
