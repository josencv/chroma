using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

namespace Chroma.ColorSystem
{
    public class QuadrantSystem : MonoBehaviour
    {
        public const float QuadrandSize = 10;

        private List<ColorProbe> probes;
        private Dictionary<int, ColorProbeData[]> quadrants;

        private void Awake()
        {
            quadrants = new Dictionary<int, ColorProbeData[]>();
        }

        [Inject]
        private void Inject(List<ColorProbe> probes)
        {
            this.probes = probes;
        }

        private void Start()
        {
            Dictionary<int, List<ColorProbeData>> quadrantsList = new Dictionary<int, List<ColorProbeData>>();

            int index;
            foreach(ColorProbe probe in probes)
            {
                index = GetQuadrantHash(probe.transform.position);
                if(!quadrantsList.ContainsKey(index))
                {
                    quadrantsList.Add(index, new List<ColorProbeData>());
                }

                quadrantsList[index].Add(new ColorProbeData(probe.Color));
            }

            foreach(KeyValuePair<int, List<ColorProbeData>> pair in quadrantsList)
            {
                quadrants.Add(pair.Key, pair.Value.ToArray());
            }
        }

        private static int GetQuadrantHash(Vector3 position)
        {
            return (int)(math.floor(position.x / QuadrandSize) +
                         math.floor(position.y / QuadrandSize) * 1000 +
                         math.floor(position.z / QuadrandSize) * 1000000);
        }
    }
}
