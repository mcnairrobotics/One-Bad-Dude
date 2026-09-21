using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }

    public bool JumpPressed { get; private set; }
    public bool FirePressed { get; private set; }
    public bool FireHeld { get; private set; }
    public bool ReloadPressed { get; private set; }
    public bool SpecialPressed { get; private set; }


    // Movement
    public void OnMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }

    // Mouse/Gamepad Look
    public void OnLook(InputAction.CallbackContext context)
    {
        Look = context.ReadValue<Vector2>();
    }

    // Jump
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            JumpPressed = true;
    }
    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed)
            ReloadPressed = true;
    }
    public void OnSpecial(InputAction.CallbackContext context)
    {
        if (context.performed)
            SpecialPressed = true;
    }
    // Fire
    public void OnFire(InputAction.CallbackContext context)
    {
        FireHeld = context.ReadValueAsButton();

        if (context.performed)
            FirePressed = true;
    }

    // Call this at the end of Update()
    public void ClearFrameInput()
    {
        JumpPressed = false;
        FirePressed = false;
        ReloadPressed = false;
        SpecialPressed = false;
    }
}