using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class AttractableCharacterController : MonoBehaviour
{
    public float attractionSpeed = 5f;

    // This method now returns the attraction force instead of applying it directly.
    public Vector3 GetAttractionForce(Vector3 attractorPosition, float attractionForce)
    {
        Vector3 direction = (attractorPosition - transform.position);
        float distance = direction.magnitude;

        // Check for destruction and return a game over signal or handle it here.
        if (distance <= 0.5f) // Using a hardcoded value, consider making it a property
        {
            Debug.Log("Game Over");
            return Vector3.zero; // Or a specific signal for game over state
        }

        // Calculate the attraction velocity.
        //float pullStrength = attractionForce / (distance * distance);
        float pullStrength = attractionForce;
        Vector3 attractionMove = direction.normalized * pullStrength;

        // Apply a multiplier to scale the attraction speed
        return attractionMove * attractionSpeed;
    }
}