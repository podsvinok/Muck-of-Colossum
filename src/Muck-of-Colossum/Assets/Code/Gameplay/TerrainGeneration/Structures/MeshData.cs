using System;
using GameKit.Dependencies.Utilities;
using UnityEngine;
using UnityEngine.Profiling;

namespace Code.Gameplay.TerrainGeneration.Structures
{
    public class MeshData
    {
        private Vector3[] vertices;
        private int[] triangles;
        private Vector2[] uvs;
        private Vector3[] bakedNormals;

        private Vector3[] outOfMeshVertices;
        private int[] outOfMeshTriangles;

        private int triangleIndex;
        private int outOfMeshTriangleIndex;

        public int skipIncrement;

        public MeshData(int numVertsPerLine, int skipIncrement)
        {
            this.skipIncrement = skipIncrement;

            var numMeshEdgeVertices = (numVertsPerLine - 2) * 4 - 4;
            var numEdgeConnectionVertices = (skipIncrement - 1) * (numVertsPerLine - 5) / skipIncrement * 4;
            var numMainVerticesPerLine = (numVertsPerLine - 5) / skipIncrement + 1;
            var numMainVertices = numMainVerticesPerLine * numMainVerticesPerLine;

            vertices = new Vector3[numMeshEdgeVertices + numEdgeConnectionVertices + numMainVertices];
            uvs = new Vector2[vertices.Length];

            var numMeshEdgeTriangles = 8 * (numVertsPerLine - 4);
            var numMainTriangles = (numMainVerticesPerLine - 1) * (numMainVerticesPerLine - 1) * 2;
            triangles = new int[(numMeshEdgeTriangles + numMainTriangles) * 3];

            bakedNormals = new Vector3[vertices.Length];
            outOfMeshVertices = new Vector3[numVertsPerLine * 4 - 4];
            outOfMeshTriangles = new int[24 * (numVertsPerLine - 2)];
        }

        public void Reuse()
        {
            triangleIndex = 0;
            outOfMeshTriangleIndex = 0;
            Array.Clear(vertices, 0, vertices.Length);
            Array.Clear(triangles, 0, triangles.Length);
            Array.Clear(uvs, 0, uvs.Length);
            Array.Clear(bakedNormals, 0, bakedNormals.Length);
            Array.Clear(outOfMeshVertices, 0, outOfMeshVertices.Length);
            Array.Clear(outOfMeshTriangles, 0, outOfMeshTriangles.Length);
        }
        
        public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
        {
            if (vertexIndex < 0)
            {
                outOfMeshVertices[-vertexIndex - 1] = vertexPosition;
            }
            else
            {
                vertices[vertexIndex] = vertexPosition;
                uvs[vertexIndex] = uv;
            }
        }

        public void AddTriangle(int a, int b, int c)
        {
            if (a < 0 || b < 0 || c < 0)
            {
                outOfMeshTriangles[outOfMeshTriangleIndex] = a;
                outOfMeshTriangles[outOfMeshTriangleIndex + 1] = b;
                outOfMeshTriangles[outOfMeshTriangleIndex + 2] = c;
                outOfMeshTriangleIndex += 3;
            }
            else
            {
                triangles[triangleIndex] = a;
                triangles[triangleIndex + 1] = b;
                triangles[triangleIndex + 2] = c;
                triangleIndex += 3;
            }
        }

        private void CalculateNormals()
        {
            Profiler.BeginSample("MeshData.CalculateNormals");
            var triangleCount = triangles.Length / 3;
            for (var i = 0; i < triangleCount; i++)
            {
                var normalTriangleIndex = i * 3;
                var vertexIndexA = triangles[normalTriangleIndex];
                var vertexIndexB = triangles[normalTriangleIndex + 1];
                var vertexIndexC = triangles[normalTriangleIndex + 2];

                var triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
                bakedNormals[vertexIndexA] += triangleNormal;
                bakedNormals[vertexIndexB] += triangleNormal;
                bakedNormals[vertexIndexC] += triangleNormal;
            }

            var borderTriangleCount = outOfMeshTriangles.Length / 3;
            for (var i = 0; i < borderTriangleCount; i++)
            {
                var normalTriangleIndex = i * 3;
                var vertexIndexA = outOfMeshTriangles[normalTriangleIndex];
                var vertexIndexB = outOfMeshTriangles[normalTriangleIndex + 1];
                var vertexIndexC = outOfMeshTriangles[normalTriangleIndex + 2];

                var triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
                if (vertexIndexA >= 0) bakedNormals[vertexIndexA] += triangleNormal;
                if (vertexIndexB >= 0) bakedNormals[vertexIndexB] += triangleNormal;
                if (vertexIndexC >= 0) bakedNormals[vertexIndexC] += triangleNormal;
            }
            
            for (var i = 0; i < bakedNormals.Length; i++) 
                bakedNormals[i].Normalize();
            Profiler.EndSample();
        }

        private Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
        {
            var pointA = indexA < 0 ? outOfMeshVertices[-indexA - 1] : vertices[indexA];
            var pointB = indexB < 0 ? outOfMeshVertices[-indexB - 1] : vertices[indexB];
            var pointC = indexC < 0 ? outOfMeshVertices[-indexC - 1] : vertices[indexC];

            var sideAB = pointB - pointA;
            var sideAC = pointC - pointA;
            
            return Vector3.Cross(sideAB, sideAC).normalized;
        }

        public void ProcessMesh()
        {
            CalculateNormals();
        }

        public Mesh CreateMesh()
        {
            Profiler.BeginSample("MeshData.CreateMesh");
            return new Mesh
            {
                vertices = vertices,
                triangles = triangles,
                uv = uvs,
                normals = bakedNormals
            };
            Profiler.EndSample();
        }
    }
}