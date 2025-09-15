using System.Collections;
using UnityEngine;

public class GrapplingState : MovementBaseState
{
    private Vector3 velocity;
    private Vector3 targetPosition;
    private bool isGrappling;
    private float speed = 30f; // adjust speed of grappling pull
    PlayerManager player;
    public GrapplingState(PlayerManager player)
    {
        this.player = player;
    }

    public override void EnterState()
    {
        isGrappling = false;
        velocity = Vector3.zero;
        //player.animator.SetBool("Fly", true);

        player._cameraController.ZoomIn(); // Zoom in when grappling starts
    }

    public override void UpdateState()
    {
        if (!isGrappling) return;

        if (player.TryGetComponent(out CharacterController controller))
        {
            // Move with velocity
            controller.Move(velocity * Time.deltaTime);

            // Check distance to target
            float distance = Vector3.Distance(player.transform.position, targetPosition);

            if (distance <= player.stopDistance)
            {
                // Snap to target
                player.transform.position = targetPosition;

                // Stop grappling
                player._stateMachine.SwitchState(player.idleState);
            }
        }
    }

    public override void ExitState()
    {
        //player.animator?.SetBool("grappling", false);
        isGrappling = false;
        velocity = Vector3.zero;
        player._cameraController.ZoomOut(); // Back to normal FOV
        //player.animator.SetBool("Fly", false);
    }

    // Called externally from Grappling.cs
    public void JumpToPosition(Vector3 target, float trajectoryHeight)
    {
        targetPosition = target;
        velocity = CalculateVelocity(player.transform.position, target, trajectoryHeight);

        isGrappling = true;

        // Safety reset in case never reached
        player.StartCoroutine(ResetAfterDelay());
    }

    private IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(player.resetDelay);
        if (isGrappling)
        {
            player._stateMachine.SwitchState(player.idleState);
        }
    }

    private Vector3 CalculateVelocity(Vector3 start, Vector3 end, float trajectoryHeight)
    {
        // In space, no arc → just constant velocity towards target
        Vector3 direction = (end - start).normalized;
        return direction * speed;
    }
}
