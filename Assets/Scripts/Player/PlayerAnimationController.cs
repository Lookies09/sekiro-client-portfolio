using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public static class CharacterAnimatorParamId
{
    public static Dictionary<string, int> animationsHash = new Dictionary<string, int>();

    public static readonly int VerticalSpeed = Animator.StringToHash("VerticalSpeed");
    public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");    
    public static readonly int HorizonValue = Animator.StringToHash("HorizonValue");
    public static readonly int VerticalValue = Animator.StringToHash("VerticalValue");
    public static readonly int Velocity = Animator.StringToHash("Velocity");

    public static readonly int ComboType = Animator.StringToHash("ComboType");

    public static readonly int MouseDownTime = Animator.StringToHash("MouseDownTime"); 

    public static readonly int IsBusy = Animator.StringToHash("IsBusy"); 

    public static readonly int IsCrouch = Animator.StringToHash("IsCrouch");

    public static readonly int DefenseTime = Animator.StringToHash("DefenseTime");

    public static readonly int Run_F_End = Animator.StringToHash("Run_F_End"); 
    public static readonly int Walk_F_End = Animator.StringToHash("Walk_F_End"); 

    static CharacterAnimatorParamId()
    {
        animationsHash["RunningTurn"] = Animator.StringToHash("RunningTurn");
        animationsHash["Turn180"] = Animator.StringToHash("Turn180");
        animationsHash["Slide"] = Animator.StringToHash("Slide_BT");
        animationsHash["AttackTrigger"] = Animator.StringToHash("Attack01_1");
        animationsHash["Defense_Hit"] = Animator.StringToHash("DefenseR_Hit01");
        animationsHash["Hit_F"] = Animator.StringToHash("Hit_F_Root");

        animationsHash["Parry_R2L_Up"] = Animator.StringToHash("Parry_R2L_Up");
        animationsHash["Parry_L2R_Up"] = Animator.StringToHash("Parry_L2R_Up");
    }

}
public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private PlayerAttack playerAttack;
    private PlayerMovement playerMovement;
    private PlayerDefense playerDefense;

    [SerializeField] private PlayerInputComponent inputComponent;

    public bool isAnimationBusy{ get; set; } // 애니메이션 바쁜상황 확인

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack= GetComponent<PlayerAttack>();
        playerDefense = GetComponent<PlayerDefense>();
    }

    private void FixedUpdate()
    {
        UpdateState();
    }

    public void UpdateState()
    {
        float jumpSpeed = playerMovement.movementSettings.jumpSpeed;
        float normVerticalSpeed = playerMovement.verticalSpeed.Remap(-jumpSpeed, jumpSpeed, -1.0f, 1.0f);        

        float targetVelocity = playerMovement.smoothInput.magnitude * playerMovement.currentSpeed;
        float animVelocity = animator.GetFloat(CharacterAnimatorParamId.Velocity);

        Vector3 localInput = transform.InverseTransformDirection(playerMovement.smoothInput);
        animator.SetFloat(CharacterAnimatorParamId.VerticalSpeed, normVerticalSpeed);
        animator.SetFloat(CharacterAnimatorParamId.HorizonValue, localInput.x);
        animator.SetFloat(CharacterAnimatorParamId.VerticalValue, localInput.z);
        animator.SetFloat(CharacterAnimatorParamId.Velocity, Mathf.Lerp(animVelocity, targetVelocity, Time.deltaTime * 30f));
        animator.SetFloat(CharacterAnimatorParamId.MouseDownTime, playerAttack.mouseUpTime.Remap(0, playerAttack.mouseUpTime, 0, 1));
        animator.SetFloat(CharacterAnimatorParamId.DefenseTime, playerDefense.defenseTime);

        animator.SetInteger(CharacterAnimatorParamId.ComboType, playerAttack.comboType);

        animator.SetBool(CharacterAnimatorParamId.IsGrounded, playerMovement.IsGrounded);
        animator.SetBool(CharacterAnimatorParamId.IsBusy, isAnimationBusy);
        animator.SetBool(CharacterAnimatorParamId.IsCrouch, playerMovement.isCrouch);
    }
    

    /// <summary>
    /// 애니메이션 바쁨 설정
    /// </summary>
    /// <param name="isBusy">파라미터값을 직접 설정</param>
    public void SetAnimationBusy(bool isBusy)
    {
        isAnimationBusy = isBusy;
    }

    public void SetAnimationNotBusy()
    {
        isAnimationBusy = false;
    }

    public void CrossFadeAnimation(string name)
    {
        int v = CharacterAnimatorParamId.animationsHash[name];

        animator.CrossFade(v, 0.1f);
    }

    public float GetCurrentVelocity()
    {
        return animator.GetFloat(CharacterAnimatorParamId.Velocity);
    }

    public Vector3 GetAnim_rotation()
    {
        return animator.rootRotation.eulerAngles;
    }


    public void RunEndTrigger()
    {
        animator.CrossFade(CharacterAnimatorParamId.Run_F_End, 0.1f);
    }

    public void Walk_F_EndTrigger()
    {
        animator.CrossFade(CharacterAnimatorParamId.Walk_F_End, 0.1f);
    }

    public bool CheckAnimationEnd(string name, float time, bool isTag)
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


