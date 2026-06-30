using UnityEngine;

public class EnemyAttackAbleState : EnemyState
{
    [SerializeField] protected float attackDistance; // 플레이어 공격 가능 거리
    [SerializeField] protected float detectDistance; // 플레이어 추적 가능 거리
    [SerializeField] protected float slerpValue = 20;
    protected int dmg;
    protected bool isReboundable;
    protected bool isDefenseable;

    public override void EnterState(int state)
    {

    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {

    }

    // 대상을 주시
    public void LookAtTarget(bool isFindTarget)
    {
        if (!isFindTarget) return;

        // 공격 대상을 향한 방향을 계산
        Vector3 direction = (controller.player.transform.position - transform.position).normalized;
        direction.y = 0f;

        // 회전 쿼터니언 계산
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        if (Quaternion.Angle(transform.rotation, lookRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, slerpValue * Time.deltaTime);
        }
    }

    // 이동하는 방향으로 부드럽게 회전
    public void MoveRotation()
    {
        if (navMeshAgent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(navMeshAgent.velocity.normalized);
            float maxDegreesPerSecond = 360f; // 1초에 360도 회전
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxDegreesPerSecond * Time.deltaTime);
        }
    }

    public void SetAttackType(int dmg, bool isReboundable, bool isDefenseable)
    {
        this.dmg = dmg;
        this.isReboundable = isReboundable;
        this.isDefenseable = isDefenseable;
    }
}
