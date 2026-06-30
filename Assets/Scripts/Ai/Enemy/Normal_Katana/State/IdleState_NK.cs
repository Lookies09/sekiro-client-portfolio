using UnityEngine;

public class IdleState_NK : EnemyAttackAbleState
{
    private float time;
    private float checkTime;
    [SerializeField] private Vector2 checkTimeRange; // 대기 체크 시간 (최소,최대)

    public override void EnterState(int state)
    {        
        navMeshAgent.isStopped = true;
        navMeshAgent.updateRotation = false;

        // 상태 체크 주기 시간을 추첨함
        time = 0;
        checkTime = Random.Range(checkTimeRange.x, checkTimeRange.y);
    }

    // 추적 상태 기능 동작 처리 (상태 실행)
    public override void UpdateState()
    {
        time += Time.deltaTime; // 대기 시간 계산

        bool isFInd = detector.FindTarget(controller.player);
        float playerDistance = controller.GetPlayerDistance();

        if (isFInd)
        {
            if (playerDistance <= attackDistance)
            {
                controller.TransactionToState((int)EnemyController_NK.STATE.ATTACK);
            }
            else if (playerDistance <= detectDistance)
            {
                controller.TransactionToState((int)EnemyController_NK.STATE.CHASE);
            }
        }

        // 대기 상태가 지났다면
        if (time > checkTime)
        {
            controller.TransactionToState((int)EnemyController_NK.STATE.PATROL);
            return;
        }
    }

    public override void ExitState()
    {

    }
}
