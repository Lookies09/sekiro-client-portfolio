using Unity.VisualScripting;
using UnityEngine;

public class EnemyController_NK : EnemyController
{
    public enum STATE { IDLE, PATROL, CHASE, RETURN, ATTACK, HIT, DEFENSE, REBOUND ,BROKEN, DEAD };

    public override bool IsExecuteAbleState()
    {
        return currentState == enemyStates[(int)STATE.IDLE]
          || currentState == enemyStates[(int)STATE.PATROL]
          || currentState == enemyStates[(int)STATE.BROKEN]
          || currentState == enemyStates[(int)STATE.RETURN];
    }
}
