using UnityEngine;

public class ReboundState_NK : EnemyAttackAbleState
{
    public override void EnterState(int state)
    {
        navMeshAgent.isStopped = true;
        animationController.CrossFadeAnimation("Rebound");
    }

    // 추적 상태 기능 동작 처리 (상태 실행)
    public override void UpdateState()
    {
        if (!animationController.CheckAnimationEnd("DefenseR_Rebound_1", 0.8f, false)) return;

        float playerDistance = controller.GetPlayerDistance();

        if (playerDistance <= attackDistance)
        {
            controller.TransactionToState((int)EnemyController_NK.STATE.DEFENSE);
        }
        else if (playerDistance <= detectDistance)
        {
            controller.TransactionToState((int)EnemyController_NK.STATE.CHASE);
        }
        else
        {
            controller.TransactionToState((int)EnemyController_NK.STATE.RETURN);
        }
    }

    public override void ExitState()
    {

    }
}
