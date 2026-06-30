using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MeleeAttackVariable
{
    // 피격 이벤트
    public List<Vector3> trailPositions { set; get; } = new List<Vector3>();
    public Transform weaponTip; // 무기 끝부분 위치
    public LayerMask enemyLayer;
    public float traceRadius = 0.3f; // 피격 판정 범위

    public bool pendingAttack { set; get; }
    public bool isAttacking { set; get; }
    public int dmg;

    public HashSet<Collider> alreadyHit = new HashSet<Collider>();
    public UnityEvent<Vector3> OnPlayHitEffect = new UnityEvent<Vector3>();
}

public class PlayerAttack : MonoBehaviour
{
    #region 변수
    private PlayerAnimationController playerAnimationController;
    private PlayerMovement playerMovement;
    public MeleeAttackVariable meleeAttackVariable;
    [SerializeField] private PlayerInputComponent playerInputComponent;


    public int comboType{get; private set; } // 콤보 타입

    [Header("유효한 클릭 최소 시간")]
    [Range(0f, 0.5f)]
    [SerializeField] private float minMouseUpTime = 0.1f; // 유효한 클릭으로 인식되기 위한 최소 클릭 시간 (초)

    [Header("유효한 클릭 최대 시간")]
    [Range(0f, 1f)]
    [SerializeField] private float maxMouseUpTime = 0.5f; // 유효한 클릭으로 인식되기 위한 최대 클릭 시간 (초)

    [Header("공격과 공격 사이 최대 시간")]
    [Range(0.5f, 2.0f)]
    [SerializeField] private float comboResetTime = 1f; // 콤보가 유지되기 위한 최대 공격 간 딜레이 (초)

    [Header("공격과 공격 사이 최소 시간")]
    [Range(0.3f, 0.5f)]
    [SerializeField] private float comboInputDelay = 0.4f;

    public float mouseUpTime { get; private set; } // 마우스를 떼는 시간

    private float aDelayTime; // 공격과 공격 사이 지연 가능 시간

    private bool onSkill = false;  // 스킬 확인

    #endregion

    private void Awake()
    {
        playerAnimationController = GetComponent<PlayerAnimationController>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    public void Update()
    {
        Attack();
    }

    public void FixedUpdate()
    {
        //Debug.Log("FixedUpdate 호출됨");
        if (meleeAttackVariable.isAttacking)
        {
            RecordWeaponTrail();

            if (meleeAttackVariable.pendingAttack)
            {
                CheckTrailCollision();
            }
        }
    }


    public void Attack()
    {
        if (!playerMovement.IsGrounded) return;
        //SetMouseInputTime(playerInputComponent.LeftMouseDownTime);

        if (comboType > 0)
        {
            aDelayTime += Time.deltaTime;

            if (aDelayTime > comboResetTime)
            {
                ResetCombo();
            }
        }

        if (onSkill) return;

        if (Input.GetMouseButtonDown(0))
        {
            mouseUpTime = 0f; // 눌렀을 때 초기화
        }

        if (Input.GetMouseButton(0))
        {            
            mouseUpTime += Time.deltaTime;
        }

        // 최소 시간
        if (Input.GetMouseButtonUp(0) && mouseUpTime >= minMouseUpTime)
        {
            HandleAttack(4); // 총 4번에 걸쳐 콤보를 진행
            mouseUpTime = 0f;
            playerAnimationController.SetAnimationBusy(true);
        }
    }

    /// <summary>
    /// 플레이어의 공격을 컨트롤.
    /// </summary>
    /// <param name="AttackCount">플레이어의 최대 공격콤보 횟수</param>
    private void HandleAttack(int AttackCount)
    {
        if (mouseUpTime < maxMouseUpTime)
        {
            if (comboType == 0)
            {
                playerAnimationController.CrossFadeAnimation("AttackTrigger");
                comboType = 1;
                aDelayTime = 0f; // 첫 공격시 초기화                
                return;
            }

            // 공격과 공격 0.4초 간격
            if (aDelayTime > comboInputDelay)
            {
                aDelayTime = 0f;
                comboType = (comboType >= AttackCount) ? 0 : comboType + 1;
                
            }
        }
    }

    private void ResetCombo()
    {
        comboType = 0;
        aDelayTime = 0f;

        playerAnimationController.SetAnimationBusy(false);
    }


    #region 피격관련 메서드
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

                        // 방향 비교
                        bool isHitFront = !IsFacingSameDirection(transform, hit.collider.transform);
                        IHealth healthComponent = hit.collider.GetComponent<IHealth>();
                        healthComponent.TakeDamage(meleeAttackVariable.dmg, isHitFront);

                        meleeAttackVariable.OnPlayHitEffect.Invoke(effectPos);
                    }
                }
            }
        }
    }


    private void OnDrawGizmos()
    {
        if (meleeAttackVariable == null || meleeAttackVariable.trailPositions == null || meleeAttackVariable.trailPositions.Count < 2)
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


    // 오버랩 그리는 애니메이션 이벤트
    public void StartTrail()
    {
        meleeAttackVariable.isAttacking = true;
        meleeAttackVariable.pendingAttack = true; // FixedUpdate에서 실행하도록 플래그 설정
        meleeAttackVariable.trailPositions.Clear();
        meleeAttackVariable.alreadyHit.Clear();
    }

    public void EndTrail()
    {
        meleeAttackVariable.isAttacking = false;
    }

    public void SetMouseInputTime(float time)
    {
        mouseUpTime = time;
    }


    /// <summary>
    /// 히트대상과 나의 각도를 판별하는 메서드
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool IsFacingSameDirection(Transform attacker, Transform target, float angleThreshold = 130f)
    {
        float angle = Vector3.Angle(attacker.forward, target.forward);
        return angle <= angleThreshold * 0.5f;
    }

    #endregion


}
