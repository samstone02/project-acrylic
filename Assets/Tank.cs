using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    [field: SerializeField]
    public GameObject TankAgentPrefab { get; set; }
    
    [field: SerializeField]
    public GameObject TankGunPrefab { get; set; }
    
    [field: SerializeField]
    public Rigidbody Rigidbody { get; set; }
    
    private BaseTankAgent _agent { get; set; }
    
    private BaseTankGun _tankGun { get; set; }
    
    private void Start()
    {
        Debug.Log("Hello, Tanks!");
        
        GameObject agentInstance = Instantiate(TankAgentPrefab, transform);
        _agent = agentInstance.GetComponent<BaseTankAgent>();
        
        GameObject gunInstance = Instantiate(TankGunPrefab, transform);
        _tankGun = TankGunPrefab.GetComponent<BaseTankGun>();
    }
    
    private void Update()
    {
        bool shootDecision = _agent.GetDecisionShoot();
        
        if (shootDecision)
        {
            Debug.Log("Player Shot");
            var projectile = _tankGun.Shoot();
        }

        (float left, float right) = _agent.GetDecisionMoveTreads();

        if (left > 0 && right > 0)
        {
            Debug.Log("Roll Forward");
            Rigidbody.velocity = transform.rotation * new Vector3(1, 0, 0);
        }
        else if (left < 0 && right < 0)
        {
            Debug.Log("Roll Back");
            Rigidbody.velocity = transform.rotation * new Vector3(-1, 0, 0);
        }
        else if (left > 0 && right < 0)
        {
            Debug.Log("Pivot Right");
            transform.Rotate(Vector3.up, 5);
        }
        else if (left < 0 && right > 0)
        {
            Debug.Log("Pivot Left");
            transform.Rotate(Vector3.up, -5);
        }
        else if (left > 0 && right == 0)
        {
            Debug.Log("Neutral Turn Left");
            Rigidbody.velocity = transform.rotation * new Vector3(0.75f, 0, 0);
            transform.Rotate(Vector3.up, 2);
        }
        else if (left == 0 && right > 0)
        {
            Debug.Log("Neutral Turn Right");
            Rigidbody.velocity = transform.rotation * new Vector3(0.75f, 0, 0);
            transform.Rotate(Vector3.up, -2);
        }
        else
        {
            Debug.Log("Stop");
            Rigidbody.velocity = Vector3.zero;
        }
    }
}
