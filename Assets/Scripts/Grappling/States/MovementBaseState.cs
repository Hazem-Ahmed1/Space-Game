using UnityEngine;

public abstract class MovementBaseState
{
    public virtual void EnterState() { }
    public virtual void UpdateState() { }
    public virtual void ExitState() { }
}
