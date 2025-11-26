using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Code.Gameplay.TerrainGeneration.Jobs
{
    [BurstCompile]
    public struct GenerateMeshJob : IJob
    {
        //Output
        public NativeList<float3> Vertices;
        public NativeList<int> Triangles;
        public NativeList<float2> UVs;
        
        //Input
        [ReadOnly] public NativeArray<float> HeightMap;
        public int NumVertsPerLine;   
        public int SkipIncrement;    
        public float MeshWorldSize;   
        public float TopLeftX;        
        public float TopLeftZ;

        public void Execute()
        {
            int width = NumVertsPerLine;
            int meshVertexIndex = 0;
            
            NativeArray<int> vertexIndicesMap = new NativeArray<int>(width * width, Allocator.Temp);

            //Generate Vertices
            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool isOutOfMeshVertex = y == 0 || y == width - 1 || x == 0 || x == width - 1;
                    bool isSkippedVertex = x > 2 && x < width - 3 && y > 2 && y < width - 3 &&
                                           ((x - 2) % SkipIncrement != 0 || (y - 2) % SkipIncrement != 0);
                    if (isOutOfMeshVertex)
                    {
                        vertexIndicesMap[y * width + x] = meshVertexIndex;
                        meshVertexIndex++;
                    }
                    else if (!isSkippedVertex)
                    {
                        vertexIndicesMap[y * width + x] = meshVertexIndex;
                        meshVertexIndex++;
                    }
                    else
                    {
                        vertexIndicesMap[y * width + x] = -1;
                        continue;
                    }

                    //Vertex Position Logic
                    bool isMeshEdgeVertex = (y == 1 || y == width - 2 || x == 1 || x == width - 2) && !isOutOfMeshVertex;
                    bool isMainVertex = (x - 2) % SkipIncrement == 0 && (y - 2) % SkipIncrement == 0 && !isOutOfMeshVertex && !isMeshEdgeVertex;
                    bool isEdgeConnectionVertex = (y == 2 || y == width - 3 || x == 2 || x == width - 3) && !isOutOfMeshVertex && !isMeshEdgeVertex && !isMainVertex;
                    
                    float2 percent = new float2(x - 1, y - 1) / (width - 3);
                    float2 vertexPosition2D = new float2(TopLeftX, TopLeftZ) + new float2(percent.x, -percent.y) * MeshWorldSize;
                    
                    float height = HeightMap[y * width + x];

                    //Stitching Logic
                    if (isEdgeConnectionVertex)
                    {
                        bool isVertical = x == 2 || x == width - 3;
                        
                        int dstToMainVertexA = (isVertical ? y - 2 : x - 2) % SkipIncrement;
                        int dstToMainVertexB = SkipIncrement - dstToMainVertexA;
                        float dstPercentFromAToB = dstToMainVertexA / (float)SkipIncrement;

                        float heightMainVertexA = HeightMap[(isVertical ? y - dstToMainVertexA : y) * width + (isVertical ? x : x - dstToMainVertexA)];
                        float heightMainVertexB = HeightMap[(isVertical ? y + dstToMainVertexB : y) * width + (isVertical ? x : x + dstToMainVertexB)];

                        height = heightMainVertexA * (1 - dstPercentFromAToB) + heightMainVertexB * dstPercentFromAToB;
                    }

                    Vertices.Add(new float3(vertexPosition2D.x, height, vertexPosition2D.y));
                    UVs.Add(percent);
                }
            }

            //Generate Triangles
            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool isSkippedVertex = x > 2 && x < width - 3 && y > 2 && y < width - 3 &&
                                           ((x - 2) % SkipIncrement != 0 || (y - 2) % SkipIncrement != 0);
                    if (isSkippedVertex) continue;
                    
                    bool isOutOfMeshVertex = y == 0 || y == width - 1 || x == 0 || x == width - 1;
                    bool isMeshEdgeVertex = (y == 1 || y == width - 2 || x == 1 || x == width - 2) && !isOutOfMeshVertex;
                    bool isMainVertex = (x - 2) % SkipIncrement == 0 && (y - 2) % SkipIncrement == 0 && !isOutOfMeshVertex && !isMeshEdgeVertex;
                    bool isEdgeConnectionVertex = (y == 2 || y == width - 3 || x == 2 || x == width - 3) && !isOutOfMeshVertex && !isMeshEdgeVertex && !isMainVertex;

                    bool createTriangle = x < width - 1 && y < width - 1 && (!isEdgeConnectionVertex || (x != 2 && y != 2));
                    if (createTriangle)
                    {
                        int currentIncrement = (isMainVertex && x != width - 3 && y != width - 3) ? SkipIncrement : 1;

                        int a = vertexIndicesMap[y * width + x];
                        int b = vertexIndicesMap[y * width + (x + currentIncrement)];
                        int c = vertexIndicesMap[(y + currentIncrement) * width + x];
                        int d = vertexIndicesMap[(y + currentIncrement) * width + (x + currentIncrement)];

                        if (a != -1 && b != -1 && c != -1 && d != -1)
                        {
                            Triangles.Add(d);
                            Triangles.Add(a);
                            Triangles.Add(b);
                            
                            Triangles.Add(a);
                            Triangles.Add(d);
                            Triangles.Add(c);
                        }
                    }
                }
            }
            vertexIndicesMap.Dispose();
        }
    }
}