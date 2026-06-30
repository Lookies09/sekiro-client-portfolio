using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(fileName = "PlayerController", menuName = "Controller/PlayerController")]
public class PlayerController : Controller
{
    public float ControlRotationSensitivity = 1.0f;

    public PlayerInputComponent playerInput { get; private set; }

    private PlayerLockOnSystem lockOnSystem;

    public override void Init()
    {
        playerInput = FindFirstObjectByType<PlayerInputComponent>();
        lockOnSystem = FindFirstObjectByType<PlayerLockOnSystem>();
    }

    public override void OnCharacterUpdate()
    {
        UpdateControlRotation();
        PlayerMovement.SetDirection(GetMovementInput());
        PlayerMovement.SetJumpInput(playerInput.JumpInput);
        PlayerMovement.SetCrouchInput(playerInput.isCrouch);
    }

    public override void OnCharacterFixedUpdate()
    {

    }


    private void UpdateControlRotation()
    {
        Quaternion targetRotation;

        if (lockOnSystem.FocusEnemy)
        {
            Vector3 targetDir = (lockOnSystem.FocusEnemy.transform.position - PlayerMovement.transform.position).normalized;
            targetDir.y = 0;

            if (targetDir.sqrMagnitude > 0.0001f)
                targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);
            else
                return; // 0 벡터면 회전하지 않음
        }
        else
        {
            float camY = Camera.main.transform.rotation.eulerAngles.y;
            Vector3 rote = Quaternion.Euler(0, camY, 0) * playerInput.MoveInput;

            if (rote.sqrMagnitude > 0.0001f)
                targetRotation = Quaternion.LookRotation(rote, Vector3.up);
            else
                return; // 0 벡터면 회전하지 않음
        }

        PlayerMovement.SetRotation(targetRotation);
    }


    private Vector3 GetMovementInput()
    {
        Vector3 dir = playerInput.MoveInput;

        Quaternion yawRotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
        dir = yawRotation * dir;
        dir.Normalize();

        return dir;
    }
}
