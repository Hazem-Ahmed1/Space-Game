//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Destructable : MonoBehaviour
//{
//    [Header("Setup")]
//    public GameObject destructableObject;
//    public float explosionForce = 300f;     // Force amount
//    public float explosionRadius = 5f;      // How far the force spreads
//    public float upwardModifier = 0.5f;     // Push upward slightly
//    public GameObject vfx;

//    private void Destruct(Vector3 hitPoint)
//    {
//        GameObject destroyed = Instantiate(destructableObject, transform.position, transform.rotation);

//        Rigidbody[] pieces = destroyed.GetComponentsInChildren<Rigidbody>();
//        foreach (Rigidbody rb in pieces)
//        {
//            rb.AddExplosionForce(explosionForce, hitPoint, explosionRadius, upwardModifier, ForceMode.Impulse);
//        }
//        Destroy(gameObject);
//    }

//    private void OnCollisionEnter(Collision collision)
//    {
//        if (collision.gameObject.CompareTag("Laser"))
//        {
//            // Use collision contact point as explosion center
//            Vector3 hitPoint = collision.contacts[0].point;
//            Instantiate(vfx,collision.transform.position,Quaternion.identity);
//            Destroy(collision.gameObject);
//            Destruct(hitPoint);
//            Destroy(collision.gameObject);
//        }
//    }
//}


using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Destructable : MonoBehaviour, IPoolable
{
    [Header("Setup")]
    public GameObject destructableObject;
    public float explosionForce = 300f;
    public float explosionRadius = 5f;
    public float upwardModifier = 0.5f;
    public GameObject vfx;

    [Header("Pool Settings")]
    public bool usePooling = true;
    public float resetDelay = 5f; // Time before pieces are returned to pool

    private bool isDestroyed = false;
    private GameObject currentDestroyedInstance;

    private void Destruct(Vector3 hitPoint)
    {
        if (isDestroyed) return;
        isDestroyed = true;

        GameObject destroyed = Instantiate(destructableObject, transform.position, transform.rotation);
        currentDestroyedInstance = destroyed;

        Rigidbody[] pieces = destroyed.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in pieces)
        {
            rb.AddExplosionForce(explosionForce, hitPoint, explosionRadius, upwardModifier, ForceMode.Impulse);
        }

        // Hide the original object instead of destroying it
        gameObject.SetActive(false);

        // Start coroutine to clean up pieces after delay
        if (usePooling)
        {
            StartCoroutine(CleanupPieces());
        }
        else
        {
            Destroy(destroyed, resetDelay);
        }
    }

    private IEnumerator CleanupPieces()
    {
        yield return new WaitForSeconds(resetDelay);

        if (currentDestroyedInstance != null)
        {
            // Instead of destroying, you can return pieces to a pool
            // For now, we'll disable them
            currentDestroyedInstance.SetActive(false);
            // Or destroy if not using pooling for pieces
            Destroy(currentDestroyedInstance);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Laser"))
        {
            // Use collision contact point as explosion center
            Vector3 hitPoint = collision.contacts[0].point;

            if (vfx != null)
            {
                Instantiate(vfx, collision.transform.position, Quaternion.identity);
            }

            // Handle laser pooling
            IPoolable laserPoolable = collision.gameObject.GetComponent<IPoolable>();
            if (laserPoolable != null)
            {
                laserPoolable.ReturnToPool();
            }
            else
            {
                Destroy(collision.gameObject);
            }

            Destruct(hitPoint);
        }
    }

    // Implement IPoolable interface
    public void ReturnToPool()
    {
        // Reset the object state
        isDestroyed = false;

        // Clean up any existing destroyed instance
        if (currentDestroyedInstance != null)
        {
            Destroy(currentDestroyedInstance);
            currentDestroyedInstance = null;
        }

        // Return to pool (disable the object)
        gameObject.SetActive(false);

        // If you have an object pooling system, call it here
        // ObjectPool.Instance.ReturnObject(gameObject);
    }

    // Method to reset object when retrieved from pool
    public void ResetForReuse()
    {
        isDestroyed = false;
        currentDestroyedInstance = null;
        gameObject.SetActive(true);
    }
}