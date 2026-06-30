using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "AttackPattern/Pattern")]
public class AT_Pattern : AttackPattern_SO
{
    

    private Coroutine attackRoutine;

    public override void ExecuteAttack(EnemyAttackAbleState attackState, EnemyAnimationController animationController)
    {
        // 기존 공격 코루틴이 있다면 먼저 정지
        if (attackRoutine != null)
        {
            animationController.StopCoroutine(attackRoutine);
        }

        // 새로 시작
        attackRoutine = animationController.StartCoroutine(PlayAttackSequence(attackState, animationController));

    }

    private IEnumerator PlayAttackSequence(EnemyAttackAbleState attackState, EnemyAnimationController animationController)
    {
        isPatternFin = false;

        for (int i = 0; i < animationNames.Length; i++)
        {
            attackState.SetAttackType(damages[i], isReboundableAttack[i], isDefenseableAttack[i]);
            if (!isDefenseableAttack[i])
            {
                DangerEventManager.RaiseDangerEvent();
            }

            // 애니메이션 전환
            animationController.CrossFadeAnimation(animationNames[i]);

            // 애니메이션이 끝날 때까지 기다리기
            yield return WaitForAnimationEnd(animationTimes[i]);
        }

        isPatternFin = true;
    }

    private IEnumerator WaitForAnimationEnd(float waitTime)
    {
        // 애니메이션이 끝날 때까지 대기
        yield return new WaitForSeconds(waitTime);
    }

    public override void StopAttack(EnemyAnimationController animationController)
    {
        if (attackRoutine != null)
        {            
            animationController.StopCoroutine(attackRoutine);
            attackRoutine = null;
            isPatternFin = true;
        }
    }

}
