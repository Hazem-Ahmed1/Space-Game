using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    [Header("Setup")]
    public GameObject destructableObject;
    public float explosionForce = 300f;     // Force amount
    public float explosionRadius = 5f;      // How far the force spreads
    public float upwardModifier = 0.5f;     // Push upward slightly
    public GameObject vfx;

    private void Destruct(Vector3 hitPoint)
    {
        GameObject destroyed = Instantiate(destructableObject, transform.position, transform.rotation);

        Rigidbody[] pieces = destroyed.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in pieces)
        {
            rb.AddExplosionForce(explosionForce, hitPoint, explosionRadius, upwardModifier, ForceMode.Impulse);
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Laser"))
        {
            // Use collision contact point as explosion center
            Vector3 hitPoint = collision.contacts[0].point;
            Instantiate(vfx,collision.transform.position,Quaternion.identity);
            Destroy(collision.gameObject);
            Destruct(hitPoint);
            Destroy(collision.gameObject);
        }
    }
}
