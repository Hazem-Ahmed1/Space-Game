using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    public MovementBaseState CurrentState { get; private set; }
    public PlayerManager player;

    public PlayerStateMachine(PlayerManager player)
    {
        this.player = player;
    }

    public void SwitchState(MovementBaseState newState)
    {
        CurrentState?.ExitState();
        CurrentState = newState;
        CurrentState?.EnterState();
    }

    public void Update()
    {
        CurrentState?.UpdateState();
    }
}

