
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float deathDelay = 0.5f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit obstacle! Switching to DeadState in " + deathDelay + "s...");
            StartCoroutine(HandlePlayerDeath(collision.gameObject));
        }
    }

    private System.Collections.IEnumerator HandlePlayerDeath(GameObject player)
    {
        yield return new WaitForSeconds(deathDelay);

        PlayerManager manager = player.GetComponent<PlayerManager>();
        if (manager != null && manager._stateMachine != null)
        {
            manager._stateMachine.SwitchState(manager.deadState);
        }
    }
}
