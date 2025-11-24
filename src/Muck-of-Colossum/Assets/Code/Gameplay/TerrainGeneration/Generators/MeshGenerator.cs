using System;
using Code.Gameplay.TerrainGeneration.StaticData;
using Code.Gameplay.TerrainGeneration.Structures;
using Code.Infrastructure.StaticData;
using UnityEngine;
using UnityEngine.Profiling;

namespace Code.Gameplay.TerrainGeneration.Generators
{
    public class MeshGenerator
    {
        private readonly IStaticDataService staticData;
        private MeshData[] meshDatas;
        private int[,] vertexIndicesMap;

        public MeshGenerator(IStaticDataService staticData)
        {
            this.staticData = staticData;
        }

        public MeshData GenerateTerrainMesh(float[,] heightMap, int levelOfDetail)
        {
            Profiler.BeginSample("MeshGenerator.GenerateTerrainMesh");
            var skipIncrement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
            var numVertsPerLine = staticData.MeshSettings.numVertsPerLine;

            var topLeft = new Vector2(-1, 1) * staticData.MeshSettings.meshWorldSize / 2f;

            if (vertexIndicesMap == null)
                vertexIndicesMap = new int[numVertsPerLine, numVertsPerLine];
            
            var meshVertexIndex = 0;
            var outOfMeshVertexIndex = -1;

            for (var y = 0; y < numVertsPerLine; y++)
            for (var x = 0; x < numVertsPerLine; x++)
            {
                var isOutOfMeshVertex = y == 0 || y == numVertsPerLine - 1 || x == 0 || x == numVertsPerLine - 1;
                var isSkippedVertex = x > 2 && x < numVertsPerLine - 3 && y > 2 && y < numVertsPerLine - 3 &&
                                      ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);
                if (isOutOfMeshVertex)
                {
                    vertexIndicesMap[x, y] = outOfMeshVertexIndex;
                    outOfMeshVertexIndex--;
                }
                else if (!isSkippedVertex)
                {
                    vertexIndicesMap[x, y] = meshVertexIndex;
                    meshVertexIndex++;
                }
            }

            if (meshDatas == null)
            {
                meshDatas = new MeshData[staticData.MeshSettings.detailLevels.Length];
                for (int i = 0; i < staticData.MeshSettings.detailLevels.Length; i++)
                {
                    var skipInc = staticData.MeshSettings.detailLevels[i].lod == 0 
                        ? 1 
                        : staticData.MeshSettings.detailLevels[i].lod * 2;
                    meshDatas[i] = new MeshData(numVertsPerLine, skipInc);
                }
            }
            
            var meshData = CalculateVertices(
                heightMap, staticData.MeshSettings, numVertsPerLine, skipIncrement, vertexIndicesMap, topLeft);

            meshData.ProcessMesh();
            Profiler.EndSample();
            return meshData;
        }

        private MeshData CalculateVertices(float[,] heightMap, MeshSettings meshSettings, int numVertsPerLine,
            int skipIncrement, int[,] vertexIndicesMap, Vector2 topLeft)
        {
            Profiler.BeginSample("MeshGenerator.CalculateVertices");
            MeshData meshData = null;
            foreach (var data in meshDatas)
            {
                if (data.skipIncrement == skipIncrement)
                {
                    meshData = data;
                    meshData.Reuse();
                    break;
                }
            }
            
            for (var y = 0; y < numVertsPerLine; y++)
            for (var x = 0; x < numVertsPerLine; x++)
            {
                var isSkippedVertex = x > 2 && x < numVertsPerLine - 3 && y > 2 && y < numVertsPerLine - 3 &&
                                      ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);

                if (!isSkippedVertex)
                {
                    var isOutOfMeshVertex = y == 0 || y == numVertsPerLine - 1 || x == 0 || x == numVertsPerLine - 1;
                    var isMeshEdgeVertex = (y == 1 || y == numVertsPerLine - 2 || x == 1 || x == numVertsPerLine - 2) &&
                                           !isOutOfMeshVertex;
                    var isMainVertex = (x - 2) % skipIncrement == 0 && (y - 2) % skipIncrement == 0 && !isOutOfMeshVertex &&
                                       !isMeshEdgeVertex;
                    var isEdgeConnectionVertex =
                        (y == 2 || y == numVertsPerLine - 3 || x == 2 || x == numVertsPerLine - 3) && !isOutOfMeshVertex &&
                        !isMeshEdgeVertex && !isMainVertex;

                    var vertexIndex = vertexIndicesMap[x, y];
                    var percent = new Vector2(x - 1, y - 1) / (numVertsPerLine - 3);
                    var vertexPosition2D = topLeft + new Vector2(percent.x, -percent.y) * meshSettings.meshWorldSize;
                    var height = heightMap[x, y];

                    if (isEdgeConnectionVertex)
                    {
                        var isVertical = x == 2 || x == numVertsPerLine - 3;
                        var dstToMainVertexA = (isVertical ? y - 2 : x - 2) % skipIncrement;
                        var dstToMainVertexB = skipIncrement - dstToMainVertexA;
                        var dstPercentFromAToB = dstToMainVertexA / (float)skipIncrement;

                        var heightMainVertexA = heightMap[isVertical ? x : x - dstToMainVertexA,
                            isVertical ? y - dstToMainVertexA : y];
                        var heightMainVertexB = heightMap[isVertical ? x : x + dstToMainVertexB,
                            isVertical ? y + dstToMainVertexB : y];

                        height = heightMainVertexA * (1 - dstPercentFromAToB) + heightMainVertexB * dstPercentFromAToB;
                    }

                    meshData.AddVertex(new Vector3(vertexPosition2D.x, height, vertexPosition2D.y), percent, vertexIndex);

                    var createTriangle = x < numVertsPerLine - 1 && y < numVertsPerLine - 1 &&
                                         (!isEdgeConnectionVertex || (x != 2 && y != 2));

                    if (createTriangle)
                    {
                        var currentIncrement = isMainVertex && x != numVertsPerLine - 3 && y != numVertsPerLine - 3
                            ? skipIncrement
                            : 1;

                        var a = vertexIndicesMap[x, y];
                        var b = vertexIndicesMap[x + currentIncrement, y];
                        var c = vertexIndicesMap[x, y + currentIncrement];
                        var d = vertexIndicesMap[x + currentIncrement, y + currentIncrement];
                        meshData.AddTriangle(a, d, c);
                        meshData.AddTriangle(d, a, b);
                    }
                }
            }
            Profiler.EndSample();
            return meshData;
        }
    }
}