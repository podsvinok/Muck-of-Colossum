using UnityEngine;

namespace Code.Gameplay.Climbing_System
{
    public class TriangleMeshHelper
    {
        private readonly MeshCollider meshCollider;
        private Mesh CurrentMesh => meshCollider.sharedMesh;

        public TriangleMeshHelper(MeshCollider meshCollider)
        {
            this.meshCollider = meshCollider;
        }

        /// <summary>
        /// Получает мировую позицию точки на треугольнике по барицентрическим координатам.
        /// </summary>
        public Vector3 GetWorldPointOnTriangle(int triangleIndex, Vector3 baryCoords)
        {
            GetTriangleVertices(triangleIndex, out Vector3 v0, out Vector3 v1, out Vector3 v2);

            // Барицентрическая интерполяция
            Vector3 localPoint = v0 * baryCoords.x + v1 * baryCoords.y + v2 * baryCoords.z;
            return meshCollider.transform.TransformPoint(localPoint);
        }

        /// <summary>
        /// Конвертирует мировую позицию в барицентрические координаты для заданного треугольника.
        /// </summary>
        public Vector3 WorldToBarycentric(int triIndex, Vector3 worldPos)
        {
            GetTriangleVertices(triIndex, out Vector3 v0, out Vector3 v1, out Vector3 v2);

            // Переводим в мировые координаты
            v0 = meshCollider.transform.TransformPoint(v0);
            v1 = meshCollider.transform.TransformPoint(v1);
            v2 = meshCollider.transform.TransformPoint(v2);

            // Вычисление барицентрических координат
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

        /// <summary>
        /// Получает локальные вершины треугольника.
        /// </summary>
        public void GetTriangleVertices(int triIndex, out Vector3 v0, out Vector3 v1, out Vector3 v2)
        {
            int i0 = CurrentMesh.triangles[triIndex * 3 + 0];
            int i1 = CurrentMesh.triangles[triIndex * 3 + 1];
            int i2 = CurrentMesh.triangles[triIndex * 3 + 2];
            v0 = CurrentMesh.vertices[i0];
            v1 = CurrentMesh.vertices[i1];
            v2 = CurrentMesh.vertices[i2];
        }

        /// <summary>
        /// Получает мировую нормаль треугольника.
        /// </summary>
        public Vector3 GetTriangleNormal(int triIndex)
        {
            GetTriangleVertices(triIndex, out Vector3 v0, out Vector3 v1, out Vector3 v2);

            v0 = meshCollider.transform.TransformPoint(v0);
            v1 = meshCollider.transform.TransformPoint(v1);
            v2 = meshCollider.transform.TransformPoint(v2);

            return Vector3.Cross(v1 - v0, v2 - v0).normalized;
        }

        /// <summary>
        /// Получает направление вправо относительно поверхности.
        /// </summary>
        public Vector3 GetSurfaceRight(Vector3 surfaceNormal, Transform characterTransform)
        {
            // Берём текущий forward персонажа и делаем его ортогональным к поверхности
            Vector3 forwardOnSurface = Vector3.ProjectOnPlane(characterTransform.forward, surfaceNormal).normalized;

            if (forwardOnSurface.sqrMagnitude < 0.0001f)
            {
                forwardOnSurface = Vector3.ProjectOnPlane(characterTransform.up, surfaceNormal).normalized;
            }

            // Перпендикуляр к forward и normal даст вправо вдоль поверхности
            return Vector3.Cross(surfaceNormal, forwardOnSurface).normalized;
        }
    }
}
