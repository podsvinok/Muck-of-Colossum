using System;
using Code.Gameplay.Levels;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using Zenject;

public class PlayerMove : NetworkBehaviour
{
    public float moveSpeed = 6f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    public CharacterController controller;

    private Transform cameraTransform;
    private Vector3 velocity;
    private bool isGrounded;

    private ILevelDataProvider levelDataProvider;

    [Inject]
    public void Construct(ILevelDataProvider levelDataProvider)
    {
        this.levelDataProvider = levelDataProvider;
    }

    public override void OnStartClient()
    {
        if (!IsOwner)
            return;
        cameraTransform = levelDataProvider.Camera;
    }

    void Update()
    {
        if (!IsOwner)
            return;
        // Check ground
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        if (inputDir.sqrMagnitude > 0.01f)
        {
            // Camera-relative direction
            Vector3 camForward = cameraTransform.forward;
            camForward.y = 0;
            Vector3 camRight = cameraTransform.right;
            camRight.y = 0;

            Vector3 moveDir = camForward.normalized * v + camRight.normalized * h;

            // Rotate player only toward move direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }
    }
}