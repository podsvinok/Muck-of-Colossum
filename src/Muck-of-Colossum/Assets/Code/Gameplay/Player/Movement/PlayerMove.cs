using Code.Infrastructure.Inputs;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Player
{
    public class PlayerMove : NetworkBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private float movementSpeed = 5f;
        [SerializeField] private float rotationSpeed = 5f;

        private IInputService input;

        public void Construct(IInputService input)
        {
            this.input = input;
        }
        
        private void Update()
        {
            Movement();
            Turn();
        }

        private void Turn()
        {
            if (Mathf.Abs(input.GetVerticalAxis()) > 0 || Mathf.Abs(input.GetHorizontalAxis()) > 0)
            {
                Vector3 currentLookDirection = characterController.velocity.normalized;
                currentLookDirection.y = 0;
                
                currentLookDirection.Normalize();
                
                Quaternion targetRotation = Quaternion.LookRotation(currentLookDirection);
                
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        private void Movement()
        {
            var move = new Vector3(
                input.GetHorizontalAxis() * movementSpeed, 0, input.GetVerticalAxis() * rotationSpeed);
            
            characterController.Move(move * Time.deltaTime);
        }
    }
}