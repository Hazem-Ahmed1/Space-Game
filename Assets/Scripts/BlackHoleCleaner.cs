using UnityEngine;

public class BlackHoleCleaner : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Asteroid"))
        {
            other.gameObject.SetActive(false); // disable instead of destroy
        }
    }
}
