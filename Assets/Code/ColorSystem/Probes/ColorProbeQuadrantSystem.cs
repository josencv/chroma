using System.Collections.Generic;
using UnityEngine;

namespace Chroma.ColorSystem.Probes
{
    /// <summary>
    /// This class holds all the probes of the loaded scene and stores them in a hashmap with a key based on the probe
    /// position (logical 'quadrants', or cubes to precise), so when the player absorbs color we can iterate over
    /// the quadrants near the player instead of all the probes of the scene
    /// </summary>
    public class ColorProbeQuadrantSystem
    {
        public const int AdjacentQuadrantsQuantity = 8;   // Including the current quadrant
        public const float QuadrantSize = 25;

        private Dictionary<int, ColorProbeData[]> quadrants;

        public ColorProbeQuadrantSystem(List<StaticColorProbe> probes)
        {
            BuildQuadrantData(probes);
        }

        public static Vector3 GetQuadrantCenterFromPosition(Vector3 position)
        {
            return new Vector3
            (
                QuadrantSize * Mathf.Floor(position.x / QuadrantSize) + QuadrantSize / 2,
                QuadrantSize * Mathf.Floor(position.y / QuadrantSize) + QuadrantSize / 2,
                QuadrantSize * Mathf.Floor(position.z / QuadrantSize) + QuadrantSize / 2
            );
        }

        public ColorProbeData[] GetQuadrantFromHash(int hash)
        {
            return quadrants[hash];
        }

        /// <summary>
        /// Gets the list of centers of the current quadrant and the adjacent ones of the current 'sub-quadrant'.
        /// A quadrant is divided in 8 sub-quadrants (similar to the 4 unit circle quadrants of trigonometry, but in 3D).
        /// Instead of getting all 26 adjacent quadrants (26 + the current = 27, a 3 x 3 x 3 quadrants cube), using this sub-quadrant
        /// system now we need to get only the 7 nearest adjacent quadrants, building a 2 x 2 x 2 cube. The only thing to consider is
        /// that the absorption sphere diameter should not be greater than the QuadrantSize.
        /// </summary>
        /// <param name="position">The position to to get the nearest quadrants centers of</param>
        /// <returns>A list of the 7 nearest quadrants centers, plus the current one</returns>
        public static List<Vector3> GetCurrentAndAdjacentQuadrantsCenterFromPosition(Vector3 position)
        {
            List<Vector3> centerList = new List<Vector3>(AdjacentQuadrantsQuantity);
            float xAdjacentDirection = (Mathf.Floor(position.x / QuadrantSize) * QuadrantSize + (QuadrantSize / 2.0f)) < position.x ? 1 : -1;
            float yAdjacentDirection = (Mathf.Floor(position.y / QuadrantSize) * QuadrantSize + (QuadrantSize / 2.0f)) < position.y ? 1 : -1;
            float zAdjacentDirection = (Mathf.Floor(position.z / QuadrantSize) * QuadrantSize + (QuadrantSize / 2.0f)) < position.z ? 1 : -1;

            centerList.Add(GetQuadrantCenterFromPosition(position));
            centerList.Add(GetQuadrantCenterFromPosition(position + new Vector3(0, yAdjacentDirection, 0) * QuadrantSize));
            centerList.Add(GetQuadrantCenterFromPosition(position + new Vector3(xAdjacentDirection, 0, 0) * QuadrantSize));
            centerList.Add(GetQuadrantCenterFromPosition(position + new Vector3(xAdjacentDirection, yAdjacentDirection, 0) * QuadrantSize));
            centerList.Add(GetQuadrantCenterFromPosition(position + new Vector3(0, 0, zAdjacentDirection) * QuadrantSize));
            centerList.Add(GetQuadrantCenterFromPosition(position + new Vector3(0, yAdjacentDirection, zAdjacentDirection) * QuadrantSize));
            centerList.Add(GetQuadrantCenterFromPosition(position + new Vector3(xAdjacentDirection, 0, zAdjacentDirection) * QuadrantSize));
            centerList.Add(GetQuadrantCenterFromPosition(position + new Vector3(xAdjacentDirection, yAdjacentDirection, zAdjacentDirection) * QuadrantSize));

            return centerList;
        }

        public ColorProbeData[] GetProbesFromPosition(Vector3 position)
        {
            ColorProbeData[] probes;
            quadrants.TryGetValue(GetQuadrantHash(position), out probes);
            return probes ?? new ColorProbeData[0];
        }

        /// <summary>
        /// Gets the current quadrant and the adjacent ones of the current 'sub-quadrant'. See <see cref="GetCurrentAndAdjacentQuadrantsCenterFromPosition(Vector3)"/>
        /// </summary>
        /// <param name="position">The position to to get the nearest quadrants of</param>
        /// <returns>A list of the color probes data of the current and nearest quadrants</returns>
        public List<ColorProbeData[]> GetCurrentAndAdjacentQuadrants(Vector3 position)
        {
            List<Vector3> centers = GetCurrentAndAdjacentQuadrantsCenterFromPosition(position);
            List<ColorProbeData[]> probesList = new List<ColorProbeData[]>(8);

            foreach(Vector3 center in centers)
            {
                probesList.Add(GetProbesFromPosition(center));
            }

            return probesList;
        }

        private void BuildQuadrantData(List<StaticColorProbe> probes)
        {
            quadrants = new Dictionary<int, ColorProbeData[]>();
            Dictionary<int, List<ColorProbeData>> quadrantsList = new Dictionary<int, List<ColorProbeData>>();

            int index;
            foreach(StaticColorProbe probe in probes)
            {
                index = GetQuadrantHash(probe.transform.position);
                if(!quadrantsList.ContainsKey(index))
                {
                    quadrantsList.Add(index, new List<ColorProbeData>());
                }

                quadrantsList[index].Add(new ColorProbeData(probe.transform.position, probe.Color));
                // Once added to the quadrant system, the editor color probe is no longer needed, so we can safely destroy it
                Object.Destroy(probe.gameObject);
            }

            foreach(KeyValuePair<int, List<ColorProbeData>> pair in quadrantsList)
            {
                quadrants.Add(pair.Key, pair.Value.ToArray());
            }
        }

        /// <summary>
        /// For all the probes inside the same quadrant, this algorithm, generates the same hash
        /// </summary>
        /// <param name="position">The position to calculate the hash of</param>
        /// <returns>The hash key</returns>
        public static int GetQuadrantHash(Vector3 position)
        {
            return (int)(Mathf.Floor(position.x / QuadrantSize) +
                         Mathf.Floor(position.y / QuadrantSize) * 1000 +
                         Mathf.Floor(position.z / QuadrantSize) * 1000000);
        }
    }
}
