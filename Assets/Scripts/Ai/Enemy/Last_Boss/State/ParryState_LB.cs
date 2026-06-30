using UnityEngine;
using UnityEngine.Events;

public class ParryState_LB : EnemyAttackAbleState
{
    private bool isRightParry;

    private int parryCount = 0;

    [SerializeField] private Transform parryEffectPos;
    public UnityEvent<Vector3> OnPlayParryEffect = new UnityEvent<Vector3>();

    public override void EnterState(int state)
    {
        //Walk_FR
        enemyHealth.SetDefense(true);
        parryCount++;

        if (isRightParry)
        {
            animationController.CrossFadeAnimation("Parry_R2L_Up");
            isRightParry = !isRightParry;
        }
        else
        {
            animationController.CrossFadeAnimation("Parry_L2R_Up");
            isRightParry = !isRightParry;
        }

        if (parryCount >= 3)
            OnPlayParryEffect.Invoke(parryEffectPos.position);
    }

    // 추적 상태 기능 동작 처리 (상태 실행)
    public override void UpdateState()
    {
        if (!animationController.CheckAnimationEnd("Parry", 0.9f, true)) return;

        LookAtTarget(true);

        if (parryCount < 3)
        {
            controller.TransactionToState((int)EnemyController_LB.STATE.DEFENSE);
        }
        else
        {
            float playerDistance = controller.GetPlayerDistance();

            if (playerDistance <= attackDistance)
            {
                controller.TransactionToState((int)EnemyController_LB.STATE.ATTACK);
            }
            else
            {
                controller.TransactionToState((int)EnemyController_LB.STATE.CHASE);
            }

            parryCount = 0;
        }
    }

    public override void ExitState()
    {
        enemyHealth.SetDefense(false);
    }
}
