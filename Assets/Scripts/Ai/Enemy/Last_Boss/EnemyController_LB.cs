using UnityEngine;

public class EnemyController_LB : EnemyController
{
    public enum STATE { IDLE, CHASE, ATTACK, HIT, DEFENSE, PARRY, REBOUND, BROKEN, DEAD };

    public override bool IsExecuteAbleState()
    {
        return currentState == enemyStates[(int)STATE.BROKEN];
    }
}
