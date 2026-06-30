using UnityEngine;
using UnityEngine.Events;

public class IdleState_LB : EnemyAttackAbleState
{
    private float time;

    public UnityEvent<bool, string, EnemyHealth> OnSetHealthUi;

    public override void EnterState(int state)
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.updateRotation = false;

        OnSetHealthUi.Invoke(true, "최종 보스", enemyHealth);

        // 상태 체크 주기 시간을 추첨함
        time = 0;
    }

    // 추적 상태 기능 동작 처리 (상태 실행)
    public override void UpdateState()
    {
        time += Time.deltaTime; // 대기 시간 계산

        if (time < 2f) return;

        float playerDistance = controller.GetPlayerDistance();

        if (playerDistance <= attackDistance)
        {
            controller.TransactionToState((int)EnemyController_LB.STATE.ATTACK);
        }
        else
        {
            controller.TransactionToState((int)EnemyController_LB.STATE.CHASE);
        }
    }

    public override void ExitState()
    {

    }
}
