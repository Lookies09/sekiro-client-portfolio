using UnityEngine;

public class ChaseState_LB : EnemyAttackAbleState
{
    // 추적 이동 속도
    [SerializeField] private float chaseSpeed;

    public override void EnterState(int state)
    {
        navMeshAgent.isStopped = false;
        // 추적 속도 설정
        navMeshAgent.speed = chaseSpeed;
    }

    public override void UpdateState()
    {
        MoveRotation();

        float playerDistance = controller.GetPlayerDistance();

        navMeshAgent.SetDestination(controller.player.transform.position);

        // 공격 가능 거리안에 있으면
        if (playerDistance <= attackDistance)
        {
            controller.TransactionToState((int)EnemyController_LB.STATE.ATTACK);

        }

    }

    // 추적 상태 종료(다른상태로 전이) 동작 처리(상태 정리)
    public override void ExitState()
    {

    }
}
