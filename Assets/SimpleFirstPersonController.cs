using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class SimpleFirstPersonController : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float gravity = -9.81f;
    public float mouseSensitivity = 2f;
    public Transform cameraHolder;
    public float jumpHeight = 1.5f;

    CharacterController cc;
    Vector3 velocity;
    float xRot;

    Vector2 moveInput;
    Vector2 lookInput;
    bool jump;
    bool running;

    void Awake() => cc = GetComponent<CharacterController>();

    // called by Input System
    public void OnMove(InputAction.CallbackContext ctx) => moveInput = ctx.ReadValue<Vector2>();
    public void OnLook(InputAction.CallbackContext ctx) => lookInput = ctx.ReadValue<Vector2>();
    public void OnJump(InputAction.CallbackContext ctx) { if (ctx.performed) jump = true; }
    public void OnRun(InputAction.CallbackContext ctx) => running = ctx.ReadValueAsButton();

    void Update()
    {
        Look();
        Move();
    }

    void Look()
    {
        float mx = lookInput.x * mouseSensitivity;
        float my = lookInput.y * mouseSensitivity;
        xRot -= my;
        xRot = Mathf.Clamp(xRot, -85f, 85f);
        cameraHolder.localEulerAngles = Vector3.right * xRot;
        transform.Rotate(Vector3.up * mx);
    }

    void Move()
    {
        float targetSpeed = running ? runSpeed : walkSpeed;
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        cc.Move(move * targetSpeed * Time.deltaTime);

        if (cc.isGrounded && velocity.y < 0) velocity.y = -2f;
        if (jump && cc.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jump = false;
        }
        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }
}

