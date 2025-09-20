using UnityEngine;
public class FlightPowerUp : MonoBehaviour 
{ [SerializeField] private float duration = 5f;

    [SerializeField] private float speedMultiplier = 2f;

    private void OnTriggerEnter(Collider other) { 
        if (!other.CompareTag("Player")) return; 

        PowerUpHandler handler = other.GetComponent<PowerUpHandler>();
        if (handler != null) { handler.ActivateFlight(duration, speedMultiplier); } 
        Destroy(gameObject); } }