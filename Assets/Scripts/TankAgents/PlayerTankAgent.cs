using System;
using System.Collections.Generic;
using TankGuns;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace TankAgents
{
    public class PlayerTankAgent : BaseTankAgent
    {
        [SerializeField] public InputActionReference fireInput;
        
        [SerializeField] public InputActionReference reloadInput;

        [SerializeField] public InputActionReference leftTrackRollInput;

        [SerializeField] public InputActionReference rightTrackRollInput;
        
        [field: SerializeField] public InputActionReference LoadStandardAmmoInput { get; set; }
        
        [field: SerializeField] public InputActionReference LoadExplosiveAmmoInput { get; set; }
        
        [field: SerializeField] public InputActionReference LoadRicochetAmmoInput { get; set; }
        
        [field: SerializeField] public InputActionReference ScrollAmmoInput { get; set; }
        
        [field: SerializeField] public GameObject StandardAmmoPrefab { get; set; }
        
        [field: SerializeField] public GameObject ExplosiveAmmoPrefab { get; set; }
        
        [field: SerializeField] public GameObject RicochetAmmoPrefab { get; set; }

        [field: SerializeField] public List<GameObject> AvailableShellTypes { get; set; }= new List<GameObject>();

        [field: SerializeField] public int ShellScrollDeadZone { get; set; } = 0;
        
        public event Action SelectStandardAmmoEvent;

        public event Action SelectExplosiveAmmoEvent;

        public event Action SelectRicochetAmmoEvent;
        
        public event Action<string> ChangeSelectedShellEvent;

        private LayerMask _playerAimMask;

        private int _currentSelectedAmmo = 0;

        private void OnEnable()
        {
            fireInput.action.Enable();
            reloadInput.action.Enable();
            leftTrackRollInput.action.Enable();
            rightTrackRollInput.action.Enable();
            LoadStandardAmmoInput.action.Enable();
            LoadExplosiveAmmoInput.action.Enable();
            LoadRicochetAmmoInput.action.Enable();
            ScrollAmmoInput.action.Enable();
        }

        private void OnDisable()
        {
            fireInput.action.Disable();
            reloadInput.action.Disable();
            leftTrackRollInput.action.Disable();
            rightTrackRollInput.action.Disable();
            LoadStandardAmmoInput.action.Disable();
            LoadExplosiveAmmoInput.action.Disable();
            LoadRicochetAmmoInput.action.Disable();
            ScrollAmmoInput.action.Disable();
        }

        protected void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.EndVertical();
        }

        protected void Start()
        {
            if (Gun.GetType() != typeof(AutoLoadingCannon))
            {
                Debug.LogError("Expected player tank to have an autoloading gun.");
            }
            
            if (AvailableShellTypes.Count == 0)
            {
                Debug.LogError("Expected to have at least one available ammo type.");
            }

            _playerAimMask = LayerMask.GetMask("Player Aim");
        }

        protected void Update()
        {
            int mouseScrollDelta = (int) ScrollAmmoInput.action.ReadValue<float>();
            Debug.Log($"Mouse scroll delta: {mouseScrollDelta}");
            if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows)
            {
                mouseScrollDelta /= 120;
            }

            int direction = Math.Abs(mouseScrollDelta) > ShellScrollDeadZone ? 1 : 0;
            direction *= mouseScrollDelta > 0 ? 1 : -1;
            
            int newIndex = Mathf.Clamp(_currentSelectedAmmo + direction, 0, AvailableShellTypes.Count - 1);

            if (newIndex != _currentSelectedAmmo)
            {
                _currentSelectedAmmo = newIndex;
                Gun.ProjectilePrefab = AvailableShellTypes[_currentSelectedAmmo];
                ChangeSelectedShellEvent?.Invoke(Gun.ProjectilePrefab.name);
            }
            
            if (LoadStandardAmmoInput.action.triggered)
            {
                Gun.ProjectilePrefab = StandardAmmoPrefab;
                SelectStandardAmmoEvent?.Invoke();
            }
            else if (LoadExplosiveAmmoInput.action.triggered)
            {
                Gun.ProjectilePrefab = ExplosiveAmmoPrefab;
                SelectExplosiveAmmoEvent?.Invoke();
            }
            else if (LoadRicochetAmmoInput.action.triggered)
            {
                Gun.ProjectilePrefab = RicochetAmmoPrefab;
                SelectRicochetAmmoEvent?.Invoke();
            }
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
            Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
            Physics.Raycast(mousePosWorld, Camera.main.transform.forward, out RaycastHit hit, 100, _playerAimMask);
            
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
