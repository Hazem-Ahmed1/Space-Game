using UnityEngine;

public class IdleState : MovementBaseState
{
    private static readonly int FloatingHash = Animator.StringToHash("floating");
    PlayerManager player;
    public IdleState(PlayerManager player)
    {
        this.player = player;
    }
    public override void EnterState()
    {
        if (player.animator != null)
        {
            player.animator.SetBool(FloatingHash, false);
        }
    }

    public override void UpdateState()
    {
        // Transition condition: when the player is moving
        if (player._movement.Direction.sqrMagnitude > 0.01f)
        {
            player._stateMachine.SwitchState(player.floatState);
        }
    }
    public override void ExitState()
    {
        base.ExitState();
    }
}

