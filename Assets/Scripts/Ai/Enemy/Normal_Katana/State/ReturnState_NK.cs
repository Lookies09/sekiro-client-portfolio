using TMPro;
using UnityEngine;

public class ReturnState_NK : EnemyAttackAbleState
{
    [SerializeField] private float returnSpeed;

    private float time = 0;

    public override void EnterState(int state)
    {
        SetClosestPos();

        navMeshAgent.isStopped = false;

        // 추적 속도 설정
        navMeshAgent.speed = returnSpeed;

        time = 0;
    }

    // 추적 상태 기능 동작 처리 (상태 실행)
    public override void UpdateState()
    {
        time += Time.deltaTime;
        LookAtTarget(time <= 5);
        if (!(time <= 5))
        {
            navMeshAgent.speed = 3f;
            MoveRotation();
        }

        bool isFInd = detector.FindTarget(controller.player);
        float targetDistance = Vector3.Distance(transform.position, navMeshAgent.destination);
        if (isFInd)
        {
            if (controller.GetPlayerDistance() <= detectDistance)
            {
                controller.TransactionToState((int)EnemyController_NK.STATE.CHASE);
            }
            // 공격 가능 거리안에 있으면
            else if (controller.GetPlayerDistance() <= attackDistance)
            {
                controller.TransactionToState((int)EnemyController_NK.STATE.ATTACK);
            }
        }               
        
        if(targetDistance < 0.5f)
        {
            if (navMeshAgent.velocity.sqrMagnitude == 0f)
            {
                controller.TransactionToState((int)EnemyController_NK.STATE.IDLE);
            }
        }

    }

    // 추적 상태 종료(다른상태로 전이) 동작 처리(상태 정리)
    public override void ExitState()
    {

    }

    public void SetClosestPos()
    {
        Transform[] poses = GetComponent<PatrolState_NK>().PatrolPoints;
        Transform closest = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Transform pose in poses)
        {
            float distance = Vector3.Distance(currentPosition, pose.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = pose;
            }
        }

        if (closest != null)
        {
            navMeshAgent.SetDestination(closest.position);
        }
    }
}
