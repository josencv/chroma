using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chroma.ColorSystem.Probes.Builder
{
    public static class ClusterUtil
    {
        public static Dictionary<int, TriangleData[]> ClusterVertices(float clusterSize, TriangleData[] triangleData)
        {
            Dictionary<int, TriangleData[]> clusters = new Dictionary<int, TriangleData[]>();
            Dictionary<int, List<TriangleData>> clustersList = new Dictionary<int, List<TriangleData>>();
            int hash;

            for(int i = 0; i < triangleData.Length; i++)
            {
                hash = GetClusterHash(triangleData[i].Center, clusterSize);
                if(!clustersList.ContainsKey(hash))
                {
                    clustersList.Add(hash, new List<TriangleData>());
                }

                clustersList[hash].Add(triangleData[i]);
            }

            foreach(KeyValuePair<int, List<TriangleData>> pair in clustersList)
            {
                clusters.Add(pair.Key, pair.Value.ToArray());
            }

            return clusters;
        }

        public static TriangleData[] ReduceClusters(Dictionary<int, TriangleData[]> clusters)
        {
            List<TriangleData> reducedTriangles = new List<TriangleData>();

            foreach(KeyValuePair<int, TriangleData[]> pair in clusters)
            {
                int[] colorsCount = new int[ColorConstants.ColorTypesCount];
                float totalArea = 0;
                Vector3 average = Vector3.zero;
                foreach(TriangleData triangle in pair.Value)
                {
                    average = average + (triangle.Center * triangle.Area);
                    totalArea += triangle.Area;
                    colorsCount[(int)triangle.AverageColor]++;
                }

                int maxValue = colorsCount.Max();
                int maxIndex = colorsCount.ToList().IndexOf(maxValue);
                average = average / totalArea;

                reducedTriangles.Add(new TriangleData((Color)maxIndex, average, totalArea));
            }

            return reducedTriangles.ToArray();
        }

        private static int GetClusterHash(Vector3 position, float clusterSize)
        {
            return (int)(Mathf.Floor(position.x / clusterSize) +
                         Mathf.Floor(position.y / clusterSize) * 1000 +
                         Mathf.Floor(position.z / clusterSize) * 1000000);
        }
    }
}
