using UnityEngine;

public class IdleState : MovementBaseState
{
    public override void EnterState(YPlayerManager movement)
    {
        movement.animator.SetBool("floating", false);
    }

    public override void UpdateState(YPlayerManager movement)
    {
        if (movement.dir.sqrMagnitude > 0.01f)
            movement.SwitchState(movement.floatState);
    }
}
