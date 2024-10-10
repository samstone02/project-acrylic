using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(BaseTankAgent))]
public class Tank : MonoBehaviour
{
    [field: SerializeField]
    public GameObject TankGunPrefab { get; set; }

    [SerializeField]
    public GameObject turret;
    
    [field: SerializeField]
    public Rigidbody Rigidbody { get; set; }

    private BaseTankAgent _agent;

    private BaseTankGun _tankGun;

    [SerializeField] public float treadTorque = 10f;
    
    private void Awake()
    {
        Debug.Log("Hello, Tanks!");
        
        _agent = GetComponent<BaseTankAgent>();
        
        GameObject gunInstance = Instantiate(TankGunPrefab, transform);
        _tankGun = TankGunPrefab.GetComponent<BaseTankGun>();
        
        turret = transform.Find("Turret").gameObject;
    }
    
    private void Update()
    {
        bool shootDecision = _agent.GetDecisionShoot();

        var x = _agent.GetDecisionRotateTurret();
        
        if (shootDecision)
        {
            Debug.Log("Player Shot");
            var projectile = _tankGun.Shoot();
        }
        
        (float left, float right) = _agent.GetDecisionMoveTreads();
        
        Move(left, right);
    }

    private void Move(float left, float right)
    {
        // Currently a physics based movement system.
        // I'm not sure if I want to keep this because it's hard to get it feeling right.
        // I'll leave it for now.
        // It feels heavy and slow. Even though it's a tank, I don't want this to be the case.
        // There's basically no acceleration. It's just top speed right away. And the top speed is low.
        // I'll have to play with it more.
        // I think what I'm missing is friction?
        
        if (left > 0 && right > 0)
        {
            Debug.Log("Roll Forward");
            Rigidbody.AddRelativeForce(2 * treadTorque * Time.deltaTime * Vector3.forward, ForceMode.Force);
        }
        else if (left < 0 && right < 0)
        {
            Debug.Log("Roll Back");
            Rigidbody.AddRelativeForce(2 * treadTorque * Time.deltaTime * Vector3.back, ForceMode.Force);
        }
        else if (left > 0 && right < 0)
        {
            Debug.Log("Pivot Right");
            //transform.Rotate(Vector3.up, 5);
            Rigidbody.AddRelativeTorque(2 * treadTorque * Time.deltaTime * Vector3.up, ForceMode.Force);
        }
        else if (left < 0 && right > 0)
        {
            Debug.Log("Pivot Left");
            Rigidbody.AddRelativeTorque(2 * treadTorque * Time.deltaTime * Vector3.down, ForceMode.Force);
        }
        else if (left == 0 && right > 0)
        {
            Debug.Log("Neutral Forward Turn Left");
            Rigidbody.AddRelativeForce(1.25f * treadTorque * Time.deltaTime * Vector3.forward, ForceMode.Force);
            Rigidbody.AddRelativeTorque(0.75f * treadTorque * Time.deltaTime * Vector3.down, ForceMode.Force);
        }
        else if (left > 0 && right == 0)
        {
            Debug.Log("Neutral Forward Turn Right");
            Rigidbody.AddRelativeForce(1.25f * treadTorque * Time.deltaTime * Vector3.forward, ForceMode.Force);
            Rigidbody.AddRelativeTorque(0.75f * treadTorque * Time.deltaTime * Vector3.up, ForceMode.Force);
        }
        else if (left < 0 && right == 0)
        {
            Debug.Log("Neutral Backward Turn Left");
            Rigidbody.AddRelativeForce(1.25f * treadTorque * Time.deltaTime * Vector3.back, ForceMode.Force);
            Rigidbody.AddRelativeTorque(0.75f * treadTorque * Time.deltaTime * Vector3.down, ForceMode.Force);
        }
        else if (left == 0 && right < 0)
        {
            Debug.Log("Neutral Backward Turn Right");
            Rigidbody.AddRelativeForce(1.25f * treadTorque * Time.deltaTime * Vector3.back, ForceMode.Force);
            Rigidbody.AddRelativeTorque(0.75f * treadTorque * Time.deltaTime * Vector3.up, ForceMode.Force);
        }
    }
}
