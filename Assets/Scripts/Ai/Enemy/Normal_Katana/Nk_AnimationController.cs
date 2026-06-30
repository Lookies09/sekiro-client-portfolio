using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class NKAnimatorParamId
{
    public static Dictionary<string, int> values = new Dictionary<string, int>();

    public static readonly int Velocity = Animator.StringToHash("Velocity"); 
    public static readonly int HorizontalValue = Animator.StringToHash("HorizontalValue");
    public static readonly int VerticalValue = Animator.StringToHash("VerticalValue");
    public static readonly int IsDefense = Animator.StringToHash("IsDefense"); 
    public static readonly int IsPatternEnd = Animator.StringToHash("IsPatternEnd");

    static NKAnimatorParamId()
    {
        values["Attack03_1"] = Animator.StringToHash("Attack03_1");
        values["Attack03_2"] = Animator.StringToHash("Attack03_2"); 
        values["Attack05"] = Animator.StringToHash("Attack05");
        values["Hit_F"] = Animator.StringToHash("Hit_F");
        values["Hit_B"] = Animator.StringToHash("Hit_B");
        values["APose2DefenseL"] = Animator.StringToHash("APose2DefenseL");
        values["DefenseL_Hit01"] = Animator.StringToHash("DefenseL_Hit01");
        values["DefenseL_Broken"] = Animator.StringToHash("DefenseL_Broken"); 
        values["Ambushed02"] = Animator.StringToHash("Ambushed02");
        values["Eecuted03"] = Animator.StringToHash("Eecuted03");
        values["Rebound"] = Animator.StringToHash("DefenseR_Rebound_1");
    }

}
public class Nk_AnimationController : EnemyAnimationController
{    
    private AttackState_NK attackState;

    protected override void Awake()
    {
        base.Awake();
        attackState = GetComponent<AttackState_NK>();
    }

    protected override void UpdateState()
    {
        // 캐릭터 기준 로컬 방향으로 변환
        Vector3 localMoveDir = transform.InverseTransformDirection(agent.velocity.normalized);
        animator.SetFloat(NKAnimatorParamId.HorizontalValue, localMoveDir.x);
        animator.SetFloat(NKAnimatorParamId.VerticalValue, localMoveDir.z);
        animator.SetBool(NKAnimatorParamId.IsDefense, enemyHealth.isDefense);
        animator.SetBool(NKAnimatorParamId.IsPatternEnd, attackState.GetCurrentPatternEnd());

        // 평소처럼 에이전트 속도 기반
        animator.SetFloat(NKAnimatorParamId.Velocity, agent.velocity.magnitude.Remap(0, 10, 0, 1));
    }

    public override void CrossFadeAnimation(string name)
    {
        int v = NKAnimatorParamId.values[name];

        animator.CrossFade(v, 0.1f);
    }

    public override bool CheckAnimationEnd(string name, float time, bool isTag)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 0은 Base Layer
        if (isTag)
        {
            // 애니메이션을 태그로 분류
            if (stateInfo.IsTag(name))
            {
                return stateInfo.normalizedTime >= time;
            }
        }
        else
        {
            // 이름으로 분류
            if (stateInfo.IsName(name))
            {
                return stateInfo.normalizedTime >= time;
            }
        }

        return false;
    }

}
