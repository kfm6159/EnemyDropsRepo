using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class AmmoBox : MonoBehaviour
{
    [Header("Pickup Settings")]
    [AmmoType]
    public int ammoType;
    public int amount = 30;

    [Header("Physics Settings")]
    public float dropForce = 2f; //upward force

    private Rigidbody rb;
    private bool hasLanded = false;

void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.AddForce(Vector3.up * dropForce, ForceMode.Impulse);
    }
    
    // Detect when we hit the ground
    void OnCollisionEnter(Collision collision)
    {
        if (!hasLanded)
        {
            hasLanded = true;
            // Optional: Stop rolling around after landing
            rb.angularVelocity = Vector3.zero;
        }
    }
    
    // Existing trigger detection for pickup
    void OnTriggerEnter(Collider other)
    {
        Controller c = other.GetComponent<Controller>();
        
        if (c != null)
        {
            c.ChangeAmmo(ammoType, amount);
            Destroy(gameObject);
        }
    }
}