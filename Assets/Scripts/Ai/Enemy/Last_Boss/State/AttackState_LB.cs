using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;


namespace Ememy_LB
{
    [System.Serializable]
    public class MeleeAttackVariable
    {
        // 피격 이벤트
        public List<Vector3> trailPositions { set; get; } = new List<Vector3>();
        public Transform weaponTip; // 무기 끝부분 위치
        public LayerMask enemyLayer;
        public float traceRadius = 0.3f; // 피격 판정 범위

        public bool pendingAttack { set; get; }

        public HashSet<Collider> alreadyHit = new HashSet<Collider>();
    }
}

public class AttackState_LB : EnemyAttackAbleState, IParryableState
{
    public enum AttackPatternState {
        OneSlash, BowBurst, BowSP, JumpDown_Slay, JumpDown_Stab, 
        Dash_Slay, BossCombo, DodgeCombo }

    private AttackPatternState CurrentPattern;
    public Ememy_LB.MeleeAttackVariable meleeAttackVariable;
    [SerializeField] private AttackPattern_SO[] attackPatterns;
    public bool isAttacking { get; private set; }

    private bool isOnRotate = false;

    private void OnEnable()
    {
        PlayerDefense.OnParrySuccess += HandleParrySuccess;
    }

    private void OnDisable()
    {
        PlayerDefense.OnParrySuccess -= HandleParrySuccess;
    }

    public void FixedUpdate()
    {
        //Debug.Log("FixedUpdate 호출됨");
        if (isAttacking)
        {
            RecordWeaponTrail();

            if (meleeAttackVariable.pendingAttack)
            {
                CheckTrailCollision();
            }
        }
    }

    // 추적 상태 시작(진입) 처리(상태 초기화)
    public override void EnterState(int state)
    {
        // 공격 대상 추적 처리
        navMeshAgent.isStopped = true;

        CheckToSetPattern();

        isOnRotate = true;
    }

    // 추적 상태 기능 동작 처리 (상태 실행)
    public override void UpdateState()
    {
        LookAtTarget(isOnRotate);

        if (!attackPatterns[(int)CurrentPattern].isPatternFin) return;

        float playerDistance = controller.GetPlayerDistance();

        if (playerDistance <= attackDistance)
        {
            controller.TransactionToState((int)EnemyController_LB.STATE.DEFENSE);
        }
        else
        {
            controller.TransactionToState((int)EnemyController_LB.STATE.CHASE);
        }
    }

    // 추적 상태 종료(다른상태로 전이) 동작 처리(상태 정리)
    public override void ExitState()
    {
        EndTrail();
        meleeAttackVariable.pendingAttack = false;
        attackPatterns[(int)CurrentPattern].StopAttack(animationController);
    }

    #region 공격 관련 메서드

    public void SetAttackPattern(AttackPatternState attackPattern)
    {
        attackPatterns[(int)attackPattern].ExecuteAttack(this, animationController);
        CurrentPattern = attackPattern;
    }

    // 오버랩 그리는 애니메이션 이벤트
    public void StartTrail()
    {
        isOnRotate = false;
        isAttacking = true;
        meleeAttackVariable.pendingAttack = true; // FixedUpdate에서 실행하도록 플래그 설정
        meleeAttackVariable.trailPositions.Clear();
        meleeAttackVariable.alreadyHit.Clear();
    }

    public void EndTrail()
    {
        isAttacking = false;
    }

    public void RecordWeaponTrail()
    {
        meleeAttackVariable.trailPositions.Add(meleeAttackVariable.weaponTip.position);

        // 너무 많이 쌓이지 않도록 적절히 관리
        if (meleeAttackVariable.trailPositions.Count > 10)
            meleeAttackVariable.trailPositions.RemoveAt(0);
    }


    public void CheckTrailCollision()
    {
        float segmentLength = 0.05f; // 더 촘촘한 간격
        for (int i = 0; i < meleeAttackVariable.trailPositions.Count - 1; i++)
        {
            Vector3 start = meleeAttackVariable.trailPositions[i];
            Vector3 end = meleeAttackVariable.trailPositions[i + 1];
            float totalDist = Vector3.Distance(start, end);
            int steps = Mathf.CeilToInt(totalDist / segmentLength);

            for (int s = 0; s < steps; s++)
            {
                Vector3 subStart = Vector3.Lerp(start, end, s / (float)steps);
                Vector3 subEnd = Vector3.Lerp(start, end, (s + 1) / (float)steps);

                RaycastHit[] hits = Physics.CapsuleCastAll(
                    subStart,
                    subEnd,
                    meleeAttackVariable.traceRadius,
                    (subEnd - subStart).normalized,
                    (subEnd - subStart).magnitude,
                    meleeAttackVariable.enemyLayer
                );

                foreach (RaycastHit hit in hits)
                {
                    if (!meleeAttackVariable.alreadyHit.Contains(hit.collider))
                    {
                        meleeAttackVariable.alreadyHit.Add(hit.collider);
                        Vector3 closestPoint = hit.collider.ClosestPoint((subStart + subEnd) / 2f);
                        Vector3 dir = (closestPoint - (subStart + subEnd) / 2f).normalized;
                        Vector3 effectPos = closestPoint - dir * 0.05f;

                        IHealth healthComponent = hit.collider.GetComponent<IHealth>();

                        healthComponent.TakeDamage(dmg, true, isDefenseable);

                        effectController.OnPlayHitEffect(effectPos);
                    }
                }
            }
        }
    }

    public bool GetCurrentPatternEnd()
    {
        return attackPatterns[(int)CurrentPattern].isPatternFin;
    }

    public void Rebound()
    {
        if (!isReboundable) return;
        controller.TransactionToState((int)EnemyController_LB.STATE.REBOUND);
    }

    private void HandleParrySuccess(GameObject parriedEnemy)
    {
        if (parriedEnemy == gameObject)
        {
            Rebound();
        }
    }

    public void SetRotate()
    {
        isOnRotate = true;
    }

    private void OnDrawGizmos()
    {
        if (meleeAttackVariable.trailPositions == null || meleeAttackVariable.trailPositions.Count < 2)
            return;

        Gizmos.color = Color.red;

        for (int i = 0; i < meleeAttackVariable.trailPositions.Count - 1; i++)
        {
            Vector3 start = meleeAttackVariable.trailPositions[i];
            Vector3 end = meleeAttackVariable.trailPositions[i + 1];
            Vector3 direction = end - start;
            float distance = direction.magnitude;

            if (distance == 0f) continue; // same point

            direction.Normalize();

            // Draw capsule-like gizmo
            int segments = 12;
            Quaternion rotation = Quaternion.LookRotation(direction);
            float radius = meleeAttackVariable.traceRadius;

            // Draw end spheres
            Gizmos.DrawWireSphere(start, radius);
            Gizmos.DrawWireSphere(end, radius);

            // Draw cylinder-like middle
            for (int j = 0; j < segments; j++)
            {
                float angleA = (j / (float)segments) * Mathf.PI * 2;
                float angleB = ((j + 1) / (float)segments) * Mathf.PI * 2;

                Vector3 offsetA = rotation * new Vector3(Mathf.Cos(angleA), Mathf.Sin(angleA), 0) * radius;
                Vector3 offsetB = rotation * new Vector3(Mathf.Cos(angleB), Mathf.Sin(angleB), 0) * radius;

                Vector3 p1 = start + offsetA;
                Vector3 p2 = start + offsetB;
                Vector3 p3 = end + offsetA;
                Vector3 p4 = end + offsetB;

                Gizmos.DrawLine(p1, p3); // side lines
                Gizmos.DrawLine(p1, p2); // cap circle at start
                Gizmos.DrawLine(p3, p4); // cap circle at end
            }
        }
    }

    private void CheckToSetPattern()
    {
        float playerDistance = controller.GetPlayerDistance();

        AttackPatternState[] possiblePatterns;

        if (playerDistance <= 3f)
        {
            possiblePatterns = new AttackPatternState[]
            {
                AttackPatternState.OneSlash,
                AttackPatternState.JumpDown_Slay,
                AttackPatternState.JumpDown_Stab,
                AttackPatternState.BossCombo
            };
        }
        else if(playerDistance <= 5f)
        {            
            possiblePatterns = new AttackPatternState[]
            {
                AttackPatternState.Dash_Slay,
                AttackPatternState.DodgeCombo,
            };
        }
        else
        {
            int ran = Random.Range(0, 10);

            if (ran >= 4)
            {
                controller.TransactionToState((int)EnemyController_NK.STATE.CHASE);
                return;
            }
            else
            {
                possiblePatterns = new AttackPatternState[]
                {
                AttackPatternState.BowBurst,
                AttackPatternState.BowSP
                };
            }                
        }

        // 패턴 선택
        int index = Random.Range(0, possiblePatterns.Length);
        SetAttackPattern(possiblePatterns[index]);
    }

    #endregion

}
