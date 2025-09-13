using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class DeadState : MovementBaseState
{
    PlayerManager player;
    public DeadState(PlayerManager player)
    {
        this.player = player;
    }

    public override void EnterState()
    {
        Debug.Log("Player Dead!");

        player._movement.Direction = Vector3.zero;

        player.animator.SetTrigger("Die");

        player.StartCoroutine(DeathDelay());
    }

    private System.Collections.IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0f;

        GameObject gameOverUI = GameObject.Find("GameOverUI");
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }

    public override void UpdateState()
    {
        // In DeadState, we might want to disable player controls or show a death screen.
        // For now, we'll just keep the player in this state without any transitions.
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}

