using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public static class LBAnimatorParamId
{
    public static Dictionary<string, int> values = new Dictionary<string, int>();

    public static readonly int Velocity = Animator.StringToHash("Velocity");
    public static readonly int HorizontalValue = Animator.StringToHash("HorizontalValue");
    public static readonly int VerticalValue = Animator.StringToHash("VerticalValue");
    public static readonly int IsDefense = Animator.StringToHash("IsDefense");
    public static readonly int IsPatternEnd = Animator.StringToHash("IsPatternEnd");

    static LBAnimatorParamId()
    {
        values["Hit_F"] = Animator.StringToHash("Hit_F");
        values["Walk_R"] = Animator.StringToHash("Walk_R");
        values["DefenseL_Hit01"] = Animator.StringToHash("DefenseL_Hit01");
        values["DefenseL_Broken"] = Animator.StringToHash("DefenseL_Broken");
        values["Rebound"] = Animator.StringToHash("DefenseR_Rebound_1");

        values["Parry_L2R_Up"] = Animator.StringToHash("Parry_L2R_Up");
        values["Parry_R2L_Up"] = Animator.StringToHash("Parry_R2L_Up");

        values["Bow_SP_Start"] = Animator.StringToHash("Bow_SP_Start");
        values["Bow_SP_Burst"] = Animator.StringToHash("Bow_SP_Burst");
        values["Bow_SP_End"] = Animator.StringToHash("Bow_SP_End");
        values["Bow_Unarm"] = Animator.StringToHash("Bow_Unarm");
        values["Bow_SP_Hold"] = Animator.StringToHash("Bow_SP_Hold");
        values["Bow_SP02"] = Animator.StringToHash("Bow_SP02"); 

        values["Dodge_B"] = Animator.StringToHash("Dodge_B");
        values["Slide_F"] = Animator.StringToHash("Slide_F");

        values["JumpAttack02"] = Animator.StringToHash("JumpAttack02");

        values["Dodge_Attack_F"] = Animator.StringToHash("Dodge_Attack_F");

        values["SPAttack03"] = Animator.StringToHash("SPAttack03");
        values["SPAttack04"] = Animator.StringToHash("SPAttack04");
        values["SPAttack05"] = Animator.StringToHash("SPAttack05");

        values["Attack01_1"] = Animator.StringToHash("Attack01_1");
        values["Attack02_1"] = Animator.StringToHash("Attack02_1");
        values["Attack03_1"] = Animator.StringToHash("Attack03_1");
        values["Attack03_2"] = Animator.StringToHash("Attack03_2");
        values["Attack03_3"] = Animator.StringToHash("Attack03_3");
        values["Attack03_4"] = Animator.StringToHash("Attack03_4");
        values["Attack03_5"] = Animator.StringToHash("Attack03_5");
        values["Attack05"] = Animator.StringToHash("Attack05");
    }

}
public class LB_AnimationController : EnemyAnimationController
{
    private AttackState_LB attackState;

    protected override void Awake()
    {
        base.Awake();
        attackState = GetComponent<AttackState_LB>();
    }

    protected override void UpdateState()
    {
        Vector3 localMoveDir = transform.InverseTransformDirection(agent.velocity.normalized);
        animator.SetFloat(NKAnimatorParamId.HorizontalValue, localMoveDir.x);
        animator.SetFloat(NKAnimatorParamId.VerticalValue, localMoveDir.z);
        animator.SetBool(NKAnimatorParamId.IsDefense, enemyHealth.isDefense);
        animator.SetBool(NKAnimatorParamId.IsPatternEnd, attackState.GetCurrentPatternEnd());
        animator.SetFloat(NKAnimatorParamId.Velocity, agent.velocity.magnitude.Remap(0, 10, 0, 1));
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

    public override void CrossFadeAnimation(string name)
    {
        int v = LBAnimatorParamId.values[name];

        animator.CrossFade(v, 0.1f);
    }    
}
