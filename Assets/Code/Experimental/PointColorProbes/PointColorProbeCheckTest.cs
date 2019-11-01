using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Chroma.Experimental.PointColorProbes
{
    public class PointColorProbeCheckTest : MonoBehaviour
    {
        private PointColorProbe[] probesArray;
        private List<PointColorProbe> probesList;

        private void Awake()
        {
            //GenerateRandomProbesArray(50000);
            GenerateRandomProbesList(1000000);
        }

        private void Update()
        {
            //TestPointsInSphereArray(float3.zero, 10);
            TestPointsInSphereList(float3.zero, 10);
        }

        private void GenerateRandomProbesArray(int quantity)
        {
            probesArray = new PointColorProbe[quantity];  
            Random rnd = new Random();
            rnd.InitState();

            for(int i = 0; i < probesArray.Length; i++)
            {
                float3 randomPoint = rnd.NextFloat3();
                probesArray[i] = new PointColorProbe(randomPoint, 1);
            }
        }

        private void GenerateRandomProbesList(int quantity)
        {
            probesList = new List<PointColorProbe>();
            Random rnd = new Random();
            rnd.InitState();

            for(int i = 0; i < quantity; i++)
            {
                float3 randomPoint = rnd.NextFloat3();
                probesList.Add(new PointColorProbe(randomPoint, 1));
            }
        }

        private float TestPointsInSphereArray(float3 position, float radius)
        {
            float count = 0;
            float squaredRadius = radius * radius;
            for(int i = 0; i < probesArray.Length; i++)
            {
                if(math.dot(probesArray[i].location, position) < squaredRadius)
                {
                    count++;
                }
                //count++;
                //if(squaredDistance < squaredRadius)
                //{
                //    count++;
                //}
            }

            return count;
        }

        private float TestPointsInSphereList(float3 position, float radius)
        {
            float count = 0;
            float squaredRadius = radius * radius;
            foreach(PointColorProbe probe in probesList)
            {
                if(math.dot(probe.location, position) < squaredRadius)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
