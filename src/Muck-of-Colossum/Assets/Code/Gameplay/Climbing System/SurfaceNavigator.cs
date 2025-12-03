using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ✅ ФИНАЛЬНОЕ РЕШЕНИЕ - Баланс между "не застревать" и "не телепортироваться"
/// 
/// Ключевая идея:
/// - Сначала ищем соседей через adjacency (точно)
/// - Потом через вершины (расширенно, но локально)
/// - НИКОГДА не ищем по всей сцене (исключена общая телепортация)
/// </summary>
public class SurfaceNavigator
{
    private MeshCollider meshCollider;
    private Transform meshTransform;
    private Dictionary<int, List<int>> adjacency;

    private float moveSpeed;
    private float step = 0;
    private float subStep = 0.3f;
    private float maxSaveStepSize = 0.1f;
    private Dictionary<int, List<int>> vertexToTriangles;
    private int[] remap;
    
    // ✅ ОПТИМИЗИРОВАННЫЕ параметры
    private float sharpAngleTolerance = 0.1f;   // Достаточен для острых углов
    private float baryCentricTolerance = 0.01f; // Баланс между гибкостью и точностью
    private int maxNavigationRetries = 4;
    
    private Mesh CurrentMesh => meshCollider.sharedMesh;

    public SurfaceNavigator(MeshCollider meshCollider, Transform meshTransform,
        Dictionary<int, List<int>> adjacency,
        Dictionary<int, List<int>> vertexToTriangles,
        int[] remap, float moveSpeed)
    {
        this.meshCollider = meshCollider;
        this.meshTransform = meshTransform;
        this.adjacency = adjacency;
        this.vertexToTriangles = vertexToTriangles;
        this.remap = remap;
        this.moveSpeed = moveSpeed;
    }

    public bool StepForward(
        int currentTriangle,
        Vector3 worldPos,
        Vector3 worldMove,
        out int nextTriangle,
        out Vector3 nextWorldPos)
    {
        nextTriangle = currentTriangle;
        nextWorldPos = worldPos;

        if (worldMove == Vector3.zero)
            return false;

        step = moveSpeed * Time.deltaTime;
        int stepCount = StepDivision(out float remainder);

        for (int i = 0; i < stepCount; i++)
        {
            subStep = maxSaveStepSize;

            if (!AttemptStep(ref currentTriangle, ref worldPos, worldMove, subStep,
                    out nextTriangle, out nextWorldPos))
            {
                return false;
            }

            currentTriangle = nextTriangle;
            worldPos = nextWorldPos;
        }

        if (remainder > 0)
        {
            subStep = remainder;
            if (!AttemptStep(ref currentTriangle, ref worldPos, worldMove, subStep,
                    out nextTriangle, out nextWorldPos))
            {
                return false;
            }
            currentTriangle = nextTriangle;
            worldPos = nextWorldPos;
        }

        return true;
    }

    private bool AttemptStep(ref int currentTriangle, ref Vector3 worldPos, Vector3 moveDir,
        float stepLength, out int nextTriangle, out Vector3 nextWorldPos)
    {
        nextTriangle = currentTriangle;
        nextWorldPos = worldPos;

        float currentStepLength = stepLength;

        for (int retry = 0; retry < maxNavigationRetries; retry++)
        {
            if (DoSubStep(ref currentTriangle, ref worldPos, moveDir, currentStepLength,
                    out nextTriangle, out nextWorldPos))
            {
                return true;
            }

            currentStepLength *= 0.5f;

            if (currentStepLength < 0.001f)
            {
                nextWorldPos = worldPos;
                nextTriangle = currentTriangle;
                return true;
            }
        }

        return false;
    }

    private bool DoSubStep(ref int currentTriangle, ref Vector3 worldPos, Vector3 moveDir,
        float stepLength, out int nextTriangle, out Vector3 nextWorldPos)
    {
        nextTriangle = currentTriangle;
        nextWorldPos = worldPos;

        GetTriangleVertices(currentTriangle, out Vector3 v0, out Vector3 v1, out Vector3 v2);
        v0 = meshTransform.TransformPoint(v0);
        v1 = meshTransform.TransformPoint(v1);
        v2 = meshTransform.TransformPoint(v2);

        Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0).normalized;
        Vector3 moveProjected = Vector3.ProjectOnPlane(moveDir, normal).normalized;

        Vector3 candidate = worldPos + moveProjected * stepLength;
        Vector3 bc = ToBarycentric(candidate, v0, v1, v2);

        // ✅ Достаточная толерантность для острых углов
        if (bc.x >= -baryCentricTolerance && bc.y >= -baryCentricTolerance &&
            bc.z >= -baryCentricTolerance)
        {
            nextWorldPos = candidate;
            return true;
        }

        // ✅ ЛОКАЛЬНЫЙ поиск - только через adjacency и вершины
        int neighbor = FindNeighborLocal(currentTriangle, v0, v1, v2, candidate);
        if (neighbor >= 0)
        {
            nextTriangle = neighbor;
            nextWorldPos = candidate;
            return true;
        }

        return false;
    }

    private int StepDivision(out float remainder)
    {
        int stepDivision = (int)(step / maxSaveStepSize);
        remainder = step % maxSaveStepSize;
        return stepDivision;
    }

    /// <summary>
    /// ✅ ЛОКАЛЬНЫЙ поиск соседей - НЕ ищет по всей сцене!
    /// Только: adjacency → вершины → всё
    /// </summary>
    private int FindNeighborLocal(int currentTriangle, Vector3 v0, Vector3 v1, Vector3 v2,
        Vector3 candidate)
    {
        // ✅ ШАГ 1: Поиск через ADJACENCY (прямые соседи)
        if (adjacency.ContainsKey(currentTriangle))
        {
            foreach (int neighbor in adjacency[currentTriangle])
            {
                GetTriangleVertices(neighbor, out Vector3 n0, out Vector3 n1, out Vector3 n2);
                n0 = meshTransform.TransformPoint(n0);
                n1 = meshTransform.TransformPoint(n1);
                n2 = meshTransform.TransformPoint(n2);

                Vector3 bc = ToBarycentric(candidate, n0, n1, n2);

                if (bc.x >= -sharpAngleTolerance && bc.y >= -sharpAngleTolerance &&
                    bc.z >= -sharpAngleTolerance)
                {
                    return neighbor;
                }
            }
        }

        // ✅ ШАГ 2: Поиск через ВЕРШИНЫ (локальное расширение)
        return FindNeighborByVertices(currentTriangle, candidate);
    }

    /// <summary>
    /// ✅ Поиск через все вершины текущего треугольника
    /// Это гарантирует локальность - ищем только треугольники, 
    /// которые делят вершину с текущим
    /// </summary>
    private int FindNeighborByVertices(int currentTriangle, Vector3 candidate)
    {
        var indices = GetTriangleVertexIndices(currentTriangle);
        var vertexIndices = new[] { indices.i0, indices.i1, indices.i2 };

        int bestNeighbor = -1;
        float bestDistance = float.MaxValue;

        foreach (int vertexIndex in vertexIndices)
        {
            int canonicalIndex = remap[vertexIndex];

            if (vertexToTriangles.TryGetValue(canonicalIndex, out var triangles))
            {
                foreach (int tri in triangles)
                {
                    if (tri == currentTriangle) continue;

                    GetTriangleVertices(tri, out Vector3 n0, out Vector3 n1, out Vector3 n2);
                    n0 = meshTransform.TransformPoint(n0);
                    n1 = meshTransform.TransformPoint(n1);
                    n2 = meshTransform.TransformPoint(n2);

                    Vector3 bc = ToBarycentric(candidate, n0, n1, n2);

                    // ✅ Используем ШирокуюВЫ tolerantance для острых углов
                    if (bc.x >= -sharpAngleTolerance && bc.y >= -sharpAngleTolerance &&
                        bc.z >= -sharpAngleTolerance)
                    {
                        Vector3 triCenter = (n0 + n1 + n2) / 3f;
                        float distToCenter = Vector3.Distance(candidate, triCenter);

                        // ✅ Выбираем БЛИЖАЙШИЙ - это предотвращает случайные прыжки
                        if (distToCenter < bestDistance)
                        {
                            bestDistance = distToCenter;
                            bestNeighbor = tri;
                        }
                    }
                }
            }
        }

        return bestNeighbor;
    }

    private (int i0, int i1, int i2) GetTriangleVertexIndices(int triangleIndex)
    {
        int baseIndex = triangleIndex * 3;
        return (
            CurrentMesh.triangles[baseIndex],
            CurrentMesh.triangles[baseIndex + 1],
            CurrentMesh.triangles[baseIndex + 2]
        );
    }

    private void GetTriangleVertices(int triangleIndex, out Vector3 v0, out Vector3 v1,
        out Vector3 v2)
    {
        var (i0, i1, i2) = GetTriangleVertexIndices(triangleIndex);
        v0 = CurrentMesh.vertices[i0];
        v1 = CurrentMesh.vertices[i1];
        v2 = CurrentMesh.vertices[i2];
    }

    private Vector3 ToBarycentric(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
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
            return new Vector3(1f / 3f, 1f / 3f, 1f / 3f);

        float v = (d11 * d20 - d01 * d21) / denom;
        float w = (d00 * d21 - d01 * d20) / denom;
        float u = 1.0f - v - w;

        return new Vector3(u, v, w);
    }
}