using Code.Infrastructure.Inputs;
using FishNet.Object;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Player.Movement
{
    public class PlayerMove : NetworkBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Camera playerCamera;
        
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float jumpForce = 1f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float groundCheckDistance = 0.2f;
        [SerializeField] private float lookSensitivity = 2f;
        [SerializeField] private float maxLookAngle = 80f;
    
        private Vector3 velocity;
        private float verticalRotation;
        private IInputService input;

        [Inject]
        public void Construct(IInputService input)
        {
            this.input = input;
        }
        
        public override void OnStartClient()
        {
            if (!IsOwner)
            {
                Destroy(playerCamera.gameObject);
                enabled = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void Update()
        {
            if (!characterController.enabled)
                return;
            HandleMovement();
            HandleRotation();
        }

        private void HandleMovement()
        {
            bool isGrounded = true;//IsGrounded();
            if (isGrounded && velocity.y < 0) 
                velocity.y = -2f;

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
            moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
            characterController.Move(moveDirection * (currentSpeed * Time.deltaTime));

            if (input.GetJumpButtonUp() && isGrounded) 
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);

            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }

        private void HandleRotation()
        {
            float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
            playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

            transform.Rotate(Vector3.up * mouseX);
        }

        private bool IsGrounded() => 
            Physics.Raycast(transform.position + Vector3.up * 0.03f, Vector3.down, groundCheckDistance);
    }
}