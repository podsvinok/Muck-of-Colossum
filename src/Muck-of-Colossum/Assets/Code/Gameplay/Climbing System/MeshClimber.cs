using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Gameplay.Climbing_System
{
    [RequireComponent(typeof(CharacterController))]
    public class MeshClimber : MonoBehaviour
    {
        [Header("Detection")]
        [SerializeField] private float attachDistance = 2f;
        [SerializeField] private LayerMask climbableMask = ~0;
        [SerializeField] private KeyCode attachKey = KeyCode.X;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField, Tooltip("Время плавного перехода между треугольниками")] private float crossFadeTime = 0.08f;

        [Header("Debug")]
        [SerializeField] private bool drawCurrentTriangle = true;
        [SerializeField] private Color debugTriangleColor = Color.yellow;

        private MeshCollider targetCollider;
        private Mesh targetMesh;
        private TriangleGraph graph;

        private TriangleNode currentTriangle;
        private Vector3 baryCoords; // (u, v, w)

        private bool attached = false;
        private bool isCrossFading = false;

        void Update()
        {
            if (!attached)
            {
                if (Input.GetKeyDown(attachKey))
                    TryAttach();
            }
            else
            {
                if (!isCrossFading)
                    UpdateMovement();

                if (Input.GetKeyDown(attachKey))
                    Detach();
            }
        }

        private void TryAttach()
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, attachDistance, climbableMask))
            {
                if (hit.collider is MeshCollider mc && mc.sharedMesh != null)
                {
                    Debug.Log(mc);
                    targetCollider = mc;
                    targetMesh = mc.sharedMesh;

                    if (graph == null || graph.Mesh != targetMesh)
                        graph = new TriangleGraph(targetMesh);

                    currentTriangle = graph.GetTriangle(hit.triangleIndex);
                    baryCoords = hit.barycentricCoordinate;
                    attached = true;
                }
            }
        }

        public void Detach()
        {
            attached = false;
            targetCollider = null;
            targetMesh = null;
            currentTriangle = null;
        }

        private void UpdateMovement()
        {
            if (currentTriangle == null || targetCollider == null) return;

            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (input == Vector2.zero) return;

            // Мировые вершины и нормаль треугольника
            currentTriangle.GetWorldVertices(targetCollider.transform, out Vector3 w0, out Vector3 w1, out Vector3 w2);
            Vector3 edge1 = w1 - w0;
            Vector3 edge2 = w2 - w0;
            Vector3 normal = Vector3.Cross(edge1, edge2).normalized;

            // --- ГЛОБАЛЬНЫЙ БАЗИС УПРАВЛЕНИЯ ---
            Vector3 worldForward = Vector3.forward;
            Vector3 worldRight = Vector3.right;

            // Вектор движения из ввода игрока
            Vector3 desired = worldForward * input.y + worldRight * input.x;

            // Проекция на плоскость треугольника
            Vector3 moveWorld = Vector3.ProjectOnPlane(desired, normal).normalized * moveSpeed * Time.deltaTime;

            // Считаем новые барицентрические координаты
            Vector3 newBary = MoveBarycentric(baryCoords, moveWorld, currentTriangle, targetCollider.transform, out TriangleNode neighborTri);

            if (neighborTri != null)
            {
                StartCoroutine(CrossFadeToNeighbor(newBary, neighborTri));
            }
            else
            {
                baryCoords = newBary;
                Vector3 worldPos = currentTriangle.BaryToWorld(baryCoords, targetCollider.transform);
                transform.position = worldPos;

                // Поворот игрока в сторону движения (но только в плоскости треугольника)
                if (desired.sqrMagnitude > 1e-6f)
                {
                    Vector3 flatDir = Vector3.ProjectOnPlane(desired, normal).normalized;
                    transform.rotation = Quaternion.LookRotation(flatDir, normal);
                }
            }
        }


        // Плавный переход между треугольниками — явный System.Collections.IEnumerator
        private IEnumerator CrossFadeToNeighbor(Vector3 targetBary, TriangleNode newTri)
        {
            isCrossFading = true;
            float t = 0f;

            // стартовая и целевая мировые позиции
            Vector3 startWorld = currentTriangle.BaryToWorld(baryCoords, targetCollider.transform);
            Vector3 endWorld = newTri.BaryToWorld(targetBary, targetCollider.transform);

            // стартовая и целевая ротации (ориентация по нормали соседнего треугольника)
            currentTriangle.GetWorldVertices(targetCollider.transform, out Vector3 sw0, out Vector3 sw1, out Vector3 sw2);
            Vector3 sNormal = Vector3.Cross(sw1 - sw0, sw2 - sw0).normalized;
            Vector3 sRight = Vector3.ProjectOnPlane(sw1 - sw0, sNormal).normalized;
            Vector3 sForward = Vector3.Cross(sNormal, sRight).normalized;
            Quaternion startRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, sNormal).sqrMagnitude > 1e-6f ? Vector3.ProjectOnPlane(transform.forward, sNormal) : sForward, sNormal);

            newTri.GetWorldVertices(targetCollider.transform, out Vector3 nw0, out Vector3 nw1, out Vector3 nw2);
            Vector3 nNormal = Vector3.Cross(nw1 - nw0, nw2 - nw0).normalized;
            Vector3 nRight = Vector3.ProjectOnPlane(nw1 - nw0, nNormal).normalized;
            Vector3 nForward = Vector3.Cross(nNormal, nRight).normalized;
            Quaternion endRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, nNormal).sqrMagnitude > 1e-6f ? Vector3.ProjectOnPlane(transform.forward, nNormal) : nForward, nNormal);

            while (t < crossFadeTime)
            {
                t += Time.deltaTime;
                float f = Mathf.Clamp01(t / crossFadeTime);
                transform.position = Vector3.Lerp(startWorld, endWorld, f);
                transform.rotation = Quaternion.Slerp(startRot, endRot, f);
                yield return null;
            }

            // завершили переход
            currentTriangle = newTri;
            baryCoords = targetBary;
            isCrossFading = false;
        }

        /// <summary>
        /// Перемещает точку, возвращает новые барицентрики. Если переход через ребро - возвращает соседний треугольник в out.
        /// </summary>
        private Vector3 MoveBarycentric(Vector3 bary, Vector3 moveWorld, TriangleNode tri, Transform meshTr, out TriangleNode newTri)
        {
            newTri = null;

            Vector3 worldPos = tri.BaryToWorld(bary, meshTr);
            Vector3 newWorldPos = worldPos + moveWorld;

            Vector3 newBary = tri.WorldToBary(newWorldPos, meshTr);

            // Найдём минимальную компоненту (вышли ли за треугольник)
            int minIndex = 0;
            float minVal = newBary.x;
            if (newBary.y < minVal) { minVal = newBary.y; minIndex = 1; }
            if (newBary.z < minVal) { minVal = newBary.z; minIndex = 2; }

            if (minVal >= 0f)
            {
                // осталось внутри текущего треугольника
                return newBary;
            }

            // если вышли наружу — ребро, с которым пересечение: edgeIndex = (minIndex + 1) % 3
            int edgeIndex = (minIndex + 1) % 3;
            TriangleNode neighbor = tri.neighbors[edgeIndex];

            if (neighbor != null)
            {
                // пересчитываем барицентрики в соседнем треугольнике
                Vector3 neighborBary = neighbor.WorldToBary(newWorldPos, meshTr);
                newTri = neighbor;
                return neighborBary;
            }
            else
            {
                // соседа нет — зажимаем точку на границе (fallback)
                float ux = Mathf.Max(newBary.x, 0f);
                float uy = Mathf.Max(newBary.y, 0f);
                float uz = Mathf.Max(newBary.z, 0f);
                float sum = ux + uy + uz;
                if (sum <= 1e-6f) return new Vector3(1f / 3f, 1f / 3f, 1f / 3f);
                return new Vector3(ux / sum, uy / sum, uz / sum);
            }
        }

        void OnDrawGizmos()
        {
            if (!drawCurrentTriangle) return;

            if (attached && currentTriangle != null && targetCollider != null)
            {
                currentTriangle.GetWorldVertices(targetCollider.transform, out Vector3 w0, out Vector3 w1, out Vector3 w2);
                Gizmos.color = debugTriangleColor;
                Gizmos.DrawLine(w0, w1);
                Gizmos.DrawLine(w1, w2);
                Gizmos.DrawLine(w2, w0);

                Vector3 worldPos = currentTriangle.BaryToWorld(baryCoords, targetCollider.transform);
                Gizmos.DrawSphere(worldPos, 0.03f);
            }
        }
    }

    #region Вспомогательные классы

    internal struct EdgeKey
    {
        public readonly int a;
        public readonly int b;
        public EdgeKey(int v0, int v1)
        {
            if (v0 < v1) { a = v0; b = v1; } else { a = v1; b = v0; }
        }

        public override bool Equals(object obj)
        {
            return obj is EdgeKey other && a == other.a && b == other.b;
        }

        public override int GetHashCode()
        {
            unchecked { return a * 397 ^ b; }
        }
    }

    internal struct EdgeInfo
    {
        public TriangleNode tri;
        public int edgeIndex;
    }

    internal class TriangleNode
    {
        public int index;
        public int[] vertices; // индексы вершин
        public TriangleNode[] neighbors = new TriangleNode[3];

        private Mesh mesh;

        public TriangleNode(int triIndex, Mesh mesh)
        {
            this.index = triIndex;
            this.mesh = mesh;
            vertices = new int[3]
            {
                mesh.triangles[triIndex * 3 + 0],
                mesh.triangles[triIndex * 3 + 1],
                mesh.triangles[triIndex * 3 + 2]
            };
        }

        public void GetWorldVertices(Transform tr, out Vector3 w0, out Vector3 w1, out Vector3 w2)
        {
            Vector3[] verts = mesh.vertices;
            w0 = tr.TransformPoint(verts[vertices[0]]);
            w1 = tr.TransformPoint(verts[vertices[1]]);
            w2 = tr.TransformPoint(verts[vertices[2]]);
        }

        public Vector3 BaryToWorld(Vector3 bary, Transform tr)
        {
            Vector3[] verts = mesh.vertices;
            Vector3 v0 = verts[vertices[0]];
            Vector3 v1 = verts[vertices[1]];
            Vector3 v2 = verts[vertices[2]];
            Vector3 local = v0 * bary.x + v1 * bary.y + v2 * bary.z;
            return tr.TransformPoint(local);
        }

        public Vector3 WorldToBary(Vector3 worldPos, Transform tr)
        {
            GetWorldVertices(tr, out Vector3 w0, out Vector3 w1, out Vector3 w2);
            return Barycentric(worldPos, w0, w1, w2);
        }

        private Vector3 Barycentric(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 v0 = b - a;
            Vector3 v1 = c - a;
            Vector3 v2 = p - a;
            float d00 = Vector3.Dot(v0, v0);
            float d01 = Vector3.Dot(v0, v1);
            float d11 = Vector3.Dot(v1, v1);
            float d20 = Vector3.Dot(v2, v0);
            float d21 = Vector3.Dot(v2, v1);
            float denom = d00 * d11 - d01 * d01;
            if (Mathf.Abs(denom) < 1e-8f)
            {
                return new Vector3(1f / 3f, 1f / 3f, 1f / 3f);
            }
            float v = (d11 * d20 - d01 * d21) / denom;
            float w = (d00 * d21 - d01 * d20) / denom;
            float u = 1.0f - v - w;
            return new Vector3(u, v, w);
        }
    }

    internal class TriangleGraph
    {
        public Mesh Mesh { get; private set; }
        private TriangleNode[] triangles;

        public TriangleGraph(Mesh mesh)
        {
            Mesh = mesh;
            BuildGraph();
        }

        private void BuildGraph()
        {
            int triCount = Mesh.triangles.Length / 3;
            triangles = new TriangleNode[triCount];

            for (int i = 0; i < triCount; i++)
                triangles[i] = new TriangleNode(i, Mesh);

            var edgeMap = new Dictionary<EdgeKey, EdgeInfo>();

            foreach (var tri in triangles)
            {
                for (int e = 0; e < 3; e++)
                {
                    int v0 = tri.vertices[e];
                    int v1 = tri.vertices[(e + 1) % 3];
                    var key = new EdgeKey(v0, v1);

                    if (edgeMap.TryGetValue(key, out EdgeInfo other))
                    {
                        tri.neighbors[e] = other.tri;
                        other.tri.neighbors[other.edgeIndex] = tri;
                    }
                    else
                    {
                        edgeMap[key] = new EdgeInfo { tri = tri, edgeIndex = e };
                    }
                }
            }
        }

        public TriangleNode GetTriangle(int index)
        {
            if (index < 0 || index >= triangles.Length) return null;
            return triangles[index];
        }
    }

    #endregion
}
