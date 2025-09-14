// AttractableRigidbody.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AttractableRigidbody : MonoBehaviour, IAttractable
{
    private Rigidbody rb;
    private BlackHoleCore blackHoleCore;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        blackHoleCore = FindObjectOfType<BlackHoleCore>(); // Find the black hole to call its Grow method
    }

    public void Attract(Vector3 attractorPosition, float attractionForce, float destroyRadius)
    {
        Vector3 direction = (attractorPosition - transform.position);
        float distance = direction.magnitude;

        // Check for destruction
        if (distance <= destroyRadius)
        {
            HandleDestruction();
            return;
        }

        // Apply attraction force
        float forceMagnitude = attractionForce;
        Vector3 force = direction.normalized * forceMagnitude;
        rb.AddForce(force, ForceMode.Acceleration);
    }

    private void HandleDestruction()
    {
        // For simplicity, we'll just disable the object and call the black hole's grow method
        if (blackHoleCore != null)
        {
            blackHoleCore.Grow();
        }
        this.transform.gameObject.SetActive(false);
    }
}