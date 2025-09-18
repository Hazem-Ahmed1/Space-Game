using UnityEngine;
using System.Collections;

public class PowerUpHandler : MonoBehaviour
{
    private Animator animator;
    private PlayerManager playerManager;

    private float originalSpeed;
    private bool isFlyingActive;
    
    //[SerializeField] private GameObject flightFire;
    [SerializeField] private GameObject rightShoe;   
    [SerializeField] private GameObject leftShoe;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerManager = GetComponent<PlayerManager>();
    }

    public void ActivateFlight(float duration, float speedMultiplier)
    {
        if (isFlyingActive) return;

        isFlyingActive = true;

        animator.SetBool("Fly", true);


        originalSpeed = playerManager._movement.BaseSpeed;
        playerManager._movement.BaseSpeed *= speedMultiplier;

        if (rightShoe != null) rightShoe.SetActive(true);
        if (leftShoe != null) leftShoe.SetActive(true);
        
        StartCoroutine(ResetFlight(duration));
    }

    private IEnumerator ResetFlight(float duration)
    {
        yield return new WaitForSeconds(duration);

        animator.SetBool("Fly", false);


        playerManager._movement.BaseSpeed = originalSpeed;

        if (rightShoe != null) rightShoe.SetActive(false);
        if (leftShoe != null) leftShoe.SetActive(false);

        isFlyingActive = false;
    }
}