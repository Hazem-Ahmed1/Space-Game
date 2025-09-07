using UnityEngine;

public class FreezeState : MovementBaseState
{
    PlayerManager player;
    public FreezeState(PlayerManager player)
    {
        this.player = player;
    }
    public override void EnterState()
    {
        if (player.TryGetComponent(out Rigidbody rb))
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }

    public override void UpdateState()
    {
        // No movement allowed
    }

    public override void ExitState()
    {
        if (player.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
        }
    }
}

