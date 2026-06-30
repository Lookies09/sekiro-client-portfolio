using UnityEngine;

public class HitState_LB : EnemyAttackAbleState
{
    public override void EnterState(int state)
    {
        // 공격 대상 추적 처리
        navMeshAgent.isStopped = true;
        animationController.CrossFadeAnimation("Hit_F");
    }

    // 추적 상태 기능 동작 처리 (상태 실행)
    public override void UpdateState()
    {
        if (!animationController.CheckAnimationEnd("Hit", 0.75f, true)) return;

        controller.TransactionToState((int)EnemyController_LB.STATE.DEFENSE);
    }

    // 추적 상태 종료(다른상태로 전이) 동작 처리(상태 정리)
    public override void ExitState()
    {
    }
}
