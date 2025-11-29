using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Строит карту соседей треугольников и карту ребро → сосед,
/// учитывая дублированные вершины (позиции схлопываются).
/// </summary>
[RequireComponent(typeof(MeshFilter))]
public class TriangleAdjacency : MonoBehaviour
{
    private Mesh mesh;

    public Dictionary<int, List<int>> Neighbors = new Dictionary<int, List<int>>();
    public Dictionary<(int, int), int> EdgeToNeighbor = new Dictionary<(int, int), int>();
    public Dictionary<int, List<int>> VertexToTriangles = new Dictionary<int, List<int>>();
    public int[] Remap; // Добавляем поле для хранения переназначенных индексов

    void Awake()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        BuildAdjacency(mesh, 1e-6f);
    }
    
    public void RebuildFromMesh(Mesh sourceMesh, float epsilon = 1e-6f)
    {
        mesh = sourceMesh;
        BuildAdjacency(mesh, epsilon);
    }

    private void BuildAdjacency(Mesh mesh, float epsilon)
    {
        Neighbors.Clear();
        EdgeToNeighbor.Clear();
        VertexToTriangles.Clear();

        Vector3[] verts = mesh.vertices;
        int[] tris = mesh.triangles;
        int triCount = tris.Length / 3;

        // === Шаг 1: схлопываем вершины по позициям ===
        Dictionary<Vector3, int> canonical = new Dictionary<Vector3, int>(new Vec3Comparer(epsilon));
        Remap = new int[verts.Length]; // Инициализируем массив Remap

        for (int i = 0; i < verts.Length; i++)
        {
            if (!canonical.TryGetValue(verts[i], out int idx))
            {
                idx = canonical.Count;
                canonical[verts[i]] = idx;
            }
            Remap[i] = idx; // Сохраняем переназначенный индекс
        }

        // === Шаг 2: строим edge->triangles и vertex->triangles ===
        Dictionary<(int, int), List<int>> edgeToTriangles = new();

        // Инициализируем словарь для вершин
        for (int i = 0; i < canonical.Count; i++)
        {
            VertexToTriangles[i] = new List<int>();
        }

        for (int t = 0; t < triCount; t++)
        {
            int i0 = Remap[tris[t * 3 + 0]];
            int i1 = Remap[tris[t * 3 + 1]];
            int i2 = Remap[tris[t * 3 + 2]];

            AddEdge(edgeToTriangles, i0, i1, t);
            AddEdge(edgeToTriangles, i1, i2, t);
            AddEdge(edgeToTriangles, i2, i0, t);

            // Добавляем треугольник в списки вершин
            VertexToTriangles[i0].Add(t);
            VertexToTriangles[i1].Add(t);
            VertexToTriangles[i2].Add(t);
        }

        // === Шаг 3: финальные словари ===
        for (int t = 0; t < triCount; t++)
            Neighbors[t] = new List<int>();

        foreach (var kvp in edgeToTriangles)
        {
            List<int> trisWithEdge = kvp.Value;
            if (trisWithEdge.Count == 2)
            {
                int tA = trisWithEdge[0];
                int tB = trisWithEdge[1];

                Neighbors[tA].Add(tB);
                Neighbors[tB].Add(tA);

                EdgeToNeighbor[kvp.Key] = tB;
                EdgeToNeighbor[(kvp.Key.Item2, kvp.Key.Item1)] = tA;
            }
            else
            {
                // Висячие рёбра можно залогировать для отладки
                 Debug.LogWarning($"Ребро {kvp.Key} принадлежит {trisWithEdge.Count} треугольников");
            }
        }

        Debug.Log($"[TriangleAdjacency] Построено {triCount} треугольников, соседей: {EdgeToNeighbor.Count}");
    }

    private void AddEdge(Dictionary<(int, int), List<int>> dict, int a, int b, int triIndex)
    {
        var edge = a < b ? (a, b) : (b, a);
        if (!dict.TryGetValue(edge, out var list))
        {
            list = new List<int>();
            dict[edge] = list;
        }
        list.Add(triIndex);
    }

    /// <summary>
    /// Компаратор для Vector3 с допуском.
    /// </summary>
    private class Vec3Comparer : IEqualityComparer<Vector3>
    {
        private readonly float epsilon;
        public Vec3Comparer(float eps) { epsilon = eps; }

        public bool Equals(Vector3 a, Vector3 b) =>
            (a - b).sqrMagnitude < epsilon * epsilon;

        public int GetHashCode(Vector3 v) =>
            Mathf.RoundToInt(v.x / epsilon) * 73856093 ^
            Mathf.RoundToInt(v.y / epsilon) * 19349663 ^
            Mathf.RoundToInt(v.z / epsilon) * 83492791;
    }
}