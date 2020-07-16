using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chroma.Utility;
using UnityEngine;

namespace Chroma.ColorSystem.Probes.Builder
{
    public static class ColorProbesBuilder
    {
        public static ColorProbe[] GenerateProbes(Mesh mesh, Material material, float clusterSize = 1f, int mipLevel = 5)
        {
            Texture2D texture = material.mainTexture as Texture2D;
            TextureWrapMode wrapMode = texture.wrapMode;
            UnityEngine.Color tint = MaterialUtil.GetMaterialTint(material);
            texture = GraphicsUtil.CopyTexture(texture);

            // Possible improvement: the SplitLargeTriangles method has to calculate the area of
            // each triangle in order to decide if it needs to be split.
            // The cluster reducer algorithm also uses the area of the triangle so it's being calculated
            // twice and that could be avoided if we store the first results.
            Mesh transformedMesh = SplitLargeTriangles(mesh, clusterSize);
            Vector3[] vertices = transformedMesh.vertices;
            Vector2[] uvs = transformedMesh.uv;
            int[] triangles = transformedMesh.triangles;
            int trianglesCount = triangles.Length / 3;
            TriangleData[] trianglesData = new TriangleData[trianglesCount];
            mipLevel = Mathf.Clamp(mipLevel, 1, texture.mipmapCount);

            int textureWidth = Mathf.Max(1, texture.width >> mipLevel);
            int textureHeight = Mathf.Max(1, texture.height >> mipLevel);
            UnityEngine.Color[] pixels = texture.GetPixels(mipLevel);

            Parallel.For(0, trianglesCount, (index) => {
                int currentIndex = index * 3;

                Color averageColor = MeshUtil.GetTriangleAverageColor(
                    uvs[triangles[currentIndex]],
                    uvs[triangles[currentIndex + 1]],
                    uvs[triangles[currentIndex + 2]],
                    pixels,
                    textureWidth,
                    textureHeight,
                    wrapMode,
                    tint);
                Vector3 center = VectorMath.CalculateTriangleCenter(
                    vertices[triangles[currentIndex]],
                    vertices[triangles[currentIndex + 1]],
                    vertices[triangles[currentIndex + 2]]);
                float area = VectorMath.CalculateTriangleArea(
                    vertices[triangles[currentIndex]],
                    vertices[triangles[currentIndex + 1]],
                    vertices[triangles[currentIndex + 2]]);

                TriangleData triangleData = new TriangleData(averageColor, center, area);
                trianglesData[index] = triangleData;
            });

            Dictionary<int, TriangleData[]> clusters = ClusterUtil.ClusterVertices(clusterSize, trianglesData);
            TriangleData[] reducedTriangleData = ClusterUtil.ReduceClusters(clusters);

            int[] colorAveragesCount = new int[ColorConstants.ColorTypesCount];
            for(int i = 0; i < reducedTriangleData.Length; i++)
            {
                colorAveragesCount[(int)reducedTriangleData[i].AverageColor]++;
            }

            PrintResults(colorAveragesCount);
            return BuildProbesData(reducedTriangleData);
        }

        public static ColorProbe[] GenerateProbes(Mesh mesh, Color color, float clusterSize = 1f)
        {
            Mesh transformedMesh = SplitLargeTriangles(mesh, clusterSize);
            Vector3[] vertices = transformedMesh.vertices;
            Vector2[] uvs = transformedMesh.uv;
            int[] triangles = transformedMesh.triangles;
            int trianglesCount = triangles.Length / 3;
            TriangleData[] trianglesData = new TriangleData[trianglesCount];

            Parallel.For(0, trianglesCount, (index) => {
                int currentIndex = index * 3;

                Color averageColor = color;
                Vector3 center = VectorMath.CalculateTriangleCenter(
                    vertices[triangles[currentIndex]],
                    vertices[triangles[currentIndex + 1]],
                    vertices[triangles[currentIndex + 2]]);
                float area = VectorMath.CalculateTriangleArea(
                    vertices[triangles[currentIndex]],
                    vertices[triangles[currentIndex + 1]],
                    vertices[triangles[currentIndex + 2]]);

                TriangleData triangleData = new TriangleData(averageColor, center, area);
                trianglesData[index] = triangleData;
            });

            Dictionary<int, TriangleData[]> clusters = ClusterUtil.ClusterVertices(clusterSize, trianglesData);
            TriangleData[] reducedTriangleData = ClusterUtil.ReduceClusters(clusters);

            int[] colorAveragesCount = new int[ColorConstants.ColorTypesCount];
            for(int i = 0; i < reducedTriangleData.Length; i++)
            {
                colorAveragesCount[(int)reducedTriangleData[i].AverageColor]++;
            }

            PrintResults(colorAveragesCount);
            return BuildProbesData(reducedTriangleData);
        }

        private static Mesh SplitLargeTriangles(Mesh mesh, float areaThreshold)
        {
            List<int> newTriangles = new List<int>();
            List<Vector3> newVertices = new List<Vector3>();
            List<Vector2> newUvs = new List<Vector2>();

            int trianglesCount = mesh.triangles.Length / 3;
            int nextVertexIndex = mesh.vertices.Length;
            Queue<MeshTriangle> trianglesToSplit = new Queue<MeshTriangle>();
            float[] magnitudes = new float[3];

            for(int i = 0, currentIndex = 0; i < trianglesCount; i++, currentIndex = i * 3)
            {
                float area = VectorMath.CalculateTriangleArea(
                    mesh.vertices[mesh.triangles[currentIndex]],
                    mesh.vertices[mesh.triangles[currentIndex + 1]],
                    mesh.vertices[mesh.triangles[currentIndex + 2]]);

                if (area <= areaThreshold)
                {
                    newTriangles.Add(mesh.triangles[currentIndex]);
                    newTriangles.Add(mesh.triangles[currentIndex + 1]);
                    newTriangles.Add(mesh.triangles[currentIndex + 2]);
                    continue;
                }

                var triangleWithUvs = new MeshTriangle(
                    new Vector3[] {
                        mesh.vertices[mesh.triangles[currentIndex]],
                        mesh.vertices[mesh.triangles[currentIndex + 1]],
                        mesh.vertices[mesh.triangles[currentIndex + 2]],
                    },
                    new Vector2[] {
                        mesh.uv[mesh.triangles[currentIndex]],
                        mesh.uv[mesh.triangles[currentIndex + 1]],
                        mesh.uv[mesh.triangles[currentIndex + 2]],
                    },
                    new int[] {
                        mesh.triangles[currentIndex],
                        mesh.triangles[currentIndex + 1],
                        mesh.triangles[currentIndex + 2]
                    }
                );

                trianglesToSplit.Enqueue(triangleWithUvs);
            }

            while(trianglesToSplit.Count > 0)
            {
                MeshTriangle meshTriangle = trianglesToSplit.Dequeue();
                Vector3[] triangleVertices = meshTriangle.Vertices;
                Vector2[] triangleUvs = meshTriangle.UVs;
                int[] triangleVertexIndices = meshTriangle.VertexIndices;

                magnitudes[0] = (triangleVertices[0] - triangleVertices[1]).sqrMagnitude;
                magnitudes[1] = (triangleVertices[1] - triangleVertices[2]).sqrMagnitude;
                magnitudes[2] = (triangleVertices[2] - triangleVertices[0]).sqrMagnitude;

                float max = float.NegativeInfinity;
                int index = -1;

                for(int j = 0; j < magnitudes.Length; j++)
                {
                    if(magnitudes[j] > max)
                    {
                        max = magnitudes[j];
                        index = j;
                    }
                }

                int startSplitEdgeVertexIndex = index;
                int endSplitEdgeVertexIndex = (index + 1) % 3;
                int theOtherVertexIndex = (index + 2) % 3;
                Vector3 splitVertex = (triangleVertices[startSplitEdgeVertexIndex] + triangleVertices[endSplitEdgeVertexIndex]) / 2.0f;
                Vector2 splitUvs = (triangleUvs[startSplitEdgeVertexIndex] + triangleUvs[endSplitEdgeVertexIndex]) / 2.0f;
                newVertices.Add(splitVertex);
                newUvs.Add(splitUvs);

                MeshTriangle firstSplitTriangle = new MeshTriangle(
                    new Vector3[]
                    {
                            triangleVertices[startSplitEdgeVertexIndex],
                            splitVertex,
                            triangleVertices[theOtherVertexIndex],
                    },
                    new Vector2[]
                    {
                            triangleUvs[startSplitEdgeVertexIndex],
                            splitUvs,
                            triangleUvs[theOtherVertexIndex],
                    },
                    new int[]
                    {
                            triangleVertexIndices[startSplitEdgeVertexIndex],
                            nextVertexIndex,
                            triangleVertexIndices[theOtherVertexIndex],
                    }
                );
                MeshTriangle secondSplitTriangle = new MeshTriangle(
                    new Vector3[]
                    {
                            triangleVertices[endSplitEdgeVertexIndex],
                            triangleVertices[theOtherVertexIndex],
                            splitVertex,
                    },
                    new Vector2[]
                    {
                            triangleUvs[endSplitEdgeVertexIndex],
                            triangleUvs[theOtherVertexIndex],
                            splitUvs,
                    },
                    new int[]
                    {
                            triangleVertexIndices[endSplitEdgeVertexIndex],
                            triangleVertexIndices[theOtherVertexIndex],
                            nextVertexIndex,
                    }
                );
                nextVertexIndex++;
                float area = VectorMath.CalculateTriangleArea(firstSplitTriangle.Vertices[0], firstSplitTriangle.Vertices[1], firstSplitTriangle.Vertices[2]);

                if(area > areaThreshold)
                {
                    trianglesToSplit.Enqueue(firstSplitTriangle);
                    trianglesToSplit.Enqueue(secondSplitTriangle);
                }
                else
                {
                    newTriangles.Add(firstSplitTriangle.VertexIndices[0]);
                    newTriangles.Add(firstSplitTriangle.VertexIndices[1]);
                    newTriangles.Add(firstSplitTriangle.VertexIndices[2]);
                    newTriangles.Add(secondSplitTriangle.VertexIndices[0]);
                    newTriangles.Add(secondSplitTriangle.VertexIndices[1]);
                    newTriangles.Add(secondSplitTriangle.VertexIndices[2]);
                }
            }

            Mesh newMesh = new Mesh();
            List<Vector3> newMeshVertices = mesh.vertices.ToList();
            newMeshVertices.AddRange(newVertices);
            List<Vector2> newMeshUvs = mesh.uv.ToList();
            newMeshUvs.AddRange(newUvs);

            newMesh.vertices = newMeshVertices.ToArray();
            newMesh.triangles = newTriangles.ToArray();
            newMesh.uv = newMeshUvs.ToArray();

            return newMesh;
        }

        private static ColorProbe[] BuildProbesData(TriangleData[] triangleData)
        {
            ColorProbe[] probeData = new ColorProbe[triangleData.Length];
            for(int i = 0; i < triangleData.Length; i++)
            {
                probeData[i] = new ColorProbe(triangleData[i].Center, triangleData[i].AverageColor);
            }

            return probeData;
        }

        private static void PrintResults(int[] colorAveragesCount)
        {
            Debug.Log("Red clusters:" + colorAveragesCount[(int)Color.Red]);
            Debug.Log("Yellow clusters:" + colorAveragesCount[(int)Color.Yellow]);
            Debug.Log("Green clusters:" + colorAveragesCount[(int)Color.Green]);
            Debug.Log("Cyan clusters:" + colorAveragesCount[(int)Color.Cyan]);
            Debug.Log("Blue clusters:" + colorAveragesCount[(int)Color.Blue]);
            Debug.Log("Magenta clusters:" + colorAveragesCount[(int)Color.Magenta]);
        }
    }
}
