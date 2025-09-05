using UnityEngine;

public class AsteroidDrift : MonoBehaviour
{
    public float driftSpeed = 2f;
    public float rotationSpeed = 50f;

    void OnEnable()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.velocity = Random.insideUnitSphere * driftSpeed;
            rb.angularVelocity = Random.insideUnitSphere * rotationSpeed * Mathf.Deg2Rad;
        }
    }
}
