using UnityEngine;

public class DefenseState_LB : EnemyAttackAbleState
{
    private float time;

    public override void EnterState(int state)
    {
        enemyHealth.SetDefense(true);
        animationController.CrossFadeAnimation("Walk_R");
    }

    // 추적 상태 기능 동작 처리 (상태 실행)
    public override void UpdateState()
    {
        LookAtTarget(true);

        time += Time.deltaTime;

        if (time < 4f) return;

        float playerDistance = controller.GetPlayerDistance();

        if (playerDistance <= attackDistance)
        {
            controller.TransactionToState((int)EnemyController_LB.STATE.ATTACK);

            time = 0;
        }
        else
        {
            controller.TransactionToState((int)EnemyController_LB.STATE.CHASE);

            time = 0;
        }

    }

    public override void ExitState()
    {
        enemyHealth.SetDefense(false);
    }
}
