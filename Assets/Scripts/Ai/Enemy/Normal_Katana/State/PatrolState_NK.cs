using UnityEngine;
using UnityEngine.AI;

public class PatrolState_NK : EnemyAttackAbleState
{
    [SerializeField] private float radius = 5f;
    [SerializeField] private float wanderNavCheckRadius = 2f;
    [SerializeField] private float moveSpeed = 3.5f;

    [SerializeField] private Transform[] patrolPoints;

    private Vector3 targetPosition = Vector3.positiveInfinity;
    private Vector3 lastTargetPosition = Vector3.positiveInfinity;

    public Transform[] PatrolPoints { get => patrolPoints;}

    public override void EnterState(int state)
    {
        lastTargetPosition = Vector3.positiveInfinity;
        NewRandomDestination();
    }

    // 추적 상태 기능 동작 처리 (상태 실행)
    public override void UpdateState()
    {
        bool isFInd = detector.FindTarget(controller.player);
        float playerDistance = controller.GetPlayerDistance();
        float targetDistance = Vector3.Distance(transform.position, targetPosition);
        MoveRotation();

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
        else if (targetDistance < 0.5f && navMeshAgent.velocity.sqrMagnitude == 0f)
        {
            controller.TransactionToState((int)EnemyController_NK.STATE.IDLE);
        }        
    }

    public override void ExitState()
    {

    }    

    private void NewRandomDestination()
    {
        if (PatrolPoints == null || PatrolPoints.Length == 0)
        {
            Debug.LogWarning("[Patrol] 배회 위치가 설정되지 않았습니다.");
            return;
        }

        const int maxTries = 10;
        const float minDistanceFromLastTarget = 1.0f;

        for (int i = 0; i < maxTries; i++)
        {
            int index = Random.Range(0, PatrolPoints.Length);
            Vector3 basePos = PatrolPoints[index].position;
            float distance = Vector3.Distance(basePos, transform.position);

            if (distance < radius)
                continue;

            Vector3 randomDirection = Random.insideUnitSphere * radius + basePos;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomDirection, out hit, wanderNavCheckRadius, NavMesh.AllAreas))
            {
                if (Vector3.Distance(hit.position, lastTargetPosition) < minDistanceFromLastTarget)
                    continue;

                targetPosition = hit.position;
                lastTargetPosition = targetPosition;

                navMeshAgent.isStopped = false;
                navMeshAgent.speed = moveSpeed;
                navMeshAgent.SetDestination(targetPosition);
                return;
            }
        }
    }   

}
