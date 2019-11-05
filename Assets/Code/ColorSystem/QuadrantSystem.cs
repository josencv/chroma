using System.Collections.Generic;
using UnityEngine;

namespace Chroma.ColorSystem
{
    public class QuadrantSystem
    {
        public const float QuadrandSize = 10;

        private Dictionary<int, ColorProbeData[]> quadrants;

        public QuadrantSystem(List<ColorProbe> probes)
        {
            BuildQuadrantData(probes);
        }

        private void BuildQuadrantData(List<ColorProbe> probes)
        {
            quadrants = new Dictionary<int, ColorProbeData[]>();
            Dictionary<int, List<ColorProbeData>> quadrantsList = new Dictionary<int, List<ColorProbeData>>();

            int index;
            foreach(ColorProbe probe in probes)
            {
                index = GetQuadrantHash(probe.transform.position);
                if(!quadrantsList.ContainsKey(index))
                {
                    quadrantsList.Add(index, new List<ColorProbeData>());
                }

                quadrantsList[index].Add(new ColorProbeData(probe.transform.position, probe.Color));
                Object.Destroy(probe.gameObject);
            }

            foreach(KeyValuePair<int, List<ColorProbeData>> pair in quadrantsList)
            {
                quadrants.Add(pair.Key, pair.Value.ToArray());
            }
        }

        private static int GetQuadrantHash(Vector3 position)
        {
            return (int)(Mathf.Floor(position.x / QuadrandSize) +
                         Mathf.Floor(position.y / QuadrandSize) * 1000 +
                         Mathf.Floor(position.z / QuadrandSize) * 1000000);
        }

        public static Vector3 GetQuadrantCenterFromPosition(Vector3 position)
        {
            return new Vector3
            (
                QuadrandSize * Mathf.Floor(position.x / QuadrandSize) + QuadrandSize / 2,
                QuadrandSize * Mathf.Floor(position.y / QuadrandSize) + QuadrandSize / 2,
                QuadrandSize * Mathf.Floor(position.z / QuadrandSize) + QuadrandSize / 2
            );
        }

        public ColorProbeData[] GetProbesFromPosition(Vector3 position)
        {
            ColorProbeData[] probes;
            quadrants.TryGetValue(GetQuadrantHash(position), out probes);
            return probes ?? new ColorProbeData[0];
        }
    }
}
