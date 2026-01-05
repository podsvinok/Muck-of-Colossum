using Code.Gameplay.Player.StateMachine.States.RotationLogic;
using UnityEngine;

namespace Code.Gameplay.Climbing_System
{
    [RequireComponent(typeof(CharacterController))]
    public class ClimbDetector : MonoBehaviour
    {
        [SerializeField] private Camera ClimbCamera;
    
        [Header("Detection")]
        [SerializeField] private float rayDistance = 2f;
        [SerializeField] private LayerMask climbableMask;
        [SerializeField] private float moveSpeed = 0.5f;
        [SerializeField] private float rotationSpeed = 10f;

        private MeshCollider targetCollider;
        private Mesh targetMesh;
        private int triangleIndex = -1;
        private Vector3 baryCoords;
        private bool attached = false;
        private IRotationStrategy rotationStrategy;

        private Vector3 rotationDirection;

        private SurfaceNavigator navigator;

        private void Start()
        {
            rotationStrategy = new ClimbRotation();
            TryAttach();
        }
    
        void Update()
        {
        
            if (!attached)
            {
                if (Input.GetKeyDown(KeyCode.X))
                {
                    TryAttach();
                }
            }
            else
            {
                HandleMovement();
            }
        }

        // ---- Публичный API ----
        public void Detach()
        {
            attached = false;
            targetCollider = null;
            targetMesh = null;
            triangleIndex = -1;
            navigator = null;
        }

        // ---- Вспомогательные методы ----

        private void TryAttach()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, rayDistance, climbableMask))
            {
                if (ValidateHit(hit))
                {
                    AttachToHit(hit);
                }
            }
        
        }

        private bool ValidateHit(RaycastHit hit)
        {
            return hit.collider is MeshCollider mc && mc.sharedMesh != null;
        }

        private void AttachToHit(RaycastHit hit)
        {
            targetCollider = hit.collider as MeshCollider;
            targetMesh = targetCollider.sharedMesh;
            triangleIndex = hit.triangleIndex;
            baryCoords = hit.barycentricCoordinate;
            attached = true;
        
            targetCollider.GetComponent<BakeMesh>()?.ForceUpdateCollider();
        
            // Берём TriangleAdjacency с поверхности
            TriangleAdjacency adjacency = targetCollider.GetComponentInChildren<TriangleAdjacency>();
            if (adjacency != null)
            {
                adjacency.RebuildFromMesh(targetCollider.sharedMesh, 1e-6f);
            
                navigator = new SurfaceNavigator( targetCollider, 
                    targetCollider.transform, 
                    adjacency.Neighbors, 
                    adjacency.VertexToTriangles,
                    adjacency.Remap, moveSpeed);
            
                if (navigator != null)
                {
                    if (adjacency.Remap == null || adjacency.Remap.Length == 0)
                        Debug.LogError("Remap пустой");

                    if (adjacency.Remap.Length < targetMesh.vertexCount)
                        Debug.LogWarning($"Remap.Length ({adjacency.Remap.Length}) < baked vertex count ({targetMesh.vertexCount})");
                }
            }
            else
            {
                Debug.LogError("На целевом меше нет TriangleAdjacency!");
                Debug.Log(hit.collider.name);
            }
        }

        private void HandleMovement()
        {
            if (navigator == null || triangleIndex < 0) return;

            Vector3 moveDir = Vector3.zero;
        
            if (Input.GetKey(KeyCode.W)) moveDir += transform.forward;
            if (Input.GetKey(KeyCode.S)) moveDir -= transform.forward;

            moveDir.Normalize();

            int _nextTri = triangleIndex;
            int _oldTri = triangleIndex;
        
            // Делаем шаг по поверхности
            if (moveDir != Vector3.zero)
            {
                if (navigator.StepForward(triangleIndex, GetWorldPointOnTriangle(), moveDir,
                        out int nextTri, out Vector3 nextPos))
                {
                    _nextTri = nextTri;
                    triangleIndex = nextTri;
                    baryCoords = WorldToBarycentric(nextTri, nextPos);
                }
            }
            Vector3 surfaceNormal = GetTriangleNormal(triangleIndex);
            Vector3 surfaceRight = GetSurfaceRight(surfaceNormal);

            if (Input.GetKey(KeyCode.D)) rotationDirection += transform.right;
            if (Input.GetKey(KeyCode.A)) rotationDirection -= transform.right;
        
        
            HandleUpdateAttachRotation(_oldTri, _nextTri, rotationDirection);

            // Обновляем мировую позицию через барицентрики
            Vector3 worldPoint = GetWorldPointOnTriangle();
            transform.position = worldPoint;
        }

        private void HandleUpdateAttachRotation(int _oldTri, int _nextTri, Vector3 rotaionDirection)
        {
            Vector3 oldNormal = GetTriangleNormal(_oldTri);
            Vector3 newNormal = GetTriangleNormal(_nextTri);

            // 1. Определяем forward: если игрок нажимает AD (накопилось), берем его
            Vector3 desiredForward = rotationDirection.normalized != Vector3.zero 
                ? rotationDirection.normalized 
                : transform.forward;

            // 2. Проецируем forward на поверхность, чтобы forward не улетал в воздух
            Vector3 forwardOnSurface = Vector3.ProjectOnPlane(desiredForward, newNormal).normalized;

            // Если forward совпал с нормалью (редкий случай), берем крест к up
            if (forwardOnSurface == Vector3.zero)
                forwardOnSurface = Vector3.ProjectOnPlane(transform.up, newNormal).normalized;

            // 3. Создаем ориентацию: направление туда, куда хочет игрок, и вверх по нормали поверхности
            Quaternion targetRot = Quaternion.LookRotation(forwardOnSurface, newNormal);

            // 4. Плавно применяем
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);

            // 5. Сбрасываем направление вращения каждый кадр, чтобы оно накапливалось только по кнопкам
            this.rotationDirection = Vector3.zero;
        }

        // ---- Работа с треугольниками ----

        private Vector3 GetWorldPointOnTriangle()
        {
            GetTriangleVertices(triangleIndex, out Vector3 v0, out Vector3 v1, out Vector3 v2);

            // barycentric интерполяция
            Vector3 localPoint = v0 * baryCoords.x + v1 * baryCoords.y + v2 * baryCoords.z;
            return targetCollider.transform.TransformPoint(localPoint);
        }

        private Vector3 WorldToBarycentric(int triIndex, Vector3 worldPos)
        {
            GetTriangleVertices(triIndex, out Vector3 v0, out Vector3 v1, out Vector3 v2);

            // Переводим в мировые
            v0 = targetCollider.transform.TransformPoint(v0);
            v1 = targetCollider.transform.TransformPoint(v1);
            v2 = targetCollider.transform.TransformPoint(v2);

            // Плоскость и векторы
            Vector3 v0v1 = v1 - v0;
            Vector3 v0v2 = v2 - v0;
            Vector3 v0p = worldPos - v0;

            float d00 = Vector3.Dot(v0v1, v0v1);
            float d01 = Vector3.Dot(v0v1, v0v2);
            float d11 = Vector3.Dot(v0v2, v0v2);
            float d20 = Vector3.Dot(v0p, v0v1);
            float d21 = Vector3.Dot(v0p, v0v2);

            float denom = d00 * d11 - d01 * d01;
            float v = (d11 * d20 - d01 * d21) / denom;
            float w = (d00 * d21 - d01 * d20) / denom;
            float u = 1.0f - v - w;

            return new Vector3(u, v, w);
        }

        private void GetTriangleVertices(int triIndex, out Vector3 v0, out Vector3 v1, out Vector3 v2)
        {
            int i0 = targetMesh.triangles[triIndex * 3 + 0];
            int i1 = targetMesh.triangles[triIndex * 3 + 1];
            int i2 = targetMesh.triangles[triIndex * 3 + 2];

            v0 = targetMesh.vertices[i0];
            v1 = targetMesh.vertices[i1];
            v2 = targetMesh.vertices[i2];
        }
    
        private Vector3 GetTriangleNormal(int triIndex)
        {
            GetTriangleVertices(triIndex, out Vector3 v0, out Vector3 v1, out Vector3 v2);

            v0 = targetCollider.transform.TransformPoint(v0);
            v1 = targetCollider.transform.TransformPoint(v1);
            v2 = targetCollider.transform.TransformPoint(v2);

            return Vector3.Cross(v1 - v0, v2 - v0).normalized;
        }
    
        private Vector3 GetSurfaceRight(Vector3 surfaceNormal)
        {
            // Берём текущий forward персонажа и делаем его ортогональным к поверхности
            Vector3 forwardOnSurface = Vector3.ProjectOnPlane(transform.forward, surfaceNormal).normalized;

            // Перпендикуляр к forward и normal даст вправо вдоль поверхности
            return Vector3.Cross(surfaceNormal, forwardOnSurface).normalized;
        }
    
    }
}
