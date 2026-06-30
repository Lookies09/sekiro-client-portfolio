using UnityEngine;

public class ChaseState_NK : EnemyAttackAbleState
{
    // 추적 이동 속도
    [SerializeField] private float chaseSpeed;

    // 추적 상태 시작(진입) 처리(상태 초기화)
    public override void EnterState(int state)
    {
        // 공격 대상 추적 처리
        navMeshAgent.isStopped = false;

        // 추적 속도 설정
        navMeshAgent.speed = chaseSpeed;
    }

    // 추적 상태 기능 동작 처리 (상태 실행)
    public override void UpdateState()
    {
        MoveRotation();

        float playerDistance = controller.GetPlayerDistance();

        navMeshAgent.SetDestination(controller.player.transform.position);

        // 추격 가능거리를 벗어나면
        if (playerDistance > detectDistance)
        {          
            controller.TransactionToState((int)EnemyController_NK.STATE.RETURN);
            
        }
        // 공격 가능 거리안에 있으면
        else if (playerDistance <= attackDistance)
        {            
            controller.TransactionToState((int)EnemyController_NK.STATE.ATTACK);
            
        }

    }

    // 추적 상태 종료(다른상태로 전이) 동작 처리(상태 정리)
    public override void ExitState()
    {

    }
}
