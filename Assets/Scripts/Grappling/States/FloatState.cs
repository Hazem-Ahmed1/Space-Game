using UnityEngine;

public class FloatState : MovementBaseState
{
    PlayerManager movement;
    public FloatState(PlayerManager movement)
    {
        this.movement = movement;
    }
    public override void EnterState()
    {
        movement.animator.SetBool("floating", true);
    }

    public override void UpdateState()
    {
        if (movement._movement.Direction.sqrMagnitude < 0.01f)
            movement._stateMachine.SwitchState(movement.idleState);
    }
    public override void ExitState()
    {
        base.ExitState();
    }
}
