using UnityEngine;

public abstract class MovementBaseState
{
    public abstract void EnterState(YPlayerManager movement);
    public abstract void UpdateState(YPlayerManager movement);
}
