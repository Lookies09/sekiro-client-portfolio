using UnityEngine;

public class DefenseState_NK : EnemyAttackAbleState
{
    private float time;

    public override void EnterState(int state)
    {
        enemyHealth.SetDefense(true);
        animationController.CrossFadeAnimation("APose2DefenseL");
        navMeshAgent.isStopped = true;
        time = 0;
    }

    // 추적 상태 기능 동작 처리 (상태 실행)
    public override void UpdateState()
    {
        LookAtTarget(enemyHealth.isDefense);

        time += Time.deltaTime;

        if (time < 3f) return;

        float playerDistance = controller.GetPlayerDistance();

        if (playerDistance <= attackDistance)
        {
            controller.TransactionToState((int)EnemyController_NK.STATE.ATTACK);
        }
        else if (playerDistance <= detectDistance)
        {
            controller.TransactionToState((int)EnemyController_NK.STATE.CHASE);
        }
        else if (playerDistance > detectDistance)
        {
            controller.TransactionToState((int)EnemyController_NK.STATE.RETURN);
        }

    }

    public override void ExitState()
    {
        enemyHealth.SetDefense(false);
    }
}
