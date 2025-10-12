using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController _controller;
    public float Speed = 5.0f;
    public Transform orientation;
    private Vector3 _velocity;
    public float Gravity = -9.81f;
    public float JumpHeight = 2f;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }
    
    void Update()
    {
        // Get input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        // Calculate movement direction relative to where the player is facing
        Vector3 moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
        // Move the character
        _controller.Move(moveDirection * Time.deltaTime * Speed);
        
        _velocity.y += Gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
        
        if (Input.GetButtonDown("Jump") && _controller.isGrounded)
            _velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
    }
}
