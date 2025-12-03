using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ClimbPlayerState : MovementPlayerState
{
    protected TriangleMeshHelper triangleHelper;
    
    private readonly ClimbStateConfig config;
    
    public ClimbPlayerState(IStateSwitcher stateSwitcher, PlayerStateMachineData data, Player player) 
        : base(stateSwitcher, data, player)
    {
        config = player.Config.ClimbStateConfig;
    }
    
    public override void Enter()
    {
        base.Enter();
        View.StartClimbing();
        
        // ✅ Ключевое изменение: проверяем ПЕРЕД raycast
        if (!Data.IsClimbing)
        {
            // Первое прикрепление - делаем raycast
            if (!climbChecker.TryGetHitInfo(out RaycastHit hitInfo))
            {
                Debug.LogError("Failed to get climb hit info!");
                return;
            }
            
            InitializeClimbData(hitInfo);
            AttachCharacterToSurface();
            Data.IsClimbing = true;
        }
        else
        {
            // Переход между состояниями - НЕ делаем raycast!
            RestoreTriangleHelper();
        }
        
        // Общая настройка для всех состояний
        movementDirectionHandler = new ClimbMovementDirection();
        rotationStrategy = new ClimbRotation();
        Data.Speed = config.MoveSpeed;
        Data.RotationSpeed = config.RotationSpeed;
        Data.YVelocity = 0f;
        
        OnClimbStateEnter();
    }

    public override void Exit()
    {
        base.Exit();
        View.StopClimbing();
    }

    public override void Update()
    {
        base.Update();
        
    }

    protected virtual void OnClimbStateEnter() { }

    protected void MaintainSurfaceAttachment()
    {
        Vector3 worldPoint = triangleHelper.GetWorldPointOnTriangle(
            Data.ClimbTriangleIndex,
            Data.ClimbBarycentricCoords
        );
        
        CharacterController.transform.position = worldPoint;

        Vector3 surfaceNormal = triangleHelper.GetTriangleNormal(Data.ClimbTriangleIndex);
        rotationContext.OldNormal = surfaceNormal;
        rotationContext.NewNormal = surfaceNormal;
    }

    protected void HandleClimbMovement()
    {
        Vector3 currentWorldPos = triangleHelper.GetWorldPointOnTriangle(
            Data.ClimbTriangleIndex, 
            Data.ClimbBarycentricCoords);
    
        Vector3 moveDirection = movementDirectionHandler.GetConvertedVelocity(movementDirectionContext);
    
        if (moveDirection != Vector3.zero)
        {
            rotationContext.OldNormal = triangleHelper.GetTriangleNormal(Data.ClimbTriangleIndex);
        
            if (Data.ClimbNavigator.StepForward(
                    Data.ClimbTriangleIndex, 
                    currentWorldPos, 
                    moveDirection, 
                    out int nextTriangle, 
                    out Vector3 nextWorldPos))
            {
                Data.ClimbTriangleIndex = nextTriangle;
                Data.ClimbBarycentricCoords = triangleHelper.WorldToBarycentric(nextTriangle, nextWorldPos);
            
                Vector3 newNormal = triangleHelper.GetTriangleNormal(nextTriangle);
                rotationContext.NewNormal = newNormal;
            }
        }
    
        // ВАЖНО: Обновляем позицию ВСЕГДА, независимо от результата StepForward!
        // Это гарантирует, что персонаж остается приклеенным к анимированному мешу
        Vector3 finalWorldPos = triangleHelper.GetWorldPointOnTriangle(
            Data.ClimbTriangleIndex, 
            Data.ClimbBarycentricCoords);
    
        CharacterController.transform.position = finalWorldPos;
    }


    protected void HandleClimbRotation()
    {
        Transform character = CharacterController.transform;
        Quaternion targetRotation = rotationStrategy.GetTargetRotation(rotationContext);
        
        character.rotation = Quaternion.Slerp(
            character.rotation,
            targetRotation,
            Time.deltaTime * Data.RotationSpeed
        );
    }

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();
        Input.Movement.Jump.started += OnJumpKeyPressed;
        Input.Movement.Climb.started += OnClimbKeyPressed;
    }

    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();
        Input.Movement.Jump.started -= OnJumpKeyPressed;
        Input.Movement.Climb.started -= OnClimbKeyPressed;
    }
    
    protected void DetachFromSurface()
    {
        Data.IsClimbing = false;
        triangleHelper = null;
    }

    protected override void OnClimbKeyPressed(InputAction.CallbackContext obj)
    {
        DetachFromSurface();
        StateSwitcher.SwitchState<FallingPlayerState>();
    }
    
    /// <summary>
    /// Инициализирует данные о треугольнике при ПЕРВОМ прикреплении.
    /// Вызывается только когда Data.IsClimbing == false.
    /// </summary>
    private void InitializeClimbData(RaycastHit hitInfo)
    {
        MeshCollider targetCollider = hitInfo.collider as MeshCollider;
        
        if (targetCollider == null || targetCollider.sharedMesh == null)
        {
            Debug.LogError("Invalid climb collider!");
            return;
        }

        // ✅ Сохраняем данные из первого raycast
        Data.ClimbTargetCollider = targetCollider;
        Data.ClimbTargetMesh = targetCollider.sharedMesh;
        Data.ClimbTriangleIndex = hitInfo.triangleIndex;
        Data.ClimbBarycentricCoords = hitInfo.barycentricCoordinate;

        triangleHelper = new TriangleMeshHelper(targetCollider);

        targetCollider.GetComponent<BakeMesh>()?.ForceUpdateCollider();
        InitializeSurfaceNavigator(targetCollider);
        
        rotationContext.TriangleMeshHelper = triangleHelper;
    }

    /// <summary>
    /// Восстанавливает triangleHelper при переходе между состояниями.
    /// НЕ делает raycast, НЕ изменяет Data.ClimbTriangleIndex/BarycentricCoords.
    /// </summary>
    private void RestoreTriangleHelper()
    {
        if (Data.ClimbTargetCollider == null || Data.ClimbTargetMesh == null)
        {
            Debug.LogWarning("Cannot restore triangle helper - collider or mesh is null!");
            return;
        }

        // ✅ Только восстанавливаем helper, данные остаются в Data неизменными
        triangleHelper = new TriangleMeshHelper(Data.ClimbTargetCollider);
        rotationContext.TriangleMeshHelper = triangleHelper;
    }

    private void InitializeSurfaceNavigator(MeshCollider targetCollider)
    {
        TriangleAdjacency adjacency = targetCollider.GetComponentInChildren<TriangleAdjacency>();
        
        if (adjacency == null)
        {
            Debug.LogError("TriangleAdjacency component not found!");
            return;
        }

        if (adjacency.Neighbors == null || adjacency.Neighbors.Count == 0)
        {
            adjacency.RebuildFromMesh(targetCollider.sharedMesh, 1e-6f);
        }

        Data.ClimbNavigator = new SurfaceNavigator(
            targetCollider,
            targetCollider.transform,
            adjacency.Neighbors,
            adjacency.VertexToTriangles,
            adjacency.Remap,
            config.MoveSpeed
        );
    }

    private void AttachCharacterToSurface()
    {
        Vector3 surfacePoint = triangleHelper.GetWorldPointOnTriangle(
            Data.ClimbTriangleIndex,
            Data.ClimbBarycentricCoords
        );
        
        Vector3 surfaceNormal = triangleHelper.GetTriangleNormal(Data.ClimbTriangleIndex);
        rotationContext.OldNormal = surfaceNormal;
        rotationContext.NewNormal = surfaceNormal;

        //Vector3 offset = surfaceNormal * 0.01f;

        CharacterController.enabled = false;
        CharacterController.transform.position = surfacePoint;
        CharacterController.enabled = true;

        Vector3 forwardOnSurface = Vector3.ProjectOnPlane(
            CharacterController.transform.forward, 
            surfaceNormal
        ).normalized;

        if (forwardOnSurface.sqrMagnitude < 0.0001f)
        {
            forwardOnSurface = Vector3.ProjectOnPlane(Vector3.forward, surfaceNormal).normalized;
        }

        CharacterController.transform.rotation = Quaternion.LookRotation(forwardOnSurface, surfaceNormal);
    }

    private void OnJumpKeyPressed(InputAction.CallbackContext obj)
    {
        DetachFromSurface();
        StateSwitcher.SwitchState<JumpingPlayerState>();
    }
}
