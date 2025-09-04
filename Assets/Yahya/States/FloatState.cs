using UnityEngine;

public class FloatState : MovementBaseState
{
    public override void EnterState(YPlayerManager movement)
    {
        movement.animator.SetBool("floating", true);
    }

    public override void UpdateState(YPlayerManager movement)
    {
        if (movement.dir.sqrMagnitude < 0.01f)
            movement.SwitchState(movement.idleState);
    }
}
