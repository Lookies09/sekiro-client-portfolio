using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class GravitySettings
{
    public float gravity = 20.0f;
    public float groundedGravity = 5.0f;
    public float maxFallSpeed = 40.0f; 
}

[System.Serializable]
public class GroundSettings
{
    public LayerMask groundLayers; 
    public float sphereCastRadius = 0.35f; 
    public float sphereCastDistance = 0.15f;
}

[System.Serializable]
public class MovementSettings
{
    public float acceleration = 25.0f; // 가속 In m/s
    public float decceleration = 25.0f; // 감속 In m/s
    public float maxHorizontalSpeed = 8.0f; // 최고 속도  In m/s
    public float jumpSpeed = 10.0f; // 점프 속도 In meters/second
    public float jumpAbortSpeed = 10.0f; // In meters/second
}

public class PlayerMovement : MonoBehaviour
{
    #region 변수
    public GravitySettings gravitySettings;
    public GroundSettings groundSettings;
    public MovementSettings movementSettings;
    private PlayerAnimationController playerAnimationController;
    private CharacterController cc;
    private PlayerLockOnSystem lockOnSystem;
    private IkFootPlacement footPlacement;

    [SerializeField] private float walkSpeed = 3.4f;
    [SerializeField] private float runSpeed = 9.4f;
    [SerializeField] private float crouchSpeed = 1f;

    public float currentSpeed { get; private set; } = 0;
    private float targetSpeed = 0;
    public float verticalSpeed { get; private set; }

    private Quaternion desired_rotation; // 180도 회전 시 희망 회전값
    private Quaternion targetRotation; // 캐릭터의 목표 회전값

    public Vector3 smoothInput { get; private set; } // 부드럽게 움직이도록 움직임 값 저장 벡터
    public Vector3 direction { get; private set; } // 움직임 방향 벡터
    private Vector3 jumpDirection = Vector3.zero; // 점프 방향
    //public Vector3 VerticalVelocity => cc.velocity.Multiply(0.0f, 1.0f, 0.0f);

    private bool isRun, useDash, isJump = false; // 움직임 관련 bool 
    private bool justWalkedOffALedge;
    private bool jumpInput;
    public bool isCrouch { get; private set; }
    public bool IsGrounded { get; private set; }

    [SerializeField] private PlayerController playerController;

    private float previousSpeed = 0f;
    private bool wasRunning = false;
    private bool wasWalking = false;
    private bool hasStopped = false;


    public Quaternion Desired_rotation { get => desired_rotation; set => desired_rotation = value; }

    #endregion


    #region Awake, Start, Update

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        playerAnimationController = GetComponent<PlayerAnimationController>();
        lockOnSystem = GetComponent<PlayerLockOnSystem>();
        footPlacement = GetComponent<IkFootPlacement>();

        playerController.Init();

        playerController.PlayerMovement = this;
        

        currentSpeed = walkSpeed;
    }

    private void Update()
    {
        playerController.OnCharacterUpdate();        
    }

    private void FixedUpdate()
    {
        Movement(Time.deltaTime);
    }

    #endregion


    #region 메서드
    /// <summary>
    /// 플레이어의 기본적인 움직임과 점프, 180도 회전을 처리하는 메서드
    /// </summary>
    private void Movement(float deltaTime)
    {    
        bool isMoving = direction.magnitude >= 0.1f;
        bool isCrouchOrDefense = isCrouch || playerController.playerInput.isDefense;
        isRun = Input.GetKey(KeyCode.LeftShift);        

        if (isCrouchOrDefense)
        {            
            targetSpeed = crouchSpeed;
        }
        else if (isRun)
        {
            targetSpeed = runSpeed;
        }
        else
        {
            targetSpeed = walkSpeed;
        }

        //StopMovementAnimationPlay(isMoving);


        if (!playerAnimationController.isAnimationBusy || isJump)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * Time.deltaTime);

        if (isMoving)
        {
            if (!isCrouchOrDefense)
            {
                Turn180();                
            }
            HandleDash();
        }
        else
        {
            useDash = false;
            isRun = false;
        }

        smoothInput = Vector3.Lerp(smoothInput, direction, Time.deltaTime * 10f);
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, Time.deltaTime * 30f);

        HandleMovement(deltaTime);
        UpdateVerticalSpeed(deltaTime);
        UpdateGrounded();

        previousSpeed = currentSpeed;

    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }

    public void SetRotation(Quaternion targetRotation)
    {
        this.targetRotation = targetRotation;
    }

    public Quaternion GetRotation()
    {
        return targetRotation;
    }


    /// <summary>
    /// 대쉬 컨트롤
    /// </summary>
    private void HandleDash()
    {
        if (!IsGrounded) return;

        if (!playerAnimationController.isAnimationBusy && !useDash && isRun)
        {
            playerAnimationController.CrossFadeAnimation("Slide");
            useDash = true;
        }
        else if (useDash && !isRun)
        {
            useDash = false;
        }
    }

    private void StopMovementAnimationPlay(bool isMoving)
    {
        wasRunning = previousSpeed == runSpeed;
        wasWalking = previousSpeed == walkSpeed;

        if (!isMoving && !hasStopped && IsGrounded)
        {
            if (wasRunning)
            {
                playerAnimationController.RunEndTrigger();
            }
            else if (wasWalking)
            {
                playerAnimationController.Walk_F_EndTrigger();
            }

            hasStopped = true; // 이미 멈췄음을 표시
        }
        else if (isMoving) // 다시 움직이기 시작하면 리셋
        {
            hasStopped = false;
        }
        else if (!IsGrounded)
        {
            hasStopped = true;
        }
    }

    /// <summary>
    /// cc에 움직임 넣어주는 과정
    /// </summary>
    private void HandleMovement(float deltaTime)
    {
        if (playerAnimationController.isAnimationBusy || !cc.enabled) return;

        Vector3 move = smoothInput * currentSpeed;
        move += verticalSpeed * Vector3.up;
       
        cc.Move(move * deltaTime);
    }

    /// <summary>
    /// 걷거나 뛰는 중 180도 돌기 구현
    /// </summary>
    public void Turn180()
    {
        if (lockOnSystem.FocusEnemy || !IsGrounded) return;

        float turn_Angle = Mathf.Abs(Vector3.SignedAngle(transform.forward, direction, Vector3.up));
        
        if (turn_Angle >= 160f && !playerAnimationController.isAnimationBusy)
        {            
            if (isRun && playerAnimationController.GetCurrentVelocity() >= (4f))
            {
                playerAnimationController.CrossFadeAnimation("RunningTurn");
            }
            else
            {
                playerAnimationController.CrossFadeAnimation("Turn180");
            }
            playerAnimationController.isAnimationBusy = true;

            Vector3 anim_rotation = playerAnimationController.GetAnim_rotation();
            Desired_rotation = Quaternion.Euler(new Vector3(anim_rotation.x, anim_rotation.y + 180, anim_rotation.z));
        }
    }

    /// <summary>
    /// 지면 충돌 보정
    /// </summary>
    private bool CheckGrounded()
    {
        Vector3 spherePosition = transform.position;
        spherePosition.y = transform.position.y + groundSettings.sphereCastRadius - groundSettings.sphereCastDistance;
        bool isGrounded = Physics.CheckSphere(spherePosition, groundSettings.sphereCastRadius, groundSettings.groundLayers, QueryTriggerInteraction.Ignore);

        return isGrounded;
    }

    public void SetJumpInput(bool jumpInput)
    {
        this.jumpInput = jumpInput;
    }

    public void SetCrouchInput(bool isCrouch)
    {
        if (this.isCrouch == isCrouch) return; 
        this.isCrouch = isCrouch;
    }

    private void UpdateGrounded()
    {
        justWalkedOffALedge = false;

        bool isGrounded = CheckGrounded();
        if (IsGrounded && !isGrounded && !jumpInput)
        {
            justWalkedOffALedge = true;
        }

        IsGrounded = isGrounded;
    }


    private void UpdateVerticalSpeed(float deltaTime)
    {
        if (IsGrounded)
        {
            verticalSpeed = -gravitySettings.groundedGravity;

            if (jumpInput)
            {
                verticalSpeed = movementSettings.jumpSpeed;
            }
        }
        else
        {
            if (!jumpInput && verticalSpeed > 0.0f)
            {
                verticalSpeed = Mathf.MoveTowards(verticalSpeed, -gravitySettings.maxFallSpeed, movementSettings.jumpAbortSpeed * deltaTime);
            }
            else if (justWalkedOffALedge)
            {
                verticalSpeed = 0.0f;
            }

            verticalSpeed = Mathf.MoveTowards(verticalSpeed, -gravitySettings.maxFallSpeed, gravitySettings.gravity * deltaTime);

        }
    }

    #endregion
}
