using UnityEngine;

public class HitState_NK : EnemyAttackAbleState
{
    public override void EnterState(int state)
    {        
        navMeshAgent.isStopped = true;        
    }

    // 추적 상태 기능 동작 처리 (상태 실행)
    public override void UpdateState()
    {
        if (!animationController.CheckAnimationEnd("Hit", 0.75f, true)) return;

        float playerDistance = controller.GetPlayerDistance();

        if (playerDistance <= attackDistance)
        {
            controller.TransactionToState((int)EnemyController_NK.STATE.DEFENSE);
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

    }
}
