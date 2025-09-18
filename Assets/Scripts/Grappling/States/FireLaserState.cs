using UnityEngine;

public class FireLaserState : MovementBaseState
{
    //private static readonly int FireTrigger = Animator.StringToHash("fire");
    private PlayerManager player;

    public FireLaserState(PlayerManager player)
    {
        this.player = player;
    }

    public override void EnterState()
    {
        player.ShootLaser();
    }

    public override void UpdateState()
    {
        // After shooting → go back to Idle (or Float if moving)
        if (player._movement.Direction.sqrMagnitude > 0.01f)
        {
            player._stateMachine.SwitchState(player.floatState);
        }
        else
        {
            player._stateMachine.SwitchState(player.idleState);
        }
    }
}
