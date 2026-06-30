using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputComponent : MonoBehaviour
{ public Vector3 MoveInput { get; private set; }
    public Vector3 LastMoveInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool isCrouch { get; private set; }
    public bool isDefense { get; private set; }
    public bool HasMoveInput { get; private set; }
    public float LeftMouseDownTime { get; private set; }
    public bool isInventoryOpen { get; private set; }

    public event System.Action<bool> OnInventoryStateChanged;

    public void OnMoveEvent(InputAction.CallbackContext context)
    {
        Vector3 moveInput = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
        bool hasMoveInput = moveInput.sqrMagnitude > 0.0f;
        if (HasMoveInput && !hasMoveInput)
        {
            LastMoveInput = MoveInput;
        }

        MoveInput = moveInput;
        HasMoveInput = hasMoveInput;
    }

    public void OnCrouchInputEvent(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isCrouch = !isCrouch;
        }
    }

    public void OnDefenseInputEvenet(InputAction.CallbackContext context)
    {
        if ((context.started || context.performed))
        {
            isDefense = true;
        }
        else if (context.canceled)
        {
            isDefense = false;
        }
    }

    public void OnJumpEvent(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            JumpInput = true;            
        }
        else if (context.canceled)
        {
            JumpInput = false;
            isCrouch = false;
        }
    }

    public void OnAttackEvenet(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            LeftMouseDownTime += Time.deltaTime;
        }
        else if (context.canceled)
        {
            LeftMouseDownTime = 0;
        }
    }
    
    public void OnSetInventoryOpen(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isInventoryOpen = !isInventoryOpen;
            OnInventoryStateChanged?.Invoke(isInventoryOpen);
        }
    }
}
